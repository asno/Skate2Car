using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Game : MonoBehaviour
{
    private const string GAME_CONFIG_FILENAME = "gamesetup.json";

    private static Game _instance;

    [SerializeField]
    private TextAsset m_configFileTemplate;
    [SerializeField]
    private int m_targetFrameRate = 60;
    [LabelOverride("First Screen")]
    [SerializeField]
    private Screen m_screen;

    private bool m_isPaused;
    private float m_timer;
    private float m_scrollingSpeed;
    private DecorManager m_decorManager;
    private CharacterManager m_characterManager;
    private CinematicManager m_cinematicManager;
    private Obstacle[] m_obstacles;
    private Queue<Obstacle> m_obstaclesQueue;
    private Queue<Tuple<string, float>> m_spawningBid;
    private Queue<float> m_spawningTime;
    private Queue<float> m_cinematicsTime;
    private GameSetup m_gameSetup;
    private bool m_isTimerPaused = true;

    public static Game Instance { get => _instance;}

    public float Timer { get => m_timer; }
    public bool IsTimerPaused { get => m_isTimerPaused; set => m_isTimerPaused = value; }
    public CharacterController2D CharacterController { get => m_characterManager.CurrentCharacterController; }
    public Decor @Decor { get => m_decorManager.CurrentDecor; }

    void Awake()
    {
        Application.targetFrameRate = m_targetFrameRate;

        _instance = this;
        Debug.Assert(m_screen != null, "Unexpected null reference to m_screen");
        Debug.Assert(m_configFileTemplate != null, "Unexpected null reference to m_configFile");
        m_decorManager = GetComponentInChildren<DecorManager>();
        Debug.Assert(m_decorManager != null, "Unexpected null reference to m_decorManager");
        m_characterManager = GetComponent<CharacterManager>();
        Debug.Assert(m_characterManager != null, "Unexpected null reference to m_characterManager");
        m_cinematicManager = GetComponentInChildren<CinematicManager>();
        Debug.Assert(m_cinematicManager != null, "Unexpected null reference to m_cinematicManager");
        m_obstacles = GetComponentsInChildren<Obstacle>(true);

        LoadGameSetupFile();

        m_scrollingSpeed = 1.0f / m_gameSetup.ScrollingSetup.Road;
        m_decorManager.Initialize(m_gameSetup.ScrollingSetup);

        m_screen.Initialize();
        InitializeQueues();
    }

    void Start()
    {
        m_screen.Begin();
    }


    void Update()
    {
        if (m_isPaused)
            return;

        m_screen.GetCurrentScreen().DoUpdate();

        if (!m_isTimerPaused)
            m_timer += Time.deltaTime;

        if (m_spawningTime != null && m_spawningTime.Count > 0)
        {
            if (m_spawningTime.First() <= m_timer)
            {
                m_spawningTime.Dequeue();
                var bid = m_spawningBid.Dequeue();
                Obstacle obstacle = m_obstaclesQueue.Dequeue();
                obstacle.gameObject.SetActive(true);
                while (!obstacle.transform.name.Contains(bid.Item1))
                {
                    obstacle.gameObject.SetActive(false);
                    m_obstaclesQueue.Enqueue(obstacle);
                    obstacle = m_obstaclesQueue.Dequeue();
                    obstacle.gameObject.SetActive(true);
                }
                m_obstaclesQueue.Enqueue(obstacle);
                var pos = obstacle.transform.position;
                pos.y = bid.Item2;
                obstacle.transform.position = pos;
                obstacle.StartScrolling(m_scrollingSpeed);
            }
        }
        if (m_cinematicsTime != null && m_cinematicsTime.Count > 0)
        {
            if (m_cinematicsTime.First() <= m_timer)
            {
                m_cinematicsTime.Dequeue();
                m_cinematicManager.StartNextCinematic();
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        m_isPaused = !hasFocus;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        m_isPaused = pauseStatus;
    }

    public void Reset()
    {
        m_timer = 0;

        InitializeQueues();
        foreach (var o in m_obstacles)
            o.Reset();
        m_characterManager.Reset();
        m_decorManager.Reset();
    }

    private void InitializeQueues()
    {
        m_cinematicsTime = new Queue<float>(m_gameSetup.CinematicTimer);
        if (m_obstacles != null)
        {
            m_obstaclesQueue = new Queue<Obstacle>(m_obstacles);
            m_spawningBid = new Queue<Tuple<string, float>>(m_gameSetup.ObstacleSetup.Select(o => new Tuple<string, float>(o.Name, Mathf.Clamp(o.Y, -5, -2))));
            m_spawningTime = new Queue<float>(m_gameSetup.ObstacleSetup.Select(o => o.Time));
        }
    }

    public void ContinueToNextStage()
    {
        m_decorManager.PickNextDecor();
        m_characterManager.PickNextCharacterController();
    }

    private void LoadGameSetupFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, GAME_CONFIG_FILENAME);
        if (!File.Exists(filePath))
        {
            string setupText = m_configFileTemplate.ToString();
            File.WriteAllText(filePath, setupText);

            //GameSetup gs = new GameSetup();
            //gs.ScrollingSetup = new ScrollingSetup[]
            //{
            //    new ScrollingSetup() {Name = "Sky", Speed = 50.0f},
            //    new ScrollingSetup() {Name = "City", Speed = 20.0f},
            //    new ScrollingSetup() {Name = "Grass", Speed = 5.0f},
            //    new ScrollingSetup() {Name = "Road", Speed = 2.0f}
            //};
            //gs.ObstacleSetup = new ObstacleSetup[]
            //{
            //    new ObstacleSetup() {Name = "plot", Time = 3.0f, Y = -2.0f},
            //    new ObstacleSetup() {Name = "cat", Time = 7.0f, Y = -4.0f},
            //    new ObstacleSetup() {Name = "oil", Time = 11.0f, Y = -1.0f}
            //};
            //gs.CinematicTimer = new float[] { 5.5f, 9.5f, 13.5f };
            //string json = JsonUtility.ToJson(gs);
            //File.WriteAllText(filePath, json);
        }
        m_gameSetup = FileHandler.ReadFromJSON<GameSetup>(GAME_CONFIG_FILENAME);

        Array.Sort(m_gameSetup.ObstacleSetup, delegate (ObstacleSetup o1, ObstacleSetup o2) {
            return o1.Time.CompareTo(o2.Time);
        });
        Array.Sort(m_gameSetup.CinematicTimer, delegate (float t1, float t2) {
            return t1.CompareTo(t2);
        });
    }
}

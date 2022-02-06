using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    private const float RESET_BUTTON_TIME = 5f;
    private const string GAME_CONFIG_FILENAME = "gamesetup.json";

    private static Game _instance;

    [SerializeField]
    private TextAsset m_configFileTemplate;
    [SerializeField]
    private int m_targetFrameRate = 60;
    [SerializeField]
    private List<Screen> m_screens;
    [SerializeField]
    private ScrollingBackground m_scrollingSky;
    [SerializeField]
    private ScrollingBackground m_scrollingCity;
    [SerializeField]
    private ScrollingBackground m_scrollingGrass;
    [SerializeField]
    private ScrollingBackground m_scrollingRoad;

    private float m_resetButtonTime = 0;
    private bool m_isPaused;
    private float m_timer;
    private IEnumerator m_currentScreen;
    private CharacterController2D m_characterController;
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

    void Awake()
    {
        Application.targetFrameRate = m_targetFrameRate;

        _instance = this;
        Debug.Assert(m_configFileTemplate != null, "Unexpected null reference to m_configFile");
        Debug.Assert(m_screens != null, "Unexpected null reference to m_screens");
        m_characterController = GetComponentInChildren<CharacterController2D>();
        Debug.Assert(m_characterController != null, "Unexpected null reference to m_characterController");
        m_cinematicManager = GetComponentInChildren<CinematicManager>();
        Debug.Assert(m_cinematicManager != null, "Unexpected null reference to m_cinematicManager");
        Debug.Assert(m_scrollingSky != null, "Unexpected null reference to m_scrollingSky");
        Debug.Assert(m_scrollingCity != null, "Unexpected null reference to m_scrollingCity");
        Debug.Assert(m_scrollingGrass != null, "Unexpected null reference to m_scrollingGrass");
        Debug.Assert(m_scrollingRoad != null, "Unexpected null reference to m_scrollingRoad");
        m_obstacles = GetComponentsInChildren<Obstacle>();

        LoadGameSetupFile();

        m_scrollingSky.Speed = 1.0f / m_gameSetup.ScrollingSetup.FirstOrDefault(s => s.Name == "Sky").Speed;
        m_scrollingCity.Speed = 1.0f / m_gameSetup.ScrollingSetup.FirstOrDefault(s => s.Name == "City").Speed;
        m_scrollingGrass.Speed = 1.0f / m_gameSetup.ScrollingSetup.FirstOrDefault(s => s.Name == "Grass").Speed;
        m_scrollingRoad.Speed = 1.0f / m_gameSetup.ScrollingSetup.FirstOrDefault(s => s.Name == "Road").Speed;
    }

    void Start()
    {
        m_currentScreen = m_screens.GetEnumerator();
        Reset();
    }


    void Update()
    {
        if (m_isPaused)
            return;

        m_resetButtonTime = Input.GetButton("Fire2") ? m_resetButtonTime + Time.deltaTime : 0;
        if (m_resetButtonTime >= RESET_BUTTON_TIME && m_characterController.CanMove() && !m_isTimerPaused)
            Reset();

        if (!m_isTimerPaused)
            m_timer += Time.deltaTime;

        if (m_spawningTime != null && m_spawningTime.Count > 0)
        {
            if (m_spawningTime.First() <= m_timer)
            {
                m_spawningTime.Dequeue();
                var bid = m_spawningBid.Dequeue();
                Obstacle obstacle = m_obstaclesQueue.Dequeue();
                while (!obstacle.transform.name.Contains(bid.Item1))
                {
                    m_obstaclesQueue.Enqueue(obstacle);
                    obstacle = m_obstaclesQueue.Dequeue();
                }
                m_obstaclesQueue.Enqueue(obstacle);
                var pos = obstacle.transform.position;
                pos.y = bid.Item2;
                obstacle.transform.position = pos;
                obstacle.StartScrolling(m_scrollingRoad.Speed);
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
        m_screens.ForEach(s => s.Reset());
        m_currentScreen.Reset();
        m_currentScreen.MoveNext();
        m_characterController.Reset();
        m_cinematicManager.Reset();
        m_cinematicsTime = new Queue<float>(m_gameSetup.CinematicTimer);
        if (m_obstacles != null)
        {
            foreach (var o in m_obstacles)
                o.Reset();
            m_obstaclesQueue = new Queue<Obstacle>(m_obstacles);
            m_spawningBid = new Queue<Tuple<string, float>>(m_gameSetup.ObstacleSetup.Select(o => new Tuple<string, float>(o.Name, Mathf.Clamp(o.Y, -5, -2))));
            m_spawningTime = new Queue<float>(m_gameSetup.ObstacleSetup.Select(o => o.Time));
        }
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
    }
}

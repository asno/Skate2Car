using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
    private bool m_canRandomizeBonusSpawn;
    private CharacterManager m_characterManager;
    private Obstacle[] m_obstacles;
    private Queue<Obstacle> m_obstaclesQueue;
    private Queue<Tuple<string, float>> m_spawningBid;
    private Queue<float> m_spawningTime;
    private Queue<float> m_cinematicsTime;
    private GameSetup m_gameSetup;
    private BonusManager m_bonusManager;
    private Timer m_timer;

    public static Game Instance { get => _instance ??= FindObjectOfType<Game>(); }
    public CharacterController2D CharacterController { get => m_characterManager.CurrentCharacterController; }
    public Timer @Timer { get => m_timer; }

    void Awake()
    {
        Application.targetFrameRate = m_targetFrameRate;

        Debug.Assert(m_screen != null, "Unexpected null reference to m_screen");
        Debug.Assert(m_configFileTemplate != null, "Unexpected null reference to m_configFile");
        m_characterManager = GetComponent<CharacterManager>();
        Debug.Assert(m_characterManager != null, "Unexpected null reference to m_characterManager");
        m_characterManager = GetComponent<CharacterManager>();
        Debug.Assert(m_characterManager != null, "Unexpected null reference to m_characterManager");
        m_obstacles = GetComponentsInChildren<Obstacle>(true);
        m_timer = GetComponent<Timer>();
        Debug.Assert(m_timer != null, "Unexpected null reference to m_timerInstance");

        LoadGameSetupFile();

        Global.ScrollingSpeed = m_gameSetup.ScrollingSetup.Road;
        DecorManager.Instance.Initialize(m_gameSetup.ScrollingSetup, m_gameSetup.PropSpawnerSetup);
        InitializeQueues();
    }

    void Start()
    {
        m_screen.Initialize();
        m_screen.Begin();
    }


    void Update()
    {
        if (m_isPaused)
            return;

        m_screen.GetCurrentScreen().DoUpdate();

        if (m_canRandomizeBonusSpawn)
        {
            m_canRandomizeBonusSpawn = false;
            float spawnTime = UnityEngine.Random.Range(0.5f, 5.5f);
            StartCoroutine(SpawnBonus(spawnTime));
        }

        if (m_spawningTime != null && m_spawningTime.Count > 0)
        {
            if (m_spawningTime.First() <= m_timer.Time)
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
                obstacle.StartScrolling();
            }
        }
        if (m_cinematicsTime != null && m_cinematicsTime.Count > 0)
        {
            float time = m_cinematicsTime.First<float>();
            if (time <= m_timer.Time && !m_timer.IsPaused)
            {
                m_cinematicsTime.Dequeue();
                CinematicManager.Instance.StartNextCinematic();
            }
        }
    }

    private IEnumerator SpawnBonus(float aDelay)
    {
        yield return new WaitForSeconds(aDelay);
        while(m_timer.IsPaused)
        {
            yield return null;
        }
        if (m_bonusManager.enabled)
        {
            m_bonusManager.SpawnBonus();
            m_canRandomizeBonusSpawn = true;
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
        m_timer.Restart();

        InitializeQueues();
        foreach (var o in m_obstacles)
            o.Reset();
        m_bonusManager = GetComponentInChildren<BonusManager>();
        m_bonusManager.Reset();
        m_characterManager.Reset();
        Score.Instance.Reset();
        DecorManager.Instance.Reset();
        CinematicManager.Instance.Reset();

        m_canRandomizeBonusSpawn = m_bonusManager.enabled;
    }

    private void InitializeQueues()
    {
        List<float> times = new List<float>();
        times.Add(0);
        times.AddRange(m_gameSetup.CinematicTimer);
        m_cinematicsTime = new Queue<float>(times);
        if (m_obstacles != null)
        {
            m_obstaclesQueue = new Queue<Obstacle>(m_obstacles);
            m_spawningBid = new Queue<Tuple<string, float>>(m_gameSetup.ObstacleSetup.Select(o => new Tuple<string, float>(o.Name, Mathf.Clamp(o.Y, -5, -2))));
            m_spawningTime = new Queue<float>(m_gameSetup.ObstacleSetup.Select(o => o.Time));
        }
    }

    public void StopGame()
    {
        DecorManager.Instance.CurrentDecor.PauseScrolling();
        DecorManager.Instance.gameObject.SetActive(false);
        CinematicManager.Instance.gameObject.SetActive(false);
        CharacterController.CanControl = false;
        CharacterController.gameObject.SetActive(false);
        m_timer.Restart(false);
    }

    public void ContinueToNextStage()
    {
        DecorManager.Instance.PickNextDecor();
        m_characterManager.PickNextCharacterController();
        m_bonusManager = GetComponentInChildren<BonusManager>();
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

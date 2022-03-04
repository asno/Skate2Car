using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    private const string GAME_CONFIG_FILENAME = "gamesetup.json";
    private const float FREEZE_SCORE_DELAY = 4;
    private const int BONUS_SPAWN_PER_GAME = 10;

    private static Game _instance;

    [SerializeField]
    private TextAsset m_configFileTemplate;
    [SerializeField]
    private int m_targetFrameRate = 60;
    [SerializeField]
    private float m_playTime;
    [LabelOverride("First Screen")]
    [SerializeField]
    private Screen m_screen;

    private bool m_isPaused;
    private bool m_canRandomizeBonusSpawn;
    private int m_bonusSpawnCount;
    private CharacterManager m_characterManager;
    private Obstacle[] m_obstacles;
    private Queue<Obstacle> m_obstaclesQueue;
    private Queue<Tuple<string, float>> m_spawningBid;
    private Queue<float> m_spawningTime;
    private Queue<float> m_cinematicsTime;
    private GameSetup m_gameSetup;
    private BonusManager m_bonusManager;
    private Score m_score;
    private Timer m_timer;
    private float m_idleControlTime;
    private UnityEvent m_dirtyControlEvent = new UnityEvent();

    public static Game Instance { get => _instance ??= FindObjectOfType<Game>(); }
    public CharacterController2D CharacterController { get => m_characterManager.CurrentCharacterController; }
    public Timer @Timer { get => m_timer; }
    public float PlayTime { get => m_playTime; }

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

        m_dirtyControlEvent.AddListener(ResetIdleTime);
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

        if (!m_timer.IsPaused)
        {
            m_idleControlTime += Time.deltaTime;
            m_score.AutoIncrement = m_idleControlTime < FREEZE_SCORE_DELAY;
            if (!m_score.AutoIncrement && m_characterManager.CurrentCharacterController.HasPerformedAction())
                m_dirtyControlEvent.Invoke();
        }

        if (m_canRandomizeBonusSpawn)
        {
            m_canRandomizeBonusSpawn = false;
            float spawnTime = UnityEngine.Random.Range(m_playTime / BONUS_SPAWN_PER_GAME * 0.9f, m_playTime / BONUS_SPAWN_PER_GAME);
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
        if (m_bonusManager.enabled && m_bonusSpawnCount < BONUS_SPAWN_PER_GAME)
        {
            m_bonusManager.SpawnBonus();
            m_canRandomizeBonusSpawn = true;
            m_bonusSpawnCount++;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        m_isPaused = !hasFocus;
        m_timer.Resume();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        m_isPaused = pauseStatus;
        if (m_isPaused)
            m_timer.Pause();
    }

    public void Reset()
    {
        m_timer.Restart();

        InitializeQueues();
        foreach (var o in m_obstacles)
            o.Reset();

        m_score = Score.Instance;
        m_score.Reset();
        m_characterManager.Reset();
        DecorManager.Instance.Reset();
        CinematicManager.Instance.Reset();
        m_bonusManager = DecorManager.Instance.CurrentDecor.GetComponentInChildren<BonusManager>(true);
        m_bonusManager.Reset();

        m_canRandomizeBonusSpawn = true;
        m_bonusSpawnCount = 0;
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

    public void StopGame()
    {
        DecorManager.Instance.CurrentDecor.PauseScrolling();
        DecorManager.Instance.gameObject.SetActive(false);
        CharacterController.CanControl = false;
        CharacterController.gameObject.SetActive(false);
        foreach (var o in m_obstacles)
            o.Reset();
        m_timer.Restart(false);
    }

    public void ContinueToNextStage()
    {
        DecorManager.Instance.PickNextDecor();
        m_characterManager.PickNextCharacterController();
        m_bonusManager = DecorManager.Instance.CurrentDecor.GetComponentInChildren<BonusManager>(true);
        m_bonusManager.Reset();
    }

    private void LoadGameSetupFile()
    {
        //string filePath = Path.Combine(Application.persistentDataPath, GAME_CONFIG_FILENAME);
        //if (!File.Exists(filePath))
        //{
        //    string setupText = m_configFileTemplate.ToString();
        //    File.WriteAllText(filePath, setupText);
        //}
        //m_gameSetup = FileHandler.ReadFromJSON<GameSetup>(GAME_CONFIG_FILENAME);
        m_gameSetup = JsonUtility.FromJson<GameSetup>(m_configFileTemplate.text);

        Array.Sort(m_gameSetup.ObstacleSetup, delegate (ObstacleSetup o1, ObstacleSetup o2) {
            return o1.Time.CompareTo(o2.Time);
        });
        Array.Sort(m_gameSetup.CinematicTimer, delegate (float t1, float t2) {
            return t1.CompareTo(t2);
        });
    }

    private void ResetIdleTime()
    {
        m_idleControlTime = 0;
    }
}

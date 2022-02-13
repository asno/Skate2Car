using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    private static InfoDisplay _instance;

    [SerializeField]
    private Text m_timer;
    [SerializeField]
    private Text m_score;
    [SerializeField]
    private Text m_fps;
    [SerializeField]
    private Text m_logs;

    public static InfoDisplay Instance { get => _instance ??= FindObjectOfType<InfoDisplay>(); }

    private Score m_scoreInstance;
    private List<string> m_logsLines;

    private Game m_game;
    private readonly string TIMER = "Timer: {0}";
    private readonly string SCORE = "Score: {0}";
    private readonly string FPS = "FPS: {0}";
    private readonly int MAX_LINES_OF_LOGS = 5;

    void Awake()
    {
        Debug.Assert(m_timer != null, "Unexpected null reference to m_timer");
        Debug.Assert(m_fps != null, "Unexpected null reference to m_fps");
    }

    void Start()
    {
        m_game = Game.Instance;
        Debug.Assert(m_game != null, "Unexpected null reference to m_game");
        m_scoreInstance = Score.Instance;
        Debug.Assert(m_scoreInstance != null, "Unexpected null reference to m_score");

        InvokeRepeating(nameof(GetFPS), 1, 1); 
    }

    void Update()
    {
        TimeSpan time = TimeSpan.FromSeconds(m_game.Timer.Time);
        m_timer.text = string.Format(TIMER, time.ToString("hh':'mm':'ss"));
        m_score.text = string.Format(SCORE, m_scoreInstance.Points);
    }

    private void GetFPS()
    {
        var fps = (int)(1f / Time.unscaledDeltaTime);
        m_fps.text = string.Format(FPS, fps);
    }

    public static void Log(string aMessage)
    {
        var lines = Instance.m_logsLines ??= new List<string>(_instance.MAX_LINES_OF_LOGS);
        if(lines.Count >= Instance.MAX_LINES_OF_LOGS)
            lines.RemoveAt(0);

        TimeSpan time = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
        string newLine = $"{time.ToString("hh':'mm':'ss")} > {aMessage}";
        lines.Add(newLine);

        Instance.m_logs.text = string.Join("\n", lines);
        Debug.Log(aMessage);
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField]
    private Text m_timer;
    [SerializeField]
    private Text m_score;
    [SerializeField]
    private Text m_fps;
    private Score m_scoreInstance;

    private Game m_game;
    private readonly string TIMER = "Timer: {0}";
    private readonly string SCORE = "Score: {0}";
    private readonly string FPS = "FPS: {0}";

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
}

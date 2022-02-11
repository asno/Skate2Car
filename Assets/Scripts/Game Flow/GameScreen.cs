using UnityEngine;
using UnityEngine.UI;

public class GameScreen : Screen
{
    private const float RESET_BUTTON_TIME = 5f;
    private const string SCORE = "Score: {0}";

    [SerializeField]
    private float m_playTime = 150;
    [SerializeField]
    private Text m_score;

    private Score m_scoreInstance;
    private bool m_canReset = true;
    private float m_resetButtonTime = 0;


    public override void Initialize()
    {
        base.Initialize();
        Deactivate();
    }

    protected override void Deactivate()
    {
        m_game.StopGame();
        m_score.gameObject.SetActive(false);
    }

    public override void Begin()
    {
        m_isSkipped = false;
        m_scoreInstance = Score.Instance;
        DecorManager.Instance.CurrentDecor.ResumeScrolling();

        m_game.CharacterController.gameObject.SetActive(true);
        m_game.CharacterController.CanControl = true;
        DecorManager.Instance.gameObject.SetActive(true);
        CinematicManager.Instance.gameObject.SetActive(true);
        m_score.gameObject.SetActive(true);

        m_game.Reset();
    }

    protected override void Exit()
    {
        base.Exit();
        Deactivate();
        m_isSkipped = true;
    }

    public override void DoUpdate()
    {
        m_score.text = string.Format(SCORE, m_scoreInstance.Points);
        if (m_game.Timer.Time >= m_playTime)
            Exit();

        m_resetButtonTime = Input.GetButton("Fire2") ? m_resetButtonTime + Time.deltaTime : 0;
        if (m_canReset)
        {
            if (m_resetButtonTime >= RESET_BUTTON_TIME && m_game.CharacterController.CanMove() && !m_game.Timer.IsPaused)
                Reset();
        }
        else
            m_canReset = Input.GetButtonUp("Fire2");
    }

    public override void Reset()
    {
        m_game.Reset();
        m_canReset = false;
    }
}

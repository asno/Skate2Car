using UnityEngine;

public class GameScreen : Screen
{
    private const float RESET_BUTTON_TIME = 5f;

    [SerializeField]
    private float m_playTime = 150;

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
    }

    public override void Begin()
    {
        m_isSkipped = false;
        DecorManager.Instance.CurrentDecor.ResumeScrolling();

        m_game.CharacterController.gameObject.SetActive(true);
        m_game.CharacterController.CanControl = true;
        DecorManager.Instance.gameObject.SetActive(true);
        CinematicManager.Instance.gameObject.SetActive(true);

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
        if (Timer.Instance.Time >= m_playTime)
            Exit();

        m_resetButtonTime = Input.GetButton("Fire2") ? m_resetButtonTime + Time.deltaTime : 0;
        if (m_canReset)
        {
            if (m_resetButtonTime >= RESET_BUTTON_TIME && m_game.CharacterController.CanMove() && !Timer.Instance.IsPaused)
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

using UnityEngine;

public class GameScreen : Screen
{
    private const float RESET_BUTTON_TIME = 5f;

    [SerializeField]
    private GameObject m_level;
    [SerializeField]
    private CinematicManager m_cinematicManager;

    private float m_resetButtonTime = 0;

    void Awake()
    {
        Debug.Assert(m_level != null, "Unexpected null reference to m_level");
        Debug.Assert(m_cinematicManager != null, "Unexpected null reference to m_cinematicManager");
    }

    public override void Initialize()
    {
        base.Initialize();
        m_game.IsTimerPaused = true;
        m_game.Decor.PauseScrolling();

        m_level.SetActive(false);

        m_game.CharacterController.CanControl = false;
        m_game.CharacterController.gameObject.SetActive(false);

        m_cinematicManager.gameObject.SetActive(false);
    }

    protected override void Deactivate()
    {
        m_game.Decor.PauseScrolling();

        m_level.SetActive(false);

        m_game.CharacterController.CanControl = false;
        m_game.CharacterController.gameObject.SetActive(false);

        m_cinematicManager.gameObject.SetActive(false);
    }

    public override void Begin()
    {
        m_level.SetActive(true);

        m_game.Decor.ResumeScrolling();

        m_game.CharacterController.gameObject.SetActive(true);
        m_game.CharacterController.CanControl = true;

        m_cinematicManager.gameObject.SetActive(true);
        m_game.IsTimerPaused = false;
    }

    protected override void Exit()
    {
        base.Exit();
        Reset();
        Deactivate();
    }

    public override void DoUpdate()
    {
        m_resetButtonTime = Input.GetButton("Fire2") ? m_resetButtonTime + Time.deltaTime : 0;
        if (m_resetButtonTime >= RESET_BUTTON_TIME && m_game.CharacterController.CanMove() && !m_game.IsTimerPaused)
            Reset();
    }

    public override void Reset()
    {
        m_game.Reset();
        m_game.CharacterController.Reset();
        m_cinematicManager.Reset();
    }
}

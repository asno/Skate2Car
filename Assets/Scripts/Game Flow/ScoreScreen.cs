using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : Screen
{
    private const string MESSAGE = "Score de {0} points\n{1}";

    [SerializeField]
    private RawImage m_image;
    [SerializeField]
    private Text m_text;

    private bool m_canPressToSkip;

    public override void Initialize()
    {
        base.Initialize();
        Deactivate();
    }

    protected override void Deactivate()
    {
        m_image.gameObject.SetActive(false);
        m_text.gameObject.SetActive(false);
        m_canPressToSkip = false;
    }

    public override void Begin()
    {
        m_isSkipped = false;
        m_image.gameObject.SetActive(true);
        m_text.gameObject.SetActive(true);
        m_text.text = string.Format(MESSAGE, Score.Instance.Points, Score.Instance.GetMessage());

        Invoke(nameof(CanPressToSkip), 2f);
    }

    protected override void Exit()
    {
        base.Exit();
        Deactivate();
        m_game.StopGame();
    }

    private void CanPressToSkip()
    {
        m_canPressToSkip = true;
    }

    public override void DoUpdate()
    {
        if (m_canPressToSkip)
        {
            m_isSkipped = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2");
            if (IsSkipped)
                Exit();
        }
    }
}

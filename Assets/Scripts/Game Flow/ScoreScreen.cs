using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : Screen
{
    private const string MESSAGE = "{0} PTS\n\n{1}";

    [SerializeField]
    private SpriteRenderer m_image;
    [SerializeField]
    private Text m_text;

    private float m_latestScoreupdated;
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
        m_latestScoreupdated = Score.Instance.Points;
        base.Begin();

        Invoke(nameof(EnablePressToSkip), 2f);
    }

    protected override void Exit()
    {
        base.Exit();
        Deactivate();
    }

    private void EnablePressToSkip()
    {
        m_canPressToSkip = true;
    }

    public override void DoUpdate()
    {
        if(m_latestScoreupdated != Score.Instance.Points)
        {
            m_latestScoreupdated = Score.Instance.Points;
            m_text.text = string.Format(MESSAGE, m_latestScoreupdated, Score.Instance.GetMessage());
        }
        if (m_canPressToSkip)
        {
            m_isSkipped = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2");
            if (IsSkipped)
                Exit();
        }
    }
}

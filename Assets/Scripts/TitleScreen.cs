using System.Collections;
using UnityEngine;

public class TitleScreen : Screen
{
    [SerializeField]
    private Animator m_titleScreenAnimator;

    private bool m_canPressToSkip;

    public override void Initialize()
    {
        base.Initialize();
        Deactivate();
    }

    protected override void Deactivate()
    {
        m_titleScreenAnimator.gameObject.SetActive(false);
        m_titleScreenAnimator.enabled = false;
        m_canPressToSkip = false;
    }

    public override void Begin()
    {
        m_titleScreenAnimator.gameObject.SetActive(true);
        m_titleScreenAnimator.enabled = true;
        StartCoroutine(Play());
    }

    protected override void Exit()
    {
        base.Exit();
        Reset();
        Deactivate();
    }

    public override void Reset()
    {
        m_titleScreenAnimator.Rebind();
        m_titleScreenAnimator.Update(0f);
    }

    public override void DoUpdate()
    {
        if(m_canPressToSkip)
        {
            m_isSkipped = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2");
            if (IsSkipped)
                Exit();
        }
    }

    private IEnumerator Play()
    {
        m_titleScreenAnimator.Play("ShowUp");
        while (m_titleScreenAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;

        m_canPressToSkip = true;
    }
}

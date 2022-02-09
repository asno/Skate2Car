using System.Collections;
using UnityEngine;

public class SplashScreen : Screen
{
    [SerializeField]
    private Animator m_splashScreenAnimator;

    public override void Initialize()
    {
        base.Initialize();
        Deactivate();
    }

    protected override void Deactivate()
    {
        m_splashScreenAnimator.gameObject.SetActive(false);
        m_splashScreenAnimator.enabled = false;
    }

    public override void Begin()
    {
        m_isSkipped = false;
        m_splashScreenAnimator.gameObject.SetActive(true);
        m_splashScreenAnimator.enabled = true;
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
        m_splashScreenAnimator.Rebind();
        m_splashScreenAnimator.Update(0f);
    }

    private IEnumerator Play()
    {
        m_splashScreenAnimator.Play("ShowUp");
        while (m_splashScreenAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;
        yield return new WaitForSeconds(2.0f);
        m_isSkipped = true;
        Exit();
    }
}

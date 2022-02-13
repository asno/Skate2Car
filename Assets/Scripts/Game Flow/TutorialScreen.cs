using System;
using System.Collections;
using UnityEngine;

public class TutorialScreen : Screen
{
    [SerializeField]
    private SpriteRenderer m_image;

    private Coroutine m_autoSkipProcess;
    private bool m_canPressToSkip;

    public override void Initialize()
    {
        base.Initialize();
        Deactivate();
    }

    protected override void Deactivate()
    {
        m_autoSkipProcess = null;
        m_canPressToSkip = false;
        m_image.gameObject.SetActive(false);

    }

    public override void Begin()
    {
        m_isSkipped = false;
        m_image.gameObject.SetActive(true);
        StartCoroutine(Wait(2, EnablePressToSkip));
        m_autoSkipProcess = StartCoroutine(Wait(30, SkipScreen));
    }

    protected override void Exit()
    {
        base.Exit();
        Reset();
        Deactivate();
    }

    public override void Reset()
    {
        if(m_autoSkipProcess != null)
        {
            StopCoroutine(m_autoSkipProcess);
            m_autoSkipProcess = null;
        }
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

    private void SkipScreen()
    {
        m_isSkipped = true;
        Exit();
    }

    private void EnablePressToSkip()
    {
        m_canPressToSkip = true;
    }

    private IEnumerator Wait(float aSeconds, Action aCallback = null)
    {
        yield return new WaitForSeconds(aSeconds);
        if (aCallback != null)
            aCallback();
    }
}

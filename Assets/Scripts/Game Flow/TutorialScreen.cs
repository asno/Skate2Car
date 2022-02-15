using System;
using System.Collections;
using UnityEngine;

public class TutorialScreen : Screen
{
    [SerializeField]
    private SpriteRenderer m_image;

    private bool m_canPressToSkip;

    public override void Initialize()
    {
        base.Initialize();
        Deactivate();
    }

    protected override void Deactivate()
    {
        m_canPressToSkip = false;
        m_image.gameObject.SetActive(false);

    }

    public override void Begin()
    {
        m_isSkipped = false;
        m_image.gameObject.SetActive(true);
        base.Begin();
        Invoke(nameof(EnablePressToSkip), 2f);
    }

    protected override void Exit()
    {
        base.Exit();
        Reset();
        Deactivate();
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
}

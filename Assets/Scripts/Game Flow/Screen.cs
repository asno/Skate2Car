using System;
using System.Collections;
using UnityEngine;

public abstract class Screen : MonoBehaviour
{
    [SerializeField]
    protected Screen m_nextScreen;
    [SerializeField]
    protected Screen m_fallbackScreen;
    [SerializeField]
    protected float m_timeOut;

    protected Game m_game;
    protected bool m_isInitialized = false;
    protected bool m_isSkipped;

    private Screen m_screen;
    private Coroutine m_processTimeOutDelay;

    public bool IsSkipped { get => m_isSkipped; }

    public Screen GetCurrentScreen()
    {
        return m_isSkipped ? m_screen.GetCurrentScreen() : this;
    }

    public virtual void Initialize()
    {
        if (m_nextScreen != null && !m_isInitialized)
        {
            m_screen = m_nextScreen;
            m_isInitialized = true;
            m_nextScreen.Initialize();
        }
        m_game = Game.Instance;
        Debug.Assert(m_game != null, "Unexpected null reference to m_game");

        m_isSkipped = false;
    }

    protected virtual void Deactivate()
    {
    }

    public virtual void Begin()
    {
        m_screen = m_nextScreen;

        if (m_fallbackScreen != null && m_timeOut > 0)
        {
            if (m_processTimeOutDelay != null)
                StopCoroutine(m_processTimeOutDelay);
            m_processTimeOutDelay = StartCoroutine(StartDelayToTimeOut());
        }
    }

    private IEnumerator StartDelayToTimeOut()
    {
        yield return new WaitForSeconds(m_timeOut);
        m_isSkipped = true;
        m_screen = m_fallbackScreen;
        Exit();
    }

    protected virtual void Exit()
    {
        if (m_processTimeOutDelay != null)
            StopCoroutine(m_processTimeOutDelay);

        if (m_screen != null)
            m_screen.Begin();
    }

    public virtual void Reset()
    {
    }

    public virtual void DoUpdate()
    {
    }
}

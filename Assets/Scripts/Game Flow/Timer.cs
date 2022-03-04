using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void NotifyTimerState();

public class Timer : MonoBehaviour
{
    private bool m_isStarted = false;
    private bool m_isPaused;
    private float m_time;
    private Coroutine m_process;

    public bool IsPaused { get => m_isPaused; }
    public float Time { get => m_time; }

    public event NotifyTimerState OnPause;
    public event NotifyTimerState OnResume;

    private Dictionary<float, NotifyTimerState> m_tickCallsDispatcher = new Dictionary<float, NotifyTimerState>();
    private List<Coroutine> m_ticksEvents = new List<Coroutine>();

    void Awake()
    {
        m_time = 0;
        m_isStarted = true;
        m_isPaused = true;
    }

    public void RegisterCallbackEveryTick(float aInterval, NotifyTimerState aCallback )
    {
        NotifyTimerState callback;
        if (!m_tickCallsDispatcher.TryGetValue(aInterval, out callback))
        { 
            callback = new NotifyTimerState(aCallback);
            m_ticksEvents.Add(StartCoroutine(StartTick(aInterval, callback)));
        }
        else
            callback += aCallback;
    }
    private IEnumerator StartTick(float aTick, NotifyTimerState callback)
    {
        while (m_isStarted)
        {
            if (m_isPaused)
                yield return null;
            else
            {
                yield return new WaitForSeconds(aTick);
                callback();
            }
        }
    }

    void Update()
    {
        if (m_isStarted && !m_isPaused)
            m_time += UnityEngine.Time.deltaTime;
    }

    public void Restart(bool aAutostart = true)
    {
        m_isStarted = true;
        m_isPaused = !aAutostart;
        m_time = 0;
    }

    public void StopAndUnbind()
    {
        m_isStarted = false;
        m_isPaused = true;
        m_time = 0;

        OnPause = null;
        OnResume = null;
        m_ticksEvents.ForEach(co => { if (co != null) StopCoroutine(co); });
        m_ticksEvents.Clear();
        m_tickCallsDispatcher.Clear();
    }

    public void Pause()
    {
        m_isPaused = true;
        if (OnPause != null)
            OnPause();
    }

    public void Resume()
    {
        Debug.Assert(m_isStarted, "Inconsistent logic call; Restart() must be called before Resume()");
        m_isPaused = false;
        if (OnResume != null)
            OnResume();
    }
}

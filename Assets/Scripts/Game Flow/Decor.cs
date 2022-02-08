using System;
using UnityEngine;

public class Decor : MonoBehaviour
{
    [SerializeField]
    private ScrollingBackground m_scrollingSky;
    [SerializeField]
    private ScrollingBackground m_scrollingBackdrop;
    [SerializeField]
    private ScrollingBackground m_scrollingBorder;
    [SerializeField]
    private ScrollingBackground m_scrollingRoad;

    internal void Set(ScrollingSetup aScrollingSetup)
    {
        m_scrollingSky.Speed = 1.0f / aScrollingSetup.Sky;
        m_scrollingBackdrop.Speed = 1.0f / aScrollingSetup.Backdrop;
        m_scrollingBorder.Speed = 1.0f / aScrollingSetup.Border;
        m_scrollingRoad.Speed = 1.0f / aScrollingSetup.Road;
    }

    public void PauseScrolling()
    {
        m_scrollingSky.IsPaused = true;
        m_scrollingBackdrop.IsPaused = true;
        m_scrollingBorder.IsPaused = true;
        m_scrollingRoad.IsPaused = true;
    }

    public void ResumeScrolling()
    {
        m_scrollingSky.IsPaused = false;
        m_scrollingBackdrop.IsPaused = false;
        m_scrollingBorder.IsPaused = false;
        m_scrollingRoad.IsPaused = false;
    }
}

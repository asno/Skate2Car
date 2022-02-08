using System;
using System.Collections;
using UnityEngine;

public class DecorManager : MonoBehaviour
{
    [SerializeField]
    private Decor[] m_decors;

    private IEnumerator m_decorIterator;
    private Decor m_currentDecor;

    public Decor CurrentDecor
    {
        get
        {
            InitIfNull();
            return m_currentDecor;
        }
    }

    void Awake()
    {
        Debug.Assert(m_decors != null, "Unexpected null reference to m_decors");
        Debug.Assert(m_decors.Length > 0, "Empty container m_decors");

        foreach (Decor decor in m_decors)
            decor.gameObject.SetActive(false);

        InitIfNull();
    }

    private void InitIfNull()
    {
        if (m_decorIterator == null)
        {
            m_decorIterator = m_decors.GetEnumerator();
            PickNextDecor();
        }
    }

    public void Reset()
    {
        foreach (Decor decor in m_decors)
            decor.PauseScrolling();

        m_decorIterator.Reset();
        PickNextDecor();
        m_currentDecor.ResumeScrolling();
    }

    public void PickNextDecor()
    {
        if (m_decorIterator.MoveNext())
        {
            m_currentDecor?.gameObject.SetActive(false);
            m_currentDecor = m_decorIterator.Current as Decor;
            m_currentDecor.gameObject.SetActive(true);
        }
    }

    internal void Initialize(ScrollingSetup aScrollingSetup)
    {
        foreach(Decor decor in m_decors)
            decor.Set(aScrollingSetup);
    }
}

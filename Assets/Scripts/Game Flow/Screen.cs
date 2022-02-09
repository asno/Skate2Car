using UnityEngine;

public abstract class Screen : MonoBehaviour
{
    [SerializeField]
    protected Screen m_nextScreen;

    protected Game m_game;
    protected bool m_isInitialized = false;
    protected bool m_isSkipped;

    public bool IsSkipped { get => m_isSkipped; }

    public Screen GetCurrentScreen()
    {
        return m_isSkipped ? m_nextScreen.GetCurrentScreen() : this;
    }

    public virtual void Initialize()
    {
        if (m_nextScreen != null && !m_isInitialized)
        {
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
    }

    protected virtual void Exit()
    {
        if (m_nextScreen != null)
            m_nextScreen.Begin();
    }

    public virtual void Reset()
    {
    }

    public virtual void DoUpdate()
    {
    }
}

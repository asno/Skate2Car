using UnityEngine;

public class Screen : MonoBehaviour
{
    private bool m_isSkipped;

    public bool IsSkipped { get => m_isSkipped; set => m_isSkipped = value; }

    public virtual void Reset()
    {
        m_isSkipped = false;
    }

    public virtual void CheckInput()
    {
    }
}

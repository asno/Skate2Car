using System.Collections;
using UnityEngine;

public abstract class AbstractSequence : MonoBehaviour
{
    [SerializeField]
    private bool m_isAsynchronous = true;

    public bool IsAsynchronous { get => m_isAsynchronous; }

    public abstract IEnumerator DoAction();

}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SequenceCallMethod : AbstractSequence
{
    [SerializeField]
    private UnityEvent m_invokeMethod;

    public override IEnumerator DoAction()
    {
        if (m_invokeMethod != null)
            m_invokeMethod.Invoke();
        yield return null;
    }
}
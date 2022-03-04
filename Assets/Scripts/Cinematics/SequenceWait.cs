using System.Collections;
using UnityEngine;

public class SequenceWait : AbstractSequence
{
    [SerializeField]
    private float m_seconds;

    public override IEnumerator DoAction()
    {
        yield return new WaitForSeconds(m_seconds);
    }
}

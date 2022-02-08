using System.Collections;
using UnityEngine;

public class SequenceTeleport : AbstractSequence
{
    [SerializeField]
    private GameObject m_actor;
    [SerializeField]
    private Transform m_anchor;
    [SerializeField]
    private bool m_isActive;

    public override IEnumerator DoAction()
    {
        if (m_anchor != null)
            m_actor.transform.position = m_anchor.position;
        m_actor.SetActive(m_isActive);
        yield return null;
    }
}

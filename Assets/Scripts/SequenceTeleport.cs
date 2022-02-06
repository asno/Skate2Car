using System.Collections;
using UnityEngine;

public class SequenceTeleport : AbstractSequence
{
    [SerializeField]
    private GameObject m_actor;
    [SerializeField]
    private Vector3 m_position;
    [SerializeField]
    private bool m_isActive;

    public override IEnumerator DoAction()
    {
        m_actor.transform.position = m_position;
        m_actor.SetActive(m_isActive);
        yield return null;
    }
}

using System.Collections;
using UnityEngine;

public class SequenceMoveTo : AbstractSequence
{
    [SerializeField]
    private GameObject m_actor;
    [SerializeField]
    private Transform m_anchor;
    [SerializeField]
    private float m_speed;

    public override IEnumerator DoAction()
    {
        while (m_actor.transform.position != m_anchor.position)
        {
            float step = m_speed * Time.deltaTime;
            m_actor.transform.position = Vector3.MoveTowards(m_actor.transform.position, m_anchor.position, step);
            yield return null;
        }
    }
}

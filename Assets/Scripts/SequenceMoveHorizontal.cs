using System.Collections;
using UnityEngine;

public class SequenceMoveHorizontal : AbstractSequence
{
    [SerializeField]
    private GameObject m_actor;
    [SerializeField]
    private float m_x;
    [SerializeField]
    private float m_speed;

    public override IEnumerator DoAction()
    {
        Vector3 destination = new Vector3(m_x, m_actor.transform.position.y, m_actor.transform.position.z);
        while (m_actor.transform.position != destination)
        {
            float step = m_speed * Time.deltaTime;
            m_actor.transform.position = Vector3.MoveTowards(m_actor.transform.position, destination, step);
            yield return null;
        }
    }
}

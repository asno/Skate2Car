using System.Collections;
using UnityEngine;

public class SequenceAnimation : AbstractSequence
{
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private string m_state;

    public override IEnumerator DoAction()
    {
        m_animator.Play(m_state);
        while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;
    }
}

using System.Collections;
using UnityEngine;

public class Skateboard : MonoBehaviour
{
    [SerializeField]
    private Animator[] m_characterAnimators;

    private IEnumerator m_currentCharacterAnimator;
    private Animator m_animator;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        Debug.Assert(m_animator != null, "Unexpected null reference to m_animator");
        Debug.Assert(m_characterAnimators != null, "Unexpected null reference to m_characterAnimators");
        Debug.Assert(m_characterAnimators.Length > 0, "Empty container m_characterAnimators");

        m_currentCharacterAnimator = m_characterAnimators.GetEnumerator();
        m_currentCharacterAnimator.MoveNext();
    }

    public void Reset()
    {
        m_currentCharacterAnimator.Reset();
        m_currentCharacterAnimator.MoveNext();

        foreach (var a in m_characterAnimators)
            a.gameObject.SetActive(false);

        m_characterAnimators[0].gameObject.SetActive(true);
    }

    public bool SwitchToNextCharacter()
    {
        Animator previousAnimator = m_currentCharacterAnimator.Current as Animator;
        bool hasMovedToNextCharacter = m_currentCharacterAnimator.MoveNext();
        if (hasMovedToNextCharacter)
        {
            Animator currentAnimator = m_currentCharacterAnimator.Current as Animator;
            previousAnimator.gameObject.SetActive(false);
            currentAnimator.gameObject.SetActive(true);
        }
        return hasMovedToNextCharacter;
    }

    public void PlayAnimation(string aState)
    {
        Animator animator = m_currentCharacterAnimator.Current as Animator;
        animator.Play(aState);
        m_animator.Play(aState);
    }
}

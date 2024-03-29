using System;
using System.Collections;
using UnityEngine;

public class Skateboard : MonoBehaviour
{
    [SerializeField]
    private Animator[] m_characterAnimators;
    [SerializeField]
    private GameObject[] m_spriteObjects;

    private Animator m_animator;
    private IEnumerator m_characterAnimatorIterator;
    private Animator m_currentCharacterAnimator;
    private SkateCollision m_skateCollision;

    public SkateCollision Collision { get => m_skateCollision; }

    void Awake()
    {
        Debug.Assert(m_characterAnimators != null, "Unexpected null reference to m_characterAnimators");
        Debug.Assert(m_characterAnimators.Length > 0, "Empty container m_characterAnimators");
        Debug.Assert(m_spriteObjects != null, "Unexpected null reference to m_spriteObjects");

        m_characterAnimatorIterator = m_characterAnimators.GetEnumerator();
        m_characterAnimatorIterator.MoveNext();
        m_currentCharacterAnimator = m_characterAnimatorIterator.Current as Animator;
    }

    public void Reset()
    {
        m_characterAnimatorIterator.Reset();
        m_characterAnimatorIterator.MoveNext();
        m_currentCharacterAnimator = m_characterAnimatorIterator.Current as Animator;

        foreach (var a in m_characterAnimators)
            a.gameObject.SetActive(false);

        m_characterAnimators[0].gameObject.SetActive(true);
    }

    public bool SwitchToNextCharacter()
    {
        bool hasMovedToNextCharacter = m_characterAnimatorIterator.MoveNext();
        if (hasMovedToNextCharacter)
        {
            m_currentCharacterAnimator.gameObject.SetActive(false);
            m_currentCharacterAnimator = m_characterAnimatorIterator.Current as Animator;
            m_currentCharacterAnimator.gameObject.SetActive(true);
        }
        return hasMovedToNextCharacter;
    }

    internal void SetColor(SkateColor aSkateColor)
    {
        int colorIndex = (int)aSkateColor;
        foreach (var spriteObject in m_spriteObjects)
            spriteObject.SetActive(false);

        m_spriteObjects[colorIndex].SetActive(true);
        m_animator = m_spriteObjects[colorIndex].GetComponent<Animator>();
        m_skateCollision = m_spriteObjects[colorIndex].GetComponent<SkateCollision>();
    }

    public void PlayAnimation(string aState)
    {
        m_currentCharacterAnimator.Play(aState);
        m_animator.Play(aState);
    }
}

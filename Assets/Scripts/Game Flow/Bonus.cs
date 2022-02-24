using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    [SerializeField]
    private int m_points;
    [SerializeField]
    private Animator m_animator;

    private Animator m_ownAnimator;
    private bool m_canBePicked;
    private bool m_isPopComplete;

    public int Points { get => m_points; }
    public bool CanBePicked { get => m_canBePicked; }
    public bool IsPopComplete { get => m_isPopComplete; }

    void Awake()
    {
        m_ownAnimator = GetComponent<Animator>();
        Debug.Assert(m_ownAnimator != null, "Unexpected null reference m_ownAnimator");
    }

    void Start()
    {
        m_canBePicked = false;
        m_isPopComplete = true;
    }

    public void Reset()
    {
        m_animator.Rebind();
        m_animator.Update(0f);

        m_canBePicked = false;
        m_isPopComplete = true;
    }

    public void Pop()
    {
        if (m_isPopComplete)
        {
            m_isPopComplete = false;
            m_animator.Play("Idle");
            m_ownAnimator.Play("Pop");
        }
    }

    public void Pick()
    {
        if(m_canBePicked)
        {
            AudioManager.Instance.PlayBonusPick();
            m_animator.Play("Pick");
            m_isPopComplete = true;
        }
    }

    void OnPickable()
    {
        m_canBePicked = true;
    }

    void OnNotPickable()
    {
        m_canBePicked = false;
    }

    void OnPopComplete()
    {
        m_isPopComplete = true;
        m_animator.Play("None");
    }
}

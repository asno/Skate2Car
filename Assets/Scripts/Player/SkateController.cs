using System.Collections;
using UnityEngine;

public class SkateController : CharacterController2D
{
    [SerializeField]
    private Skateboard[] m_skateboards;

    private Skateboard m_currentSkateboard;
    private IEnumerator m_skateboardIterator;

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(m_skateboards != null, "Unexpected null reference to m_skateboards");
        Debug.Assert(m_skateboards.Length > 0, "Empty container m_skateboards");

        m_skateboardIterator = m_skateboards.GetEnumerator();
        m_skateboardIterator.MoveNext();

        m_currentSkateboard = m_skateboardIterator.Current as Skateboard;
    }

    void Start()
    {
        Reset();
    }

    protected override void PollButtonInput()
    {
        if (Input.GetButtonDown("Fire1"))
            ChangeAnimationState(PlayerAction.Jump);
    }

    public override bool CanMove()
    {
        return !m_currentSkateboard.Collision.IsShaking && !m_currentSkateboard.Collision.IsJumping;
    }

    protected override void KeepRolling()
    {
        ChangeAnimationState(PlayerAction.None);
    }

    protected override void SteerUpwards()
    {
        ChangeAnimationState(PlayerAction.MoveUp);
    }

    protected override void SteerDownards()
    {
        ChangeAnimationState(PlayerAction.MoveDown);
    }

    public override void GetHitByObstacle()
    {
        ChangeAnimationState(PlayerAction.Shake);
    }

    public void Upgrade()
    {
        bool hasMovedNext = m_currentSkateboard.SwitchToNextCharacter();
        if (!hasMovedNext)
        {
            hasMovedNext = m_skateboardIterator.MoveNext();
            if (hasMovedNext)
            {
                m_currentSkateboard.gameObject.SetActive(false);
                m_currentSkateboard = m_skateboardIterator.Current as Skateboard;
                m_currentSkateboard.gameObject.SetActive(true);
            }
        }
    }

    public override void Reset()
    {
        transform.position = m_initialPosition;
        CanControl = true;

        m_skateboardIterator.Reset();
        m_skateboardIterator.MoveNext();
        m_currentSkateboard = m_skateboardIterator.Current as Skateboard;

        foreach (var s in m_skateboards)
        {
            s.gameObject.SetActive(false);
            s.Reset();
        }
        m_skateboards[0].gameObject.SetActive(true);
    }

    public override void ChangeAnimationState(PlayerAction aNewPlayerAction)
    {
        if (m_currentPlayerAction == aNewPlayerAction)
            return;

        if (m_currentSkateboard.Collision.IsShaking)
            return;

        if (m_currentSkateboard.Collision.IsJumping && aNewPlayerAction != PlayerAction.Shake)
            return;

        m_currentPlayerAction = aNewPlayerAction;
        string state;
        switch (aNewPlayerAction)
        {
            case PlayerAction.Shake:
                state = "Shake";
                break;
            case PlayerAction.Jump:
                state = "Jump";
                break;
            case PlayerAction.MoveUp:
                state = "MoveUp";
                break;
            case PlayerAction.MoveDown:
                state = "MoveDown";
                break;
            default:
                state = "Rolling";
                break;
        }
        m_currentSkateboard.PlayAnimation(state);
    }
}

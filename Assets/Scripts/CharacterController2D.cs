using System;
using System.Collections;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 1;
    [SerializeField]
    private Skateboard[] m_skateboards;

    private int m_floorMask;
    private int m_ceilMask;
    private int m_backMask;
    private int m_frontMask;

    private IEnumerator m_currentSkateboard;
    private PlayerAction m_currentPlayerAction;
    private Vector3 m_initialPosition;
    private Rigidbody2D m_rigidbody;
    private PlayerCollision m_playerCollision;
    private bool m_canControl = true;

    public bool CanControl { get => m_canControl; set => m_canControl = value; }
    public float MoveSpeed { get => m_moveSpeed; }

    public bool CanMove()
    {
        return !m_playerCollision.IsShaking && !m_playerCollision.IsJumping;
    }

    internal void KeepRolling()
    {
        ChangeAnimationState(PlayerAction.None);
    }

    internal void GetHitByObstacle()
    {
        ChangeAnimationState(PlayerAction.Shake);
    }

    internal bool SwitchToNextState()
    {
        Skateboard skateboard = m_currentSkateboard.Current as Skateboard;
        bool hasMovedNext = skateboard.SwitchToNextCharacter();
        if (!hasMovedNext)
        {
            hasMovedNext = m_currentSkateboard.MoveNext();
            if (hasMovedNext)
            {
                Skateboard currentSkateboard = m_currentSkateboard.Current as Skateboard;
                skateboard.gameObject.SetActive(false);
                currentSkateboard.gameObject.SetActive(true);
                m_playerCollision = currentSkateboard.GetComponent<PlayerCollision>();
            }
        }
        return hasMovedNext;
    }

    void Awake()
    {
        m_initialPosition = transform.position;
        Debug.Assert(m_moveSpeed > 0, "Zero or negative m_moveSpeed");
        Debug.Assert(m_skateboards != null, "Unexpected null reference to m_skateboards");
        Debug.Assert(m_skateboards.Length > 0, "Empty container m_skateboards");

        m_rigidbody = GetComponent<Rigidbody2D>();
        Debug.Assert(m_rigidbody != null, "Unexpected null reference m_rigidbody");

        m_floorMask = 1 << LayerMask.NameToLayer("Floor");
        m_ceilMask = 1 << LayerMask.NameToLayer("Ceil");
        m_backMask = 1 << LayerMask.NameToLayer("Back");
        m_frontMask = 1 << LayerMask.NameToLayer("Front");

        m_currentSkateboard = m_skateboards.GetEnumerator();
        m_currentSkateboard.MoveNext();

        var skateboard = m_currentSkateboard.Current as Skateboard;
        m_playerCollision = skateboard.GetComponent<PlayerCollision>();
    }

    private void OnEnable()
    {
        Reset();
    }

    void Update()
    {
        if (!m_canControl)
        {
            if (CanMove())
                ChangeAnimationState(PlayerAction.None);

            m_rigidbody.velocity = Vector2.zero;
            return;
        }

        Vector2 translation = Vector2.zero;
        if (CanMove())
        {
            if (Input.GetButtonDown("Fire1"))
                ChangeAnimationState(PlayerAction.Jump);

            var x = Input.GetAxis("Horizontal");
            if (x != 0)
            {
                bool canMoveLeft = x < 0 && !m_rigidbody.IsTouchingLayers(m_backMask);
                bool canMoveRight = x > 0 && !m_rigidbody.IsTouchingLayers(m_frontMask);
                if (canMoveLeft || canMoveRight)
                    translation.x = x;
            }
            var y = Input.GetAxis("Vertical");
            if (y != 0)
            {
                bool canMoveUp = y < 0 && !m_rigidbody.IsTouchingLayers(m_floorMask);
                bool canMoveDown = y > 0 && !m_rigidbody.IsTouchingLayers(m_ceilMask);
                if (canMoveUp || canMoveDown)
                {
                    ChangeAnimationState(canMoveUp ? PlayerAction.MoveUp : PlayerAction.MoveDown);
                    translation.y = y;
                }
                else
                    ChangeAnimationState(PlayerAction.None);
            }
            if (y == 0)
                ChangeAnimationState(PlayerAction.None);
        }
        m_rigidbody.velocity = translation.normalized * m_moveSpeed * Time.deltaTime;
    }

    internal void Reset()
    {
        transform.position = m_initialPosition;
        m_canControl = true;

        m_currentSkateboard.Reset();
        m_currentSkateboard.MoveNext();

        foreach (var s in m_skateboards)
        {
            s.gameObject.SetActive(false);
            s.Reset();
        }
        m_skateboards[0].gameObject.SetActive(true);
        m_playerCollision = m_skateboards[0].GetComponent<PlayerCollision>();
    }

    void ChangeAnimationState(PlayerAction aNewPlayerAction)
    {
        if (m_currentPlayerAction == aNewPlayerAction)
            return;

        if (m_playerCollision.IsShaking)
            return;

        if (m_playerCollision.IsJumping && aNewPlayerAction != PlayerAction.Shake)
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
        Skateboard skateboard = m_currentSkateboard.Current as Skateboard;
        skateboard.PlayAnimation(state);
    }
}

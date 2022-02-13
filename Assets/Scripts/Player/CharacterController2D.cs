using System.Collections;
using UnityEngine;

public abstract class CharacterController2D : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 1;
    [SerializeField]
    protected Vector3 m_initialPosition;

    private int m_floorMask;
    private int m_ceilMask;
    private int m_backMask;
    private int m_frontMask;

    private bool m_canControl = true;

    protected PlayerAction m_currentPlayerAction;
    protected Rigidbody2D m_rigidbody;

    public bool CanControl { get => m_canControl; set => m_canControl = value; }
    public float MoveSpeed { get => m_moveSpeed; }

    protected abstract void PollButtonInput(); 
    public abstract bool CanMove();
    protected abstract void KeepRolling();
    protected abstract void SteerUpwards();
    protected abstract void SteerDownards();

    public virtual void Reset()
    {
    }

    public virtual void ChangeAnimationState(PlayerAction aNewPlayerAction)
    {
    }


    public virtual void GetHitByObstacle()
    {
    }

    protected virtual void Awake()
    {
        Debug.Assert(m_moveSpeed > 0, "Zero or negative m_moveSpeed");

        m_rigidbody = GetComponent<Rigidbody2D>();
        Debug.Assert(m_rigidbody != null, "Unexpected null reference m_rigidbody");

        m_floorMask = 1 << LayerMask.NameToLayer("Floor");
        m_ceilMask = 1 << LayerMask.NameToLayer("Ceil");
        m_backMask = 1 << LayerMask.NameToLayer("Back");
        m_frontMask = 1 << LayerMask.NameToLayer("Front");
    }

    protected virtual void Update()
    {
        if (!m_canControl)
        {
            if (CanMove())
                KeepRolling();

            m_rigidbody.velocity = Vector2.zero;
            return;
        }

        Vector2 translation = Vector2.zero;
        if (CanMove())
        {
            PollButtonInput();

            float x = Input.GetAxis("Horizontal");
            if (x != 0)
            {
                bool canMoveLeft = x < 0 && !m_rigidbody.IsTouchingLayers(m_backMask);
                bool canMoveRight = x > 0 && !m_rigidbody.IsTouchingLayers(m_frontMask);
                if (canMoveLeft || canMoveRight)
                    translation.x = x;
            }
            float y = Input.GetAxis("Vertical");
            if (y != 0)
            {
                bool canMoveUp = y < 0 && !m_rigidbody.IsTouchingLayers(m_floorMask);
                bool canMoveDown = y > 0 && !m_rigidbody.IsTouchingLayers(m_ceilMask);
                translation.y = y;
                if (canMoveUp)
                    SteerUpwards();
                else if (canMoveDown)
                    SteerDownards();
                else
                {
                    translation.y = 0;
                    KeepRolling();
                }
            }
            else
                KeepRolling();
        }
        m_rigidbody.velocity = translation.normalized * m_moveSpeed * Time.deltaTime;
    }
}

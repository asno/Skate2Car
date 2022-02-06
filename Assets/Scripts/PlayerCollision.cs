using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D m_characterController2D;
    private BoxCollider2D m_collider;
    private bool m_isJumping = false;
    public bool IsJumping
    {
        get
        { 
            return m_isJumping;
        }
    }

    private bool m_isShaking = false;
    public bool IsShaking
    {
        get
        {
            return m_isShaking;
        }
    }

    void Start()
    {
        m_collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D aCollision)
    {
        if(aCollision.CompareTag("Obstacle"))
        {
            aCollision.GetComponent<Obstacle>()?.Hit();
            m_characterController2D.GetHitByObstacle();
            m_isShaking = true;
        }
    }

    void OnBecomingAirborne()
    {
        m_collider.enabled = false;
    }

    void OnLanding()
    {
        m_collider.enabled = true;
    }

    void OnJumpStart()
    {
        m_isJumping = true;
    }

    void OnJumpStop()
    {
        m_isJumping = false;
        m_characterController2D.KeepRolling();
    }

    void OnShakingStart()
    {
        m_isShaking = true;
        m_isJumping = false;
        m_collider.enabled = true;
    }

    void OnShakingStop()
    {
        m_isShaking = false;
        m_characterController2D.KeepRolling();
    }
}

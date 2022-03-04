using UnityEngine;

public class CarController : CharacterController2D
{
    private Animator m_animator;

    public override bool CanMove()
    {
        return true;
    }

    public override void ChangeAnimationState(PlayerAction aNewPlayerAction)
    {
        m_animator.Play(aNewPlayerAction.ToString());
    }

    public override void GetHitByObstacle()
    {
        ChangeAnimationState(PlayerAction.Shake);
    }

    protected override void KeepRolling()
    {
    }

    protected override void PollButtonInput()
    {
    }

    protected override void SteerDownards()
    {
    }

    protected override void SteerUpwards()
    {
    }

    public override bool HasPerformedAction()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }

    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        Reset();
    }

    public override void Reset()
    {
        m_rigidbody ??= GetComponent<Rigidbody2D>();
        m_rigidbody.isKinematic = true;
        transform.position = m_initialPosition;
        m_rigidbody.velocity = Vector2.zero;
        m_rigidbody.isKinematic = false;
        CanControl = true;
    }
}

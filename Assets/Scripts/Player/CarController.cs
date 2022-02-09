using UnityEngine;

public class CarController : CharacterController2D
{
    public override bool CanMove()
    {
        return true;
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

    void Start()
    {
        Reset();
    }

    public override void Reset()
    {
        m_rigidbody.isKinematic = true;
        transform.position = m_initialPosition;
        m_rigidbody.velocity = Vector2.zero;
        m_rigidbody.isKinematic = false;
        CanControl = true;
    }
}

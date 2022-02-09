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
        transform.position = m_initialPosition;
        CanControl = true;
    }
}

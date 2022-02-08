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
        transform.position = m_initialPosition;
        CanControl = true;
    }
}

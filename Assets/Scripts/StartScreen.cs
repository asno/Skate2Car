using UnityEngine;

public class StartScreen : Screen
{
    void Update()
    {
        CheckInput();
    }

    public override void CheckInput()
    {
        IsSkipped = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2");
    }
}

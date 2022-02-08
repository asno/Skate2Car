using System;

[Serializable]
public class ScrollingPlan
{
    public string Name;
    public float Speed;
}

[Serializable]
public class ObstacleSetup
{
    public float Time;
    public string Name;
    public float Y;
}

[Serializable]
public class GameSetup
{
    public ScrollingPlan[] @ScrollingPlan;
    public ObstacleSetup[] @ObstacleSetup;
    public float[] CinematicTimer;
}

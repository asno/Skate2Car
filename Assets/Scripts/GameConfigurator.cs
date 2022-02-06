using System;

public class GameConfigurator
{
}

[Serializable]
public class ScrollingSetup
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
    public ScrollingSetup[] ScrollingSetup;
    public ObstacleSetup[] ObstacleSetup;
    public float[] CinematicTimer;
}

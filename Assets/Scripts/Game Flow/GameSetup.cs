using System;

[Serializable]
public class ScrollingSetup
{
    public float Sky;
    public float Backdrop;
    public float Border;
    public float Road;
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
    public ScrollingSetup @ScrollingSetup;
    public ObstacleSetup[] @ObstacleSetup;
    public float[] CinematicTimer;
}

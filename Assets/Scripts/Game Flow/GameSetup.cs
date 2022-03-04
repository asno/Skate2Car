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
public class PropSpawnerSetup
{
    public string Stage;
    public float RandomMin;
    public float RandomMax;
    public PropSetup[] ArrayOfProps;
}

[Serializable]
public class PropSetup
{
    public PropSpawnModel Prop;
    public int Instances;
}

[Serializable]
public class GameSetup
{
    public ScrollingSetup @ScrollingSetup;
    public PropSpawnerSetup[] @PropSpawnerSetup;
    public ObstacleSetup[] @ObstacleSetup;
    public float[] CinematicTimer;
}

[Serializable]
public class PropSpawnModel
{
    public string Prefab; //use Resource.Load to retrieve as an Object type
    public float Weight;
}

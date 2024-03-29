using System;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    private static Score _instance;

    private const int POINT_PER_TENTH_SECOND = 100;
    private const int MIN_POINTS = 0;
    private const int MAX_POINTS = int.MaxValue;
    private readonly Dictionary<Tuple<int, int>, string> SCORE_TO_MSG_MAPPING = new Dictionary<Tuple<int, int>, string>()
    {
        { new Tuple<int, int>(MIN_POINTS, 14999), "VOUS POUVEZ ENCORE\nVOUS AM�LIORER !" },
        { new Tuple<int, int>(15000, 85000), "BIEN JOU�, VOUS �TES\nSUR LA BONNE VOIE !" },
        { new Tuple<int, int>(85001, MAX_POINTS), "VOUS �TES AUSSI � L'AISE SUR UN LONGBOARD QU'AVEC LE NOUVEAU DACIA JOGGER,\nBRAVO!" }
    };

    private int m_points = 0;

    public int Points { get => m_points; }
    public bool AutoIncrement { get; set; }
    public static Score Instance { get => _instance ??= new Score(); }

    private Score()
    {
        Game.Instance.Timer.RegisterCallbackEveryTick(0.1f, GetPointsForOneTenthSecond);
    }

    private void GetPointsForOneTenthSecond()
    {
        if (AutoIncrement)
            m_points += POINT_PER_TENTH_SECOND;
    }

    public void Reset()
    {
        m_points = 0;
    }

    public void Sum(int aPoints)
    {
        m_points = Mathf.Clamp(m_points + aPoints, MIN_POINTS, MAX_POINTS);
    }

    public string GetMessage()
    {
        foreach(var pair in SCORE_TO_MSG_MAPPING)
        {
            if(m_points >= pair.Key.Item1 && m_points <= pair.Key.Item2)
            {
                return pair.Value;
            }
        }
        Debug.LogWarning($"No message registered in the mapping table for score {m_points}");
        return string.Empty;
    }
}

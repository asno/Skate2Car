using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    [SerializeField]
    private Bonus[] m_bonuses;

    private Score m_score;
    private List<Bonus> m_bonusSpawned = new List<Bonus>();

    void Start()
    {
        m_score = Score.Instance;
    }

    public void Reset()
    {
        m_bonusSpawned.ForEach(b => b.Reset());
        m_bonusSpawned.Clear();
    }

    public void SpawnBonus()
    {
        Bonus bonus;
        List<Bonus> bonuses = new List<Bonus>(m_bonuses);
        do
        {
            int index = Random.Range(0, bonuses.Count);
            bonus = bonuses[index];
            bonuses.RemoveAt(index);

        } while (!bonus.IsPopComplete && bonuses.Count > 0);

        m_bonusSpawned.Add(bonus);
        bonus.Pop();
    }

    void Update()
    {
        m_bonusSpawned.RemoveAll(b => b.IsPopComplete);
        if (Input.GetButtonDown("Fire2"))
        {
            Bonus bonus = m_bonusSpawned.Find(b => b.CanBePicked);
            if(bonus != null)
            {
                bonus.Pick();
                m_bonusSpawned.Remove(bonus);
                m_score.Sum(bonus.Points);
            }
        }
    }
}

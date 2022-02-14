using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    private KeyValuePair<float, List<Prop>>[] m_props;
    private KeyValuePair<float, float> m_timeRange;
    private Timer m_timer;
    private List<Prop> m_propsSpawned = new List<Prop>();
    private float m_biggestWeight;
    private bool m_isStarted;
    private bool m_canRandomizeSpawn;

    void Awake()
    {
        m_timer = Game.Instance.Timer;
        m_isStarted = false;
        m_canRandomizeSpawn = false;
    }

    public void Initiliaze(KeyValuePair<float, List<Prop>>[] aProps, KeyValuePair<float, float> aTimeRange)
    {
        m_timeRange = aTimeRange;
        m_props = aProps;
        m_biggestWeight = m_props.Aggregate((i, j) => i.key > j.key ? i : j).key;
    }

    public void Reset()
    {
        foreach(var prop in m_props)
            prop.value.ForEach(p => p.Reset());
    }

    public void Begin()
    {
        m_isStarted = m_canRandomizeSpawn = true;
        Reset();
    }

    public void End()
    {
        m_isStarted = m_canRandomizeSpawn = false;
    }

    private void SpawnProp()
    {
        float weight = Random.Range(0, m_biggestWeight);
        var pool = m_props.Where(p => p.key >= weight).ToArray();
        var props = pool.ElementAt(Random.Range(0, pool.Count())).value;
        bool canSpawn = false;
        Prop prop;
        int index = 0;
        do
        {
            prop = props[index];
            canSpawn = !prop.IsScrolling;
            index++;

        } while (!canSpawn && index >= props.Count);

        if (canSpawn)
        {
            prop.Spawn();
        }
    }

    void Update()
    {
        if (!m_isStarted)
            return;

        if (m_canRandomizeSpawn)
        {
            m_canRandomizeSpawn = false;
            float spawnTime = UnityEngine.Random.Range(m_timeRange.key, m_timeRange.value);
            StartCoroutine(SpawnProcess(spawnTime));
        }
    }

    private IEnumerator SpawnProcess(float aDelay)
    {
        yield return new WaitForSeconds(aDelay);
        while (m_timer.IsPaused)
        {
            yield return null;
        }
        if (enabled)
        {
            SpawnProp();
            m_canRandomizeSpawn = true;
        }
    }
}

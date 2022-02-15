using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    private KeyValuePair<float, List<Prop>>[] m_props;
    private KeyValuePair<float, float> m_timeRange;
    private List<Prop> m_propsSpawned = new List<Prop>();
    private Timer m_timer;
    private float m_biggestWeight;
    private bool m_isPaused;
    private bool m_canRandomizeSpawn;

    void Awake()
    {
        m_timer = Game.Instance.Timer;
        m_isPaused = false;
        m_canRandomizeSpawn = false;
    }

    public void Initiliaze(KeyValuePair<float, List<Prop>>[] aProps, KeyValuePair<float, float> aTimeRange)
    {
        m_timeRange = aTimeRange;
        m_props = aProps;
        m_biggestWeight = m_props.Aggregate((i, j) => i.key > j.key ? i : j).key;

        foreach(var propsList in m_props)
            propsList.value.ForEach(p => p.OnPropReset += ClearOutProp);
    }

    public void Reset()
    {
        StopAllCoroutines();

        foreach(var prop in m_props)
            prop.value.ForEach(p => p.Reset());
    }

    public void End()
    {
        m_canRandomizeSpawn = false;
        m_isPaused = true;
        Reset();
    }

    public void ResumePropsScrolling()
    {
        m_propsSpawned.ForEach(p => p.Resume());
    }

    public void PausePropsScrolling()
    {
        m_propsSpawned.ForEach(p => p.Pause());
    }

    public void PauseSpawning()
    {
        m_canRandomizeSpawn = false;
        m_isPaused = true;
    }

    public void ResumeSpawning()
    {
        m_canRandomizeSpawn = true;
        m_isPaused = false;
    }

    private void ClearOutProp(Prop aProp)
    {
        m_propsSpawned.Remove(aProp);
    }

    private void SpawnProp()
    {
        float weight = Random.Range(0, m_biggestWeight);
        var pool = m_props.Where(p => p.key >= weight).ToArray();
        var props = pool.ElementAt(Random.Range(0, pool.Count())).value;
        bool canSpawn = false;
        Prop prop = null;
        int index = 0;
        while (!canSpawn && index < props.Count)
        {
            prop = props[index];
            canSpawn = !prop.IsScrolling;
            index++;
        } 

        if (canSpawn)
        {
            m_propsSpawned.Add(prop);
            prop.Spawn();
        }
    }

    void Update()
    {
        if (m_isPaused)
            return;

        if (m_canRandomizeSpawn)
        {
            m_canRandomizeSpawn = false;
            float spawnTime = Random.Range(m_timeRange.key, m_timeRange.value);
            StartCoroutine(SpawnProcess(spawnTime));
        }
    }

    private IEnumerator SpawnProcess(float aDelay)
    {
        yield return new WaitForSeconds(aDelay);
        if (enabled)
        {
            SpawnProp();
            m_canRandomizeSpawn = true;
        }
    }
}

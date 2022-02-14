using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decor : MonoBehaviour
{
    [SerializeField]
    private ScrollingBackground m_scrollingSky;
    [SerializeField]
    private ScrollingBackground m_scrollingBackdrop;
    [SerializeField]
    private ScrollingBackground m_scrollingBorder;
    [SerializeField]
    private ScrollingBackground m_scrollingRoad;

    private List<PropSpawner> m_propSpawners = new List<PropSpawner>();

    internal void Set(ScrollingSetup aScrollingSetup)
    {
        m_scrollingSky.Speed = 1.0f / aScrollingSetup.Sky;
        m_scrollingBackdrop.Speed = 1.0f / aScrollingSetup.Backdrop;
        m_scrollingBorder.Speed = 1.0f / aScrollingSetup.Border;
        m_scrollingRoad.Speed = 1.0f / aScrollingSetup.Road;
    }

    public void PauseScrolling()
    {
        m_scrollingSky.IsPaused = true;
        m_scrollingBackdrop.IsPaused = true;
        m_scrollingBorder.IsPaused = true;
        m_scrollingRoad.IsPaused = true;

        foreach (var propSpawner in m_propSpawners)
            propSpawner.End();
    }

    public void ResumeScrolling()
    {
        m_scrollingSky.IsPaused = false;
        m_scrollingBackdrop.IsPaused = false;
        m_scrollingBorder.IsPaused = false;
        m_scrollingRoad.IsPaused = false;

        foreach (var propSpawner in m_propSpawners)
            propSpawner.Begin();
    }

    public void InstantiateSpawner(KeyValuePair<float, float> aTimeRange, KeyValuePair<PropSpawnModel, int>[] aSetup)
    {
        var spawner = new GameObject("spawner").AddComponent<PropSpawner>();
        spawner.transform.parent = transform;

        var initializer = new KeyValuePair<float, List<Prop>>[aSetup.Length];
        for (int i = 0; i < aSetup.Length; i++)
        {
            var propsModel = aSetup[i];
            List<Prop> props = new List<Prop>();
            GameObject prefab = Resources.Load(propsModel.key.Prefab) as GameObject;
            for (int j = 0; j < propsModel.value; j++)
            {
                GameObject instance = Instantiate(prefab);
                instance.transform.parent = spawner.transform;
                props.Add(instance.GetComponent<Prop>());
            }
            initializer[i] = new KeyValuePair<float, List<Prop>>(propsModel.key.Weight, props);
            prefab.SetActive(false);
        }
        spawner.Initiliaze(initializer, aTimeRange);
        m_propSpawners.Add(spawner);
    }
}

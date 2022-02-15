using System;
using System.Collections;
using UnityEngine;

public class DecorManager : MonoBehaviour
{
    private static DecorManager _instance;

    [SerializeField]
    private Decor[] m_decors;

    private IEnumerator m_decorIterator;
    private Decor m_currentDecor;

    public static DecorManager Instance { get => _instance ??= FindObjectOfType<DecorManager>(); }


    public Decor CurrentDecor
    {
        get
        {
            InitIfNull();
            return m_currentDecor;
        }
    }

    void Awake()
    {
        Debug.Assert(m_decors != null, "Unexpected null reference to m_decors");
        Debug.Assert(m_decors.Length > 0, "Empty container m_decors");

        foreach (Decor decor in m_decors)
            decor.gameObject.SetActive(false);

        InitIfNull();
    }

    private void InitIfNull()
    {
        if (m_decorIterator == null)
        {
            m_decorIterator = m_decors.GetEnumerator();
            PickNextDecor();
        }
    }

    public void Reset()
    {
        foreach (Decor decor in m_decors)
            decor.Reset();

        m_decorIterator.Reset();
        PickNextDecor();
        m_currentDecor.ResumeScrolling();
    }

    public void PickNextDecor()
    {
        if (m_decorIterator.MoveNext())
        {
            m_currentDecor?.gameObject.SetActive(false);
            m_currentDecor = m_decorIterator.Current as Decor;
            m_currentDecor.gameObject.SetActive(true);
            m_currentDecor.PauseScrolling();
        }
    }

    internal void Initialize(ScrollingSetup aScrollingSetup, PropSpawnerSetup[] aPropSpawnerSetup = null)
    {
        foreach(Decor decor in m_decors)
        {
            decor.Set(aScrollingSetup);

            if (aPropSpawnerSetup == null)
                continue;

            foreach(var propSpawnerSetup in aPropSpawnerSetup)
            {
                if (decor.tag == propSpawnerSetup.Stage)
                {
                    KeyValuePair<float, float> timeRange = new KeyValuePair<float, float>(propSpawnerSetup.RandomMin, propSpawnerSetup.RandomMax);
                    KeyValuePair<PropSpawnModel, int>[] setup = new KeyValuePair<PropSpawnModel, int>[propSpawnerSetup.ArrayOfProps.Length];
                    for(int i = 0; i < setup.Length; i++)
                    {
                        PropSetup propSetup = propSpawnerSetup.ArrayOfProps[i];
                        setup[i] = new KeyValuePair<PropSpawnModel, int>(propSetup.Prop, propSetup.Instances);
                    }
                    decor.InstantiateSpawner(timeRange, setup);
                }
            }
        }
    }

    private void InitiliazePropSpawners()
    {

    }
}

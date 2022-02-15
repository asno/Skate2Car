using System.Collections.Generic;
using UnityEngine;

public delegate void NotifyProp(Prop aCaller);

public class Prop : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_spriteVariations;
    [SerializeField]
    private List<SpawnPoint> m_spawnPoints;
    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    private SpawnPoint m_spawnPointSelected;
    private float m_scrollingSpeed;
    private bool m_isScrolling;
    private float m_scrollWidth;
    private float m_screenRight;

    public bool IsScrolling => m_isScrolling;
    public event NotifyProp OnPropReset;

    void Awake()
    {
        Debug.Assert(m_spawnPoints != null, "Unexpected null reference to m_spawnPoints");
        Debug.Assert(m_spawnPoints.Count > 0, $"{gameObject.name} m_spawnPoints does not contain spawn point");
        for (int i = m_spawnPoints.Count - 1; i >= 0; i--)
        {
            Transform transform = GameObject.Find(m_spawnPoints[i].m_pathToSceneGameObject)?.transform;
            if (transform != null)
                m_spawnPoints[i].m_point = transform;
            else
                m_spawnPoints.RemoveAt(i);
        }
    }

    void Start()
    {
        m_scrollingSpeed = 1 / (Global.ScrollingSpeed + m_spawnPointSelected.m_scrollingSpeedReduction);
        m_scrollWidth = Global.ScreenWidthInUUnit;
        m_screenRight = Global.ScreenRight;
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
        int index = Random.Range(0, m_spawnPoints.Count);
        m_spawnPointSelected = m_spawnPoints[index];
        gameObject.transform.position = m_spawnPointSelected.m_point.position;
        m_spriteRenderer.sortingOrder = m_spawnPointSelected.m_orderInLayer;
        m_isScrolling = true;
    }

    public void Pause()
    {
        m_isScrolling = false;
    }

    public void Resume()
    {
        m_isScrolling = true;
    }

    public void Reset()
    {
        if (m_spriteVariations != null && m_spriteVariations.Length > 0)
        {
            int index = Random.Range(0, m_spriteVariations.Length);
            m_spriteRenderer.sprite = m_spriteVariations[index];
        }
        gameObject.SetActive(false);
        m_isScrolling = false;

        if(OnPropReset != null)
            OnPropReset(this);
    }

    void Update()
    {
        if (transform.position.x < m_screenRight && !m_spriteRenderer.isVisible)
            Reset();

        if (m_isScrolling)
        {
            var horizontalMove = Time.deltaTime * m_scrollingSpeed * m_scrollWidth;
            var pos = transform.position;
            pos.x -= horizontalMove;
            transform.position = pos;
        }
    }
}

[System.Serializable]
public class SpawnPoint
{
    public string m_pathToSceneGameObject;
    public Transform m_point;
    public int m_orderInLayer;
    public float m_scrollingSpeedReduction;
}

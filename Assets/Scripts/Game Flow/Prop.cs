using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField]
    private List<SpawnPoint> m_spawnPoints;
    [SerializeField]
    private float m_scrollingSpeedReduction;
    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    private float m_scrollingSpeed;
    private bool m_isScrolling;
    private float m_scrollWidth;
    private float m_screenRight;

    public bool IsScrolling => m_isScrolling;

    void Awake()
    {
        Debug.Assert(m_spawnPoints != null, "Unexpected null reference to m_spawnPoints");
        Debug.Assert(m_spawnPoints.Count > 0, "m_spawnPoints does not contain spawn point");
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
        m_scrollingSpeed = Global.ScrollingSpeed - m_scrollingSpeedReduction;
        m_scrollWidth = Global.ScreenWidthInUUnit;
        m_screenRight = Global.ScreenRight;
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
        int index = Random.Range(0, m_spawnPoints.Count);
        gameObject.transform.position = m_spawnPoints[index].m_point.position;
        m_spriteRenderer.sortingOrder = m_spawnPoints[index].m_orderInLayer;
        m_isScrolling = true;
    }

    public void Reset()
    {
        gameObject.SetActive(false);
        m_isScrolling = false;
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
}

using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_spriteVariations;
    [SerializeField]
    private float m_spriteSliceWidthInPixel;
    [SerializeField]
    private bool m_isFlat;
    [SerializeField]
    private int m_penaltyScorePoints;
    [SerializeField]
    private Transform m_playerTransform;

    private Vector3 m_initialPosition;
    private SpriteRenderer m_spriteRenderer;
    private Collider2D m_collider;
    private Animator m_animator;
    private float m_colliderBottomY;
    private float m_scrollWidth;
    private float m_spriteSliceWidthInUUnit;
    private float m_screenLeft;
    private bool m_isScrolling = false;

    void Awake()
    {
        m_initialPosition = transform.position;
        Debug.Assert(m_spriteSliceWidthInPixel > 0, "Zero or negative m_spriteSliceWidthInPixel");
        Debug.Assert(m_playerTransform != null, "Unexpected null reference to m_playerTransform");
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(m_spriteRenderer != null, "Unexpected null reference to m_spriteRenderer");
        m_collider = GetComponent<Collider2D>();
        Debug.Assert(m_collider != null, "Unexpected null reference to m_collider");
        m_animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    void Start()
    {
        m_scrollWidth = Global.ScreenWidthInUUnit;
        m_spriteSliceWidthInUUnit = m_spriteSliceWidthInPixel / 33f;
        m_screenLeft = Global.ScreenLeft;
    }

    internal void Hit()
    {
        if (m_animator != null)
            m_animator.Play("Hit");

        Score.Instance.Sum(m_penaltyScorePoints);
    }

    void Update()
    {
        if (!m_isFlat)
            m_spriteRenderer.sortingOrder = m_playerTransform.position.y < m_colliderBottomY ? 2 : 5;

        if (transform.position.x + m_spriteSliceWidthInUUnit < m_screenLeft)
            Reset();

        if (m_isScrolling)
        {
            var horizontalMove = (Time.deltaTime * (1 / Global.ScrollingSpeed)) * (m_scrollWidth - m_spriteSliceWidthInUUnit);
            var pos = transform.position;
            pos.x -= horizontalMove;
            transform.position = pos;
        }
    }

    public void Reset()
    {
        if (m_spriteVariations != null && m_spriteVariations.Length > 0)
        {
            int index = Random.Range(0, m_spriteVariations.Length);
            m_spriteRenderer.sprite = m_spriteVariations[index];
        }
        if (m_animator != null)
            m_animator.Play("Idle");
        m_isScrolling = false;
        transform.position = m_initialPosition;
        gameObject.SetActive(false);

    }

    public void StartScrolling()
    {
        Debug.Assert(!m_isScrolling, $"{transform.name} is already used for scrolling");
        Vector2 pos2D = transform.position;
        m_colliderBottomY = (pos2D + m_collider.offset).y - m_collider.bounds.extents.y;
        m_isScrolling = true;
    }
}

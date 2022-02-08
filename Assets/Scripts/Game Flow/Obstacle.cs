using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float m_spriteSliceWidthInPixel;
    [SerializeField]
    private bool m_isFlat;
    [SerializeField]
    private Transform m_playerTransform;
    private Vector3 m_initialPosition;
    private SpriteRenderer m_spriteRenderer;
    private Collider2D m_collider;
    private Animator m_animator;
    private float m_colliderBottomY;
    private float m_scrollWidth;
    private float m_spriteSliceWidthInUUnit;
    private float m_screenLeftX;
    private float m_scrollingSpeed;
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
    }

    private void Start()
    {
        m_scrollWidth = (704f - m_spriteSliceWidthInPixel) / 33f;
        m_spriteSliceWidthInUUnit = m_spriteSliceWidthInPixel / 33f;
        m_screenLeftX = 0 - (704f / 2f / 33f);
    }

    internal void Hit()
    {
        if (m_animator != null)
            m_animator.Play("Hit");
    }

    void LateUpdate()
    {
        if (!m_isFlat)
            m_spriteRenderer.sortingOrder = m_playerTransform.position.y < m_colliderBottomY ? 2 : 5;

        if (transform.position.x + m_spriteSliceWidthInUUnit < m_screenLeftX)
            Reset();

        if (m_isScrolling)
        {
            var horizontalMove = Time.deltaTime * m_scrollingSpeed * m_scrollWidth;
            var pos = transform.position;
            pos.x -= horizontalMove;
            transform.position = pos;
        }
    }

    public void Reset()
    {
        if (m_animator != null)
            m_animator.Play("Idle");
        m_isScrolling = false;
        transform.position = m_initialPosition;
    }

    public void StartScrolling(float aScrollingSpeed)
    {
        Debug.Assert(!m_isScrolling, $"{transform.name} is already used for scrolling");
        Vector2 pos2D = transform.position;
        m_colliderBottomY = (pos2D + m_collider.offset).y - m_collider.bounds.extents.y;
        m_scrollingSpeed = aScrollingSpeed;
        m_isScrolling = true;
    }
}

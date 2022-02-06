using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 1;
    private RawImage m_rawImage;

    public float Speed { get => m_speed; set => m_speed = value; }

    private void Awake()
    {
        m_rawImage = GetComponent<RawImage>();
        Debug.Assert(m_rawImage != null, "Unexpected null reference to m_rawImage");
    }

    void FixedUpdate()
    {
        var rect = m_rawImage.uvRect;
        rect.x = (rect.x + Time.deltaTime * m_speed) % 1;
        m_rawImage.uvRect = rect;
    }

}

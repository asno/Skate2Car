using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    [SerializeField]
    protected CharacterController2D m_characterController;

    protected BoxCollider2D m_collider;

    void Start()
    {
        m_collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D aCollision)
    {
        if (aCollision.CompareTag("Obstacle"))
        {
            aCollision.GetComponent<Obstacle>()?.Hit();
        }
    }
}

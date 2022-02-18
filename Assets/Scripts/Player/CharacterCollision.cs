using UnityEngine;
using System.Linq;

public class CharacterCollision : MonoBehaviour
{
    [SerializeField]
    protected CharacterController2D m_characterController;
    [SerializeField]
    private Animator[] m_explosions;

    protected BoxCollider2D m_collider;

    void Start()
    {
        m_collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D aCollision)
    {
        {
            if (aCollision.CompareTag("Obstacle"))
            {
                aCollision.GetComponent<Obstacle>()?.Hit();
                m_characterController.GetHitByObstacle();
            }
        }
    }

    private void OnCollisionEnter(Collision aCollision)
    {
        if (m_explosions != null && m_explosions.Length > 0)
        {
            Animator explosion = m_explosions.FirstOrDefault(e => e.GetCurrentAnimatorStateInfo(0).IsName("None"));
            if (explosion)
                return;

            ContactPoint contact = aCollision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point;
            explosion.transform.position = position;
            explosion.transform.rotation = rotation;
            explosion.Play("Strike");
        }
    }
}

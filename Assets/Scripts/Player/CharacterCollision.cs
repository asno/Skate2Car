using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CharacterCollision : MonoBehaviour
{
    [SerializeField]
    protected CharacterController2D m_characterController;
    [SerializeField]
    private Animator[] m_explosions;

    protected BoxCollider2D m_collider;
    private Queue<Animator> m_animatorQueue;

    void Start()
    {
        m_collider = GetComponent<BoxCollider2D>();
        m_animatorQueue = new Queue<Animator>(m_explosions);
    }

    private void OnTriggerEnter2D(Collider2D aCollision)
    {
        {
            if (aCollision.CompareTag("Obstacle"))
            {
                aCollision.GetComponent<Obstacle>()?.Hit();
                m_characterController.GetHitByObstacle();

                Vector2 collisionPoint = aCollision.ClosestPoint(m_collider.bounds.center);
                Animator explosion = m_animatorQueue.Dequeue();
                explosion.transform.position = collisionPoint;
                explosion.Play("Strike");
                m_animatorQueue.Enqueue(explosion);
            }
        }
    }
}

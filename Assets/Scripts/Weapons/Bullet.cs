using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float applyForce = 200f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float upward = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        var rc = collision.collider.GetComponentInParent<RagdollController>();
        if (rc != null)
        {
            rc.ActivateRagdoll();
            rc.ApplyExplosionForce(applyForce, transform.position, radius, upward);
        }
        Destroy(gameObject);
    }
}

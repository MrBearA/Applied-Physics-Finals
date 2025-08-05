using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private float bulletLifeTime = 5f;
    [SerializeField] private float spawnOffset = 0.5f;
    [SerializeField] private Vector3 bulletEulerOffset = new Vector3(0, -90, 0);

    public void Fire()
    {
        if (!bulletPrefab || !barrelTransform) return;

        Vector3 dir = barrelTransform.forward;
        Vector3 pos = barrelTransform.position + dir * spawnOffset;
        Quaternion rot = barrelTransform.rotation * Quaternion.Euler(bulletEulerOffset);

        var b = Instantiate(bulletPrefab, pos, rot);
        if (b.TryGetComponent<Collider>(out var col))
        {
            foreach (var sc in transform.root.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(col, sc);
        }
        if (b.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = dir * bulletSpeed;

        Destroy(b, bulletLifeTime);
    }
}

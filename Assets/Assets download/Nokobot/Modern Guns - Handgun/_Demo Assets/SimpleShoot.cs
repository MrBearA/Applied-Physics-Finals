using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location References")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [SerializeField, Tooltip("Time before muzzle flash/casing is destroyed")]
    private float destroyTimer = 2f;
    [SerializeField, Tooltip("Bullet speed")]
    private float shotPower = 500f;
    [SerializeField, Tooltip("Casing ejection speed")]
    private float ejectPower = 150f;

    void Awake()
    {
        if (barrelLocation == null)
            barrelLocation = transform;
        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
    }

    /// Call this to perform one shot (muzzle flash, bullet spawn, casing eject).
    public void Fire()
    {
        // 1) Trigger the fire animation
        gunAnimator.SetTrigger("Fire");

        // 2) Muzzle flash
        if (muzzleFlashPrefab)
        {
            var flash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            Destroy(flash, destroyTimer);
        }

        // 3) Spawn bullet
        if (bulletPrefab)
        {
            var bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(barrelLocation.forward * shotPower, ForceMode.Impulse);
        }

        // 4) Eject casing
        if (casingPrefab && casingExitLocation)
        {
            var casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);
            var crb = casing.GetComponent<Rigidbody>();
            if (crb != null)
            {
                crb.AddExplosionForce(
                    Random.Range(ejectPower * 0.7f, ejectPower),
                    casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f,
                    1f
                );
                crb.AddTorque(
                    new Vector3(0, Random.Range(100f,500f), Random.Range(100f,1000f)),
                    ForceMode.Impulse
                );
            }
            Destroy(casing, destroyTimer);
        }
    }
}

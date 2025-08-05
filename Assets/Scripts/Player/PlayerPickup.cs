using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private float maxPickupDistance = 3f;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwForce = 10f;

    private Camera cam;
    private GameObject heldObject;
    private Rigidbody heldRb;
    private Gun heldGun;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickup();
            else
                Drop();
        }

        if (Input.GetKeyDown(KeyCode.R) && heldObject != null)
            Throw();

        if (heldGun != null && Input.GetButtonDown("Fire1"))
            heldGun.Fire();
    }

    private void TryPickup()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (!Physics.Raycast(ray, out RaycastHit hit, maxPickupDistance))
            return;

        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        heldObject = rb.gameObject;
        heldRb = rb;
        heldRb.isKinematic = true;
        heldObject.transform.SetParent(holdPoint, false);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        heldGun = heldObject.GetComponent<Gun>();
    }

    private void Drop()
    {
        // unequip gun if held
        if (heldGun != null)
            heldGun = null;

        heldRb.isKinematic = false;
        heldObject.transform.SetParent(null);
        heldObject = null;
        heldRb = null;
    }

    private void Throw()
    {
        if (heldGun != null)
            heldGun = null;

        heldRb.isKinematic = false;
        heldObject.transform.SetParent(null);
        heldRb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

        // arm grenade if this object has one
        Grenade grenade = heldObject.GetComponent<Grenade>();
        if (grenade != null)
            grenade.Arm();

        heldObject = null;
        heldRb = null;
    }
}

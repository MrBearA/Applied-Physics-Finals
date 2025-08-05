using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable] public class Entry { public KeyCode key; public GameObject prefab; }

    [SerializeField] private List<Entry> spawnList;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private float spawnDistance = 2f;

    private Camera cam;

    private void Start() => cam = Camera.main;

    private void Update()
    {
        foreach (var e in spawnList)
            if (e.prefab && Input.GetKeyDown(e.key))
                Spawn(e.prefab);
    }

    private void Spawn(GameObject prefab)
    {
        Vector3 pos = spawnArea != null
            ? spawnArea.position
            : cam.transform.position + cam.transform.forward * spawnDistance;
        Quaternion rot = spawnArea != null
            ? spawnArea.rotation
            : Quaternion.LookRotation(cam.transform.forward);

        var go = Instantiate(prefab, pos, rot);
        if (!go.GetComponent<Collider>()) go.AddComponent<BoxCollider>();
        if (!go.GetComponent<Rigidbody>())
        {
            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 1f;
        }
        go.tag = "Pickupable";
    }
}

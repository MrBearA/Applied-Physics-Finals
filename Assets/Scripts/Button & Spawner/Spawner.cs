using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("Drag your ragdoll prefab here")]
    [SerializeField] private GameObject ragdollPrefab;

    [Tooltip("Optional: control exactly where it appears")]
    [SerializeField] private Transform spawnPoint;

    private void OnMouseDown()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
        Instantiate(ragdollPrefab, pos, rot);
    }
}

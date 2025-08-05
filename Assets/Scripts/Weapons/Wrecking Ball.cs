using UnityEngine;

public class WreckingBall : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) input += 1f;
        if (Input.GetKey(KeyCode.RightArrow)) input -= 1f;
        transform.Translate(Vector3.right * input * speed * Time.deltaTime, Space.World);
    }
}

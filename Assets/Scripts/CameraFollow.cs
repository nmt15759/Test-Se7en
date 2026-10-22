using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;                                
    [SerializeField] Vector3 offset = new Vector3(0f, 12f, 0f);
    [SerializeField] float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 goal = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, goal, followSpeed * Time.deltaTime);
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
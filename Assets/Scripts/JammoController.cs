using UnityEngine;

public class JammoController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnSpeed = 12f;
    [SerializeField] Animator animator;
    [SerializeField] Transform cam;          
    [SerializeField] Collider fieldBounds;   

    CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Đọc phím WASD
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

    
        Vector3 right = cam.right;
        Vector3 up = cam.up;
        right.y = 0;
        up.y = 0;
        Vector3 dir = right.normalized * h + up.normalized * v;
        dir = Vector3.ClampMagnitude(dir, 1f);

     
        if (dir != Vector3.zero)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
        animator.SetFloat("Speed", dir.magnitude);

     
        if (fieldBounds != null)
        {
            Bounds b = fieldBounds.bounds;
            Vector3 next = transform.position + dir * moveSpeed * Time.deltaTime;
            if (next.x < b.min.x || next.x > b.max.x) dir.x = 0;
            if (next.z < b.min.z || next.z > b.max.z) dir.z = 0;
        }
        cc.SimpleMove(dir * moveSpeed);
    }
}
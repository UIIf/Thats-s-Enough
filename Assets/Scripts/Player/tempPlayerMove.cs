using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempPlayerMove : MonoBehaviour
{
    [SerializeField] private float speed;

    private Transform _transform;
    private Rigidbody rb;
    private Animator animator;
    
    private Vector3 moveDir = Vector3.zero;
    
    void Awake()
    {
        _transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");

        if (moveDir == Vector3.zero)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);

        }

    }
    void FixedUpdate()
    {
        rb.MovePosition(transform.position + moveDir * speed * Time.deltaTime);
    }

}

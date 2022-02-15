using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempPlayerMove : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject hand;
    private CharacterController _controller;
    private Transform _transform;
    private Rigidbody rb;
    private CamScript camScript;
    private Animator animator;
    
    private Vector3 moveDir = Vector3.zero;
    
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        camScript = GetComponent<CamScript>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        _controller.Move(moveDir * speed * Time.deltaTime);
        if (moveDir == Vector3.zero)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

    }
    void FixedUpdate()
    {
        //КОСТЫЫЫЫЫЛЬ
        if (Mathf.Round(_transform.position.y * 100) > 89 || Mathf.Round(_transform.position.y * 100) < 83)
            _transform.position = new Vector3(_transform.position.x, 0.85f, _transform.position.z);      
        //зато не летаем теперь С:
    }

}

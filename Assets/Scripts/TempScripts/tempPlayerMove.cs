using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempPlayerMove : MonoBehaviour
{
    [SerializeField] private float speed;

    private CharacterController _controller;
    private Transform _transform;
    private Rigidbody rb;

    
    private Vector3 moveDir = Vector3.zero;
    


    //Чтобы не пересоздавать
    
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        _controller.Move(moveDir * speed * Time.deltaTime);
        //transform.LookAt(mousePose);
    }
    void FixedUpdate()
    {
        


        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            print("text");
        }
        
    }
}

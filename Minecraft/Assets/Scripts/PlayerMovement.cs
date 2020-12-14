using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] private float moveSpeed = 10f;

    private float speed;
    private float verticalSpeed = 0;
    [SerializeField] float jumpSpeed = 100;

    //physical parameters
    [SerializeField] private const float gravity = -8f;
    [SerializeField] private float friction = 1f;
    private bool airJump = false;
    [SerializeField] private Camera cam;

    private void Start()
    {
        speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * y;

        if (controller.isGrounded)
        {
            airJump = false;
            verticalSpeed = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                verticalSpeed += jumpSpeed;
            }
        }

        verticalSpeed += (gravity + friction) * Time.deltaTime;
        move.y = verticalSpeed;
        controller.Move(move * speed * Time.deltaTime);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    private const float PlayerHeight = 2f;
    
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    private float _movementSpeed;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    private bool _isGrounded = true;
    private bool _isJumping;
    private LayerMask _groundLayer;
    private float _groundCheckRadius;
    
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    
    private const float MovementMultiplier = 10f;

    private float _horizontalMovement;
    private float _verticalMovement;
    
    private Vector3 _movementDirection;
    

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        _groundLayer = LayerMask.GetMask("Ground");
        _groundCheckRadius = (PlayerHeight / 2) + 0.1f;
    }

    private void Update()
    {
        MovementInputControls();
        JumpCheck();
    }

    private void FixedUpdate()
    {
        // Only check on ground if not trying to jump
        if (!_isJumping) _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckRadius);
        else Jump();
        MovePlayer();
    }

    private void MovementInputControls()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");
        
        // Move in direction of player
        _movementDirection = (transform.right * _horizontalMovement + transform.forward * _verticalMovement).normalized;

        _movementSpeed = Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed;
    }

    private void MovePlayer()
    {
        rb.AddForce(_movementDirection * (_movementSpeed * MovementMultiplier), ForceMode.Acceleration);
    }

    private void JumpCheck()
    {
        if (!Input.GetKeyDown(jumpKey) || !_isGrounded) return;
        // Jump is called from fixedUpdate
        _isJumping = true;
        // Will use fast fall calculations in Jump
        _isGrounded = false;
        // Add force for 1 frame
        rb.velocity = Vector3.up * jumpForce;
    }

    private void Jump()
    {
        
        //  Falling
        if (rb.velocity.y < 0)
        {   
            rb.velocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
            // Only need to check if touching the ground when falling 
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckRadius);
        } 
        // Rising
        else if (rb.velocity.y > 0 && !_isGrounded) rb.velocity += Vector3.up * (Physics.gravity.y * Time.fixedDeltaTime);
        
        // On ground again 
        if (_isGrounded) _isJumping = false;
    }
}

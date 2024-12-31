using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Player3DController : MonoBehaviour
{
    private const float PlayerHeight = 2f;
    
    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;
    private float _health;
    public bool IsAlive { get; set; }
    public bool IsTakingDamage { get; set; }

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    private float _movementSpeed;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    private const float AirMultiplier = 1.1f;
    private bool _isGrounded = true;
    private bool _isJumping;
    private LayerMask _groundLayer;
    private float _groundCheckRadius;
    
    [Header("WallRunning")]
    [SerializeField] private float wallCheckRadius = 0.5f;
    [SerializeField] private float wallRunFOV = 60f;
    [SerializeField] private float wallRunTilt = 45f;
    [SerializeField] private float wallRunForce = 3f;
    [SerializeField] private float tiltTimer = 20;
    [SerializeField] private float gravityMultiplier = 1.5f;
    [SerializeField] private float wallJumpMultiplier = 1.5f; 
    [SerializeField] private float wallUpForce = 10f;
    [SerializeField] private float wallSideForce = 12f;
    private bool _isOnWall, _wallLeft, _wallRight, _wallJump;
    private RaycastHit _leftHit, _rightHit;
    private Vector3 _wallNormal, _wallForward, _jumpDir;
    
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CameraController cam;
    [SerializeField] private GravityController gravityController;
    public TextMeshProUGUI healthDisplay;
    
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

        _health = maxHealth;
        if (healthDisplay) healthDisplay.SetText(_health + "/" + maxHealth);

        Physics.queriesHitTriggers = false;
    }

    private void Start()
    {
        if (!cam) cam = FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        MovementInputControls();
        JumpCheck();
        if (!_wallJump) WallCheck();
        if (_isOnWall && !_isGrounded) StartWallRun();
        else cam.IsWallRunning = false;
        
        if (IsTakingDamage) TakeDamage(1);
        if (healthDisplay) healthDisplay.SetText(_health + "/" + maxHealth);
    }

    private void FixedUpdate()
    {
        // Only check on ground if not trying to jump
        if (!_isJumping) _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckRadius);
        else Jump();
        MovePlayer();
        
        if (_isOnWall && !_isGrounded) WallRunMovement();
        else gravityController.enabled = false;
    }
    
    private void TakeDamage(float dmg)
    {
        _health -= dmg;
        if (_health <= 0) IsAlive = false;
        IsTakingDamage = false;
    }

    private void MovementInputControls()
    {
        _horizontalMovement = _isOnWall ? 0 : Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");

        Vector3 forward = _isOnWall ?
            (_wallLeft ? _wallForward : -_wallForward) * wallRunForce
            : transform.forward;
        
        // Move in direction of player
        _movementDirection = (transform.right * _horizontalMovement + forward * _verticalMovement).normalized;

        _movementSpeed = Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed;
    }

    private void MovePlayer()
    {
        if (_isGrounded) rb.AddForce(_movementDirection * (_movementSpeed * MovementMultiplier), ForceMode.Acceleration);
        else rb.AddForce(_movementDirection * (_movementSpeed * MovementMultiplier * AirMultiplier), ForceMode.Acceleration);
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

    private void WallCheck()
    {
        
        _wallLeft = Physics.Raycast(transform.position, -transform.right, out _leftHit, wallCheckRadius);
        _wallRight = Physics.Raycast(transform.position, transform.right, out _rightHit, wallCheckRadius);
        
        if (_wallLeft || _wallRight) _isOnWall = true;
        else _isOnWall = false;
    }

    private void StartWallRun()
    {
        // enable cam FX
        cam.IsWallRunning = true;
        cam.WallRunfov = wallRunFOV;
        cam.TiltDT = tiltTimer;
        if (_wallLeft) cam.CamTilt = -wallRunTilt;
        else if (_wallRight) cam.CamTilt = wallRunTilt;
        
        if (Input.GetKeyDown(jumpKey)) _wallJump = true;
        
    }

    private void WallRunMovement()
    {
        // movement
        gravityController.enabled = true;
        gravityController.GravityScale = gravityMultiplier;
        
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        _wallNormal = _wallLeft ? _leftHit.normal : _rightHit.normal;
        _wallForward = Vector3.Cross(_wallNormal, transform.up);
        
        //rb.AddForce(-_wallForward * wallRunForce, ForceMode.Force);

        // Jumping off wall
        if (!_wallJump) return;
        gravityController.enabled = false;
        _jumpDir = _wallNormal * wallSideForce + Vector3.up * wallUpForce;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(_jumpDir * wallJumpMultiplier, ForceMode.Impulse);
        print(_jumpDir);
        _wallJump = false;
    }
    
}

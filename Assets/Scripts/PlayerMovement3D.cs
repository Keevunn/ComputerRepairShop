using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    private const float MovementMultiplier = 10f;

    private float _horizontalMovement;
    private float _verticalMovement;
    
    private Vector3 _movementDirection;

    private Rigidbody _rb;

    [SerializeField] private float rbDrag = 1f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        SetDrag();
    }

    private void Update()
    {
        InputControls();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void InputControls()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");
        
        // Move in direction of player
        _movementDirection = (transform.right * _horizontalMovement + transform.forward * _verticalMovement).normalized;
    }

    private void MovePlayer()
    {
        _rb.AddForce(_movementDirection * (movementSpeed * MovementMultiplier), ForceMode.Acceleration);
    }

    private void SetDrag()
    {
        _rb.drag = rbDrag;
    }
}

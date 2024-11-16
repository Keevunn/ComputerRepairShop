using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 10.0f;
    [SerializeField] private Transform orientation;
    private float _speed = 5.0f;
    private float _translation;
    private float _strafe;
    private Vector3 _newPos;
    private Vector3 _input;
    private Vector3 _dir;
    private Rigidbody _rb;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(_strafe,0, _translation);
        // Cursor.lockState = (Input.GetKeyDown("escape")) ? CursorLockMode.None : CursorLockMode.Locked;
        //_input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _input = (orientation.forward * Input.GetAxis("Vertical") + orientation.right * Input.GetAxis("Horizontal")).normalized;
    }

    private void FixedUpdate()
    {
        _speed = Input.GetKey(KeyCode.LeftShift) ? Mathf.Lerp(_speed, sprintSpeed, 10f * Time.fixedDeltaTime) : 
            Mathf.Lerp(_speed, walkSpeed, 10f * Time.fixedDeltaTime);
        
        
        //_translation = Input.GetAxis("Vertical") * _speed * Time.fixedDeltaTime;
        //_strafe = Input.GetAxis("Horizontal") * _speed * Time.fixedDeltaTime;
        //_rb.MoveRotation(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0));
        //_rb.MovePosition(transform.position + _input * (_speed * Time.fixedDeltaTime));
        _rb.AddForce(_input * _speed, ForceMode.Acceleration);
        Debug.Log(_input * _speed);
    } 
}

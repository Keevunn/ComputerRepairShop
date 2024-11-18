using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2.0f;
    private const float SensMultiplier = 100f;
    
    [SerializeField] private Transform cam;
    
    private float _camYRotation;
    private float _camXRotation;
    
    private float _rotateX;
    private float _rotateY;
    
    private bool _lockedCursor = true;
    private KeyCode _lockCursorKey = KeyCode.LeftControl;
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        if (!cam) cam = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        CursorControl();
        
        PlayerInput();
        // Rotate cam up and down
        cam.localRotation = Quaternion.Euler(Vector3.right * _camYRotation);
        // Rotate player left and right
        transform.rotation = Quaternion.Euler(Vector3.up * _camXRotation);
    }

    private void CursorControl()
    {
        if (!Input.GetKeyDown(_lockCursorKey)) return;
        if (_lockedCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _lockedCursor = false;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            transform.localEulerAngles = Vector3.zero;
            _lockedCursor = true;
        }
    }

    private void PlayerInput()
    {
        if (!_lockedCursor) return;
        
        _rotateX = Input.GetAxis("Mouse X") * sensitivity * SensMultiplier * Time.deltaTime;
        _rotateY = Input.GetAxis("Mouse Y") * sensitivity * SensMultiplier * Time.deltaTime;
        
        _camYRotation -= _rotateY;
        _camYRotation = Mathf.Clamp(_camYRotation, -90f, 90f);
            
        _camXRotation += _rotateX;
        
    }
}

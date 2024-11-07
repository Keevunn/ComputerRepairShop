using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2.0f;
    [SerializeField] private Transform player;
    private float _rotateX;
    private float _rotateY;
    private float _camYRotation = 0f;
    private bool _lockedCursor = true;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        transform.localEulerAngles = Vector3.zero;
        Debug.Log(transform.localEulerAngles);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
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
    }

    private void FixedUpdate()
    {
        if (_lockedCursor)
        {
            _rotateX = Input.GetAxis("Mouse X") * sensitivity;
            _rotateY = Input.GetAxis("Mouse Y") * sensitivity;
        
            _camYRotation -= _rotateY;
            _camYRotation = Mathf.Clamp(_camYRotation, -90f, 90f);
            transform.localEulerAngles = Vector3.right * _camYRotation;
        
            player.Rotate(Vector3.up * _rotateX);
        }
    }
}

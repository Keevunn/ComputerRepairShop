using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2.0f;
    private const float SensMultiplier = 100f;
    
    [SerializeField] private GameObject cam;
    private Camera _camCamera;
    [SerializeField] private float fov = 80f;
    
    
    public float CamTilt { private get; set; } // TODO: watch video 
    //private float _tilt = 0f;
    public float TiltDT { private get; set; }
    public bool IsWallRunning { private get; set; }
    public float WallRunfov { private get; set; }

    private float _camXRotation;
    private float _camYRotation;
    private float _camZRotation;
    
    private float _rotateX;
    private float _rotateY;
    
    private bool _lockedCursor = true;
    [SerializeField] private KeyCode lockCursorKey = KeyCode.LeftControl;
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        if (!cam) cam = transform.Find("Camera").gameObject;
        _camCamera = cam.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        CursorControl();
        
        PlayerInput();
        // Rotate cam up and down
        cam.transform.localRotation = Quaternion.Euler(_camYRotation, 0, _camZRotation);
        // Rotate player left and right
        transform.rotation = Quaternion.Euler(Vector3.up * _camXRotation);
        
        if (IsWallRunning) WallRunFX();
        else ResetCamFX();
        
    }

    private void CursorControl()
    {
        if (!Input.GetKeyDown(lockCursorKey)) return;
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

    private void WallRunFX()
    {
        // FPS camera is main camera...for now...
        _camCamera.fieldOfView = Mathf.Lerp(_camCamera.fieldOfView, WallRunfov, TiltDT*Time.deltaTime);
        _camZRotation = Mathf.Lerp(_camZRotation, CamTilt, TiltDT*Time.deltaTime);
    }

    private void ResetCamFX()
    {
        _camCamera.fieldOfView = Mathf.Lerp(_camCamera.fieldOfView, fov, TiltDT*Time.deltaTime);
        _camZRotation = Mathf.Lerp(_camZRotation, 0, TiltDT*Time.deltaTime);
    }
}

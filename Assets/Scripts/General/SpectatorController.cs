using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorController : MonoBehaviour
{
    private bool _forward;
    private bool _backward;
    private bool _left;
    private bool _right;
    private bool _jump;
    private bool _crouch;
    private bool _sprint;

    private float _yaw;
    private float _pitch;

    private Transform target;
    private float speedLerp = 0.5f;
    private float speed = 2.5f;
    private float sprintSpeed = 5f;

    private void Awake()
    {
        target = new GameObject("CameraTarget").transform;
    }

    //private void OnEnable()
    //{
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //}
    //
    //private void OnDisable()
    //{
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //}

    void PollKeys()
    {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);
        _jump = Input.GetKey(KeyCode.Space);
        _crouch = Input.GetKey(KeyCode.LeftControl);
        _sprint = Input.GetKey(KeyCode.LeftControl);
        _yaw += Input.GetAxisRaw("Mouse X");
        _yaw %= 360f;
        _pitch -= Input.GetAxisRaw("Mouse Y");
    }



    private void Update()
    {
        PollKeys();
        Vector3 movingDir = Vector3.zero;

        if (_forward ^ _backward)
        {
            movingDir += _forward ? target.forward : -target.forward;
        }

        if (_left ^ _right)
        {
            movingDir += _right ? target.right : -target.right;
        }

        if (_jump ^ _crouch)
        {
            movingDir += _crouch ? -target.up : target.up;
        }

        movingDir = Vector3.Normalize(movingDir);
        target.position += ((_sprint) ? movingDir * sprintSpeed : movingDir * speed) * BoltNetwork.FrameDeltaTime;
        target.rotation = Quaternion.Euler(target.rotation.x + _pitch, target.rotation.y + _yaw, 0f);

        transform.position = Vector3.Lerp(transform.position, target.position, speedLerp);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, speedLerp);
    }
}

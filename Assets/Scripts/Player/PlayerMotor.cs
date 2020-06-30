using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera _cam;
    private Rigidbody _rb;
    private NetworkRigidbody _networkBody;
    private bool _jumpPressed=false;
    /*[SerializeField]
    private GroundDetector _groundDetector;*/

    private int _collisionCount = 0;
    private bool _isGrounded = true;

    

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _networkBody = GetComponent<NetworkRigidbody>();
    }


    public State Move(bool forward, bool backward, bool left, bool right, bool jump, float yaw, float pitch)
    {
        Vector3 movingDir = Vector3.zero;
        if (forward ^ backward)
        {
            movingDir += forward ? transform.forward : -transform.forward;
        }
        if (left ^ right)
        {
            movingDir += right ? transform.right : -transform.right;
        }
        movingDir.Normalize();

        if (jump)
        {
            if (_jumpPressed == false && _isGrounded)
            {
                _isGrounded = false;
                _jumpPressed = true;
                _networkBody.moveForce += Vector3.up * 10;
            }
        }
        else
        {
            if (_jumpPressed)
                _jumpPressed = false;
        }

        _networkBody.moveForce = new Vector3(movingDir.x *5, _networkBody.moveForce.y, movingDir.z*5);
        //_rb.velocity = movingDir*10f;
        _cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        State _stateMotor = new State();
        _stateMotor.position = transform.position;
        _stateMotor.rotation = yaw;
        
        return _stateMotor;
    }

    public void SetState(Vector3 position, float rotation)
    {
        if (Mathf.Abs(rotation - transform.rotation.y) > 5f)
            transform.rotation = Quaternion.Euler(0, rotation, 0);

        if (Vector3.Distance(transform.position, position) > 0.1f)
        {
            transform.position += (position - transform.position) * BoltNetwork.FrameDeltaTime * 5f;
        }
    }

    public struct State
    {
        public Vector3 position;
        public float rotation;
    }

    public void DisableCamera()
    {
        _cam.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        _isGrounded = true;
    }

}

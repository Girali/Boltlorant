using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera _cam = null;
    private NetworkRigidbody _networkBody;
    private bool _jumpPressed=false;
    private float _speed = 5.0f;
    private float _jumpForce = 7.5f;
    private bool _isGrounded = true;
    [SerializeField]
    private Transform _groundTarget = null;
    [SerializeField]
    private Weapon weapon;

    void Awake()
    {
        _networkBody = GetComponent<NetworkRigidbody>();
    }

    public void Init()
    {
        weapon.Init(GetComponent<BoltEntity>(), _cam.transform);
    }

    private void FixedUpdate()
    {
        _isGrounded = false;
        Collider[] _colliders = Physics.OverlapSphere(_groundTarget.position, 0.475f);
        foreach (Collider col in _colliders)
        {
            if (col.gameObject.GetHashCode() != gameObject.GetHashCode())
            {
                _isGrounded = true;
                if (_networkBody.MoveVelocity.y < 0)
                    _networkBody.MoveVelocity = Vector3.Scale(_networkBody.MoveVelocity, new Vector3(1, 0, 1));
            }
        }
    }

    public State Move(bool forward, bool backward, bool left, bool right, bool jump, bool fire, bool aiming, bool reload, float yaw, float pitch)
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
        movingDir *= _speed;

        if (jump)
        {
            if (_jumpPressed == false && _isGrounded)
            {
                _isGrounded = false;
                _jumpPressed = true;
                _networkBody.MoveVelocity += Vector3.up * _jumpForce;
            }
        }
        else
        {
            if (_jumpPressed)
                _jumpPressed = false;
        }

        _networkBody.MoveVelocity = new Vector3(movingDir.x, _networkBody.MoveVelocity.y, movingDir.z);
        _cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        weapon.ExecuteCommand(fire, aiming, reload);

        State stateMotor = new State();
        stateMotor.position = transform.position;
        stateMotor.rotation = yaw;
        
        return stateMotor;
    }

    public void FireEffect()
    {
        weapon.FireEffect();
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
        _cam.enabled = false;
    }
}

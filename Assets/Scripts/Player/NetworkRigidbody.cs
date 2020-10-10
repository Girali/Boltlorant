using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRigidbody : Bolt.EntityEventListener<IPhysicState>
{
    private Vector3 _moveVelocity;
    [SerializeField]
    private Transform _renderTransform;
    private Rigidbody _rb;
    [SerializeField]
    private float _gravityForce = 1f;
    private bool _useGravity = true;
    public Vector3 MoveVelocity
    {
        set
        {
            if (entity.IsControllerOrOwner)
            {
                _moveVelocity = value;
            }
        }

        get
        {
            return _moveVelocity;
        }
    }

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _rb.isKinematic = false;
    }

    private void OnDisable()
    {
        _rb.isKinematic = true;
    }

    public override void Attached()
    {
        if(_renderTransform)
            state.SetTransforms(state.Transform, transform, _renderTransform);
        else
            state.SetTransforms(state.Transform, transform);
    }

    public float GravityForce
    {
        get
        {
            return Physics.gravity.y * _gravityForce * BoltNetwork.FrameDeltaTime;
        }
    }

    public bool UseGravity { get => _useGravity; set => _useGravity = value; }

    private void FixedUpdate()
    {
        if (entity.IsAttached)
        {
            if (entity.IsControllerOrOwner)
            {
                float g = _moveVelocity.y;

                if(_useGravity)
                {
                    if (_moveVelocity.y < 0f)
                        g += 1.5f * GravityForce;
                    else if (_moveVelocity.y > 0f)
                        g += 1f * GravityForce;
                    else
                        g = _rb.velocity.y;
                }

                _moveVelocity = new Vector3(_moveVelocity.x, g, _moveVelocity.z);

                _rb.velocity = _moveVelocity;
            }
        }
    }
}
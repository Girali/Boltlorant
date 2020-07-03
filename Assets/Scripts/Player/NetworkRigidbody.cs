using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRigidbody : Bolt.EntityEventListener<IPhysicState>
{
    private Vector3 _moveVelocity;
    private Rigidbody _rb;
    private float _gravityForce = 1f;

    private bool _canWriteMoveVelocity = true;


    public Vector3 MoveVelocity
    {
        set
        {
            if (_canWriteMoveVelocity && ( entity.IsOwner || entity.HasControl))
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

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
    }

    private void FixedUpdate()
    {
        float g = _moveVelocity.y;

        if (_moveVelocity.y < 0f)
            g += 1.5f * Physics.gravity.y * _gravityForce * BoltNetwork.FrameDeltaTime;
        else if (_moveVelocity.y > 0f)
            g += 1f * Physics.gravity.y * _gravityForce * BoltNetwork.FrameDeltaTime;
        else
            g = _rb.velocity.y;

        _moveVelocity = new Vector3(_moveVelocity.x, g, _moveVelocity.z);

        _rb.velocity = _moveVelocity;
    }

    public void LockMoveVelocity(Vector3 moveVelocity)
    {
        MoveVelocity = moveVelocity;
        _canWriteMoveVelocity = false;
    }

    public void UnlockMoveVelocity()
    {
        _canWriteMoveVelocity = true;
    }
}
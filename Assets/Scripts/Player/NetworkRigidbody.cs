﻿using System.Collections;
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

    public override void Attached()
    {
        //if(renderTransform)
        //    state.SetTransforms(state.Transform, transform, renderTransform);
        //else
        state.SetTransforms(state.Transform, transform);
    }

    private void FixedUpdate()
    {
        if (entity.IsAttached)
        {
            if (entity.IsControllerOrOwner)
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
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRigidbody : Bolt.EntityEventListener<IPhysicState>
{
    private Vector3 _velocityMove;
    private Vector3 _velocityForce;
    private Rigidbody _rb;

    private float _drag = 0.01f;
    private float _gravityForce = 1f;


    public Vector3 moveForce
    {
        set
        {
            if (entity.IsOwner || entity.HasControl)
            {
                _velocityMove = value;
            }
        }

        get
        {
            return _velocityMove;
        }
    }
    public Vector3 force
    {
        set
        {
            if (entity.IsOwner || entity.HasControl)
            {
                _velocityForce = value;
            }
        }

        get
        {
            return _velocityForce;
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

    public void AddForce(BoltEntity launcher, Vector3 force, ForceMode mode)
    {
        if (entity.HasControl || entity.IsOwner && launcher.NetworkId == entity.NetworkId)
        {
            _velocityForce += force;
        }
        else if (entity.IsOwner)
        {
            _velocityForce += force;
            AddForceEvent addForce = AddForceEvent.Create(entity, Bolt.EntityTargets.OnlyController);
            addForce.Force = force;
            addForce.Mode = (int)mode;
            addForce.Send();
        }
    }

    public override void OnEvent(AddForceEvent evnt)
    {
        _rb.AddForce(evnt.Force, (ForceMode)evnt.Mode);
    }

    private void FixedUpdate()
    {
        _velocityForce = Vector3.Lerp(_velocityForce, Vector3.zero, _drag);
        float g = _velocityMove.y;
        if (_velocityMove.y < 0f)
            g += 1.5f * Physics.gravity.y * _gravityForce * BoltNetwork.FrameDeltaTime;
        else if (_velocityMove.y > 0f)
            g += 1f * Physics.gravity.y * _gravityForce * BoltNetwork.FrameDeltaTime;


        _velocityMove = new Vector3(_velocityMove.x, g, _velocityMove.z);
        _rb.velocity = _velocityMove + _velocityForce;

    }

}

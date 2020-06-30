using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRigidbody : Bolt.EntityBehaviour<IPhysicState>
{
    private Vector3 _velocityMove;
    private Vector3 _velocityForce;

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

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
    }


}

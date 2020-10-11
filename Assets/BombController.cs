using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class BombController : EntityBehaviour<IBombState>
{
    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private bool _isGrounded;
    [SerializeField]
    private GameObject parent;

    Collider[] _colliders;

    private void FixedUpdate()
    {
        _isGrounded = false;
        _colliders = Physics.OverlapSphere(transform.position, transform.localScale.x);
        foreach(Collider col in _colliders)
        {
            if (col.GetHashCode() != parent.GetHashCode())
                _isGrounded = true;
        }
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }
}

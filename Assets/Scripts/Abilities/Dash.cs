using System.Collections;
using UnityEngine;

public class Dash : Ability
{
    private NetworkRigidbody _networkBody=null;
    [SerializeField]
    private Transform _cam = null;
    [SerializeField]
    private float _dashForce = 20;
    [SerializeField]
    private float _dashDuration = 1f;

    public void Awake()
    {
        _networkBody = GetComponent<NetworkRigidbody>();
    }

    private void FixedUpdate()
    {
        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame)
        {
            _timer = BoltNetwork.ServerFrame;
            _Dash();
        }
    }

    private void _Dash()
    {
        StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {

        _networkBody.LockMoveVelocity(_cam.forward * _dashForce);
        yield return new WaitForSeconds(_dashDuration);
        _networkBody.UnlockMoveVelocity();
    }


}

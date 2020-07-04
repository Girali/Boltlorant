using System.Collections;
using UnityEngine;

public class Dash : Ability
{
    private NetworkRigidbody _networkBody = null;
    [SerializeField]
    private Transform _cam = null;
    [SerializeField]
    private float _dashForce = 20f;
    [SerializeField]
    private float _dashDuration = 1f;
    private bool _dashing = false;

    public void Awake()
    {
        _networkBody = GetComponent<NetworkRigidbody>();
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);

        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame)
        {
            _timer = BoltNetwork.ServerFrame;
            _Dash();
        }

        if (_dashing)
        {
            _networkBody.MoveVelocity = Vector3.Scale(_cam.forward,new Vector3(1,0,1)).normalized * _dashForce;
        }
    }

    private void _Dash()
    {
        StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {
        _dashing = true;
        yield return new WaitForSeconds(_dashDuration);
        _dashing = false;
    }


}

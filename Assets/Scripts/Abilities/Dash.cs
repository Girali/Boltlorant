using System.Collections;
using UnityEngine;

public class Dash : Ability
{
    private NetworkRigidbody _networkBody = null;
    [SerializeField]
    private Transform _cam = null;
    private float _dashForce = 20f;
    private float _dashDuration = 1f;
    private bool _dashing = false;

    public void Awake()
    {
        _cooldown = 2;
        _networkBody = GetComponent<NetworkRigidbody>();
        _cost = 1;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);
        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - _cost) >= 0)
        {
            _timer = BoltNetwork.ServerFrame;
            if(entity.HasControl)
                _UI_cooldown.StartCooldown();
            _Dash();
        }

        if (_dashing)
        {
            _networkBody.MoveVelocity = Vector3.Scale(_cam.forward,new Vector3(1,0,1)).normalized * _dashForce;
        }
    }

    private void _Dash()
    {
        if (entity.IsOwner)
            state.Energy -= _cost;
        StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {
        _dashing = true;
        yield return new WaitForSeconds(_dashDuration);
        _dashing = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Ability
{
    [SerializeField]
    GameObject _grenade = null;
    [SerializeField]
    private Transform _cam = null;
    private float _launchForce = 10f;

    public void Awake()
    {
        _cooldown = 2;
        _UI_cooldown = GUI_Controller.Current.Cooldown1;
        _UI_cooldown.InitView(_abilityInterval);
        _cost = 0;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);
        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - _cost) >= 0)
        {
            _timer = BoltNetwork.ServerFrame;
            if (entity.HasControl)
                _UI_cooldown.StartCooldown();
            if(entity.IsOwner)
                _Launch();
        }
    }

    private void _Launch()
    {
        GameObject grenade = BoltNetwork.Instantiate(_grenade, _cam.transform.position, _cam.transform.rotation);
        grenade.GetComponent<NetworkRigidbody>().MoveVelocity = _cam.transform.forward * _launchForce;
    }
}

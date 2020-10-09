using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Ability
{
    public void Awake()
    {
        _cooldown = 2;
        _UI_cooldown = GUI_Controller.Current.Cooldown1;
        _UI_cooldown.InitView(_abilityInterval);
        _cost = 1;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);
        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - _cost) >= 0)
        {
            _timer = BoltNetwork.ServerFrame;
            if (entity.HasControl)
                _UI_cooldown.StartCooldown();
            _Launch();
        }
    }

    private void _Launch()
    {

    }
}

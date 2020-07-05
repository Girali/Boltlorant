using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class PlayerWeapons : EntityBehaviour<IPlayerState>
{
    [SerializeField]
    private Camera _cam = null;
    [SerializeField]
    private Camera _weaponsCam = null;
    [SerializeField]
    private Weapon _weapon = null;

    public override void Attached()
    {
        state.OnFire = () =>
        {
            _weapon.FireEffect();
        };
    }
    public void Init()
    {
        if (!entity.HasControl)
            _weaponsCam.enabled = false;
        _weapon.Init(entity, _cam.transform);
    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload)
    {
        _weapon.ExecuteCommand(fire, aiming, reload);
    }
}

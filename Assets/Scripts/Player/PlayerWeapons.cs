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
    private List<Weapon> _weapons;
    private int _weaponIndex = 0;
    public int weaponIndex
    {
        get => _weaponIndex;
    }

    public override void Attached()
    {
        state.OnFire = () =>
        {
            _weapons[_weaponIndex].FireEffect();
        };
    }
    public void Init()
    {
        if (!entity.HasControl)
            _weaponsCam.enabled = false;
        for(int i=0;i<_weapons.Count;i++)
        {
            _weapons[i].Init(entity, _cam.transform);
        }
        SetWeapon(state.WeaponIndex);

    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload, int wheel)
    {
        if (wheel != _weaponIndex)
        {
            SetWeapon(wheel);
        }
        _weapons[_weaponIndex].ExecuteCommand(fire, aiming, reload);
    }

    public void SetWeapon(int index)
    {
        for (int i = 0; i < _weapons.Count; i++)
            _weapons[i].gameObject.SetActive(false);
        _weaponIndex = index;
        _weapons[_weaponIndex].gameObject.SetActive(true);
    }

    public int CalculateIndex(float valueToAdd)
    {
        return _AbsoluteModulo(_weaponIndex + (int)Mathf.Sign(valueToAdd), _weapons.Count); 
    }

    private int _AbsoluteModulo(int x, int m)
    {
        return (x % m + m) % m;
    }

    
}

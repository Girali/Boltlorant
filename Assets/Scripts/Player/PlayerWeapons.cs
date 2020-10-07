using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class PlayerWeapons : EntityBehaviour<IPlayerState>
{
    [SerializeField]
    private Camera _cam = null;
    public Camera cam
    { get => _cam; }
    [SerializeField]
    private Camera _weaponsCam = null;
    [SerializeField]
    private List<Weapon> _weapons = null;
    private int _weaponIndex = 0;
    private float _precisionFactor = 0;
    private NetworkRigidbody _networkRigidbody = null;
    private PlayerMotor _playerMotor = null;
    private WeaponID _primary = WeaponID.Glock;
    private WeaponID _secondary = WeaponID.None;

    public int WeaponIndex
    {
        get => _weaponIndex;
    }

    public Camera Cam { get => _cam; }
    public float PrecisionFactor { get => _precisionFactor; set => _precisionFactor = value; }

    private void Awake()
    {
        _networkRigidbody = GetComponent<NetworkRigidbody>();
        _playerMotor = GetComponent<PlayerMotor>();
    }

    public void Update()
    {
        _precisionFactor = Mathf.Lerp(_precisionFactor, _networkRigidbody.MoveVelocity.magnitude / _playerMotor.Speed, 0.05f);
    }

    public bool CanAddWeapon(WeaponID toAdd)
    {
        bool addIt = true;

        if (_primary != WeaponID.None)
            if ((int)toAdd <= 3)
                addIt = false;
        if (_secondary != WeaponID.None)
            if ((int)toAdd > 3)
                addIt = false;

        return addIt;
    }

    public void AddWeapon(WeaponID toAdd)
    {

    }

    public void AddWeapon(WeaponDropToken token)
    {

    }

    public void DropWeapon(WeaponID toRemove)
    {

    }

    public void OnDeath(bool b)
    {

    }

    public void Init()
    {
        if (!entity.HasControl)
            _weaponsCam.enabled = false;

        for(int i=0;i<_weapons.Count;i++)
        {
            _weapons[i].Init(this);
        }

        SetWeapon(_weaponIndex);
    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload, int wheel,int seed)
    {
        if (wheel != state.WeaponIndex)
        {
            if(entity.IsOwner)
                state.WeaponIndex = wheel;
        }

        _weapons[_weaponIndex].ExecuteCommand(fire, aiming, reload, seed);
    }

    public void FireEffect(int seed,float precision)
    {
        _weapons[_weaponIndex].FireEffect(seed,precision);
    }

    public void SetWeapon(int index)
    {
        _weaponIndex = index;

        for (int i = 0; i < _weapons.Count; i++)
            _weapons[i].gameObject.SetActive(false);

        _weapons[_weaponIndex].gameObject.SetActive(true);
    }

    public int CalculateIndex(float valueToAdd)
    {
        int i = 0;

        if (valueToAdd > 0)
            i++;
        else if (valueToAdd < 0)
            i--;

        return Mathf.Abs((_weaponIndex + i + _weapons.Count) % _weapons.Count);
    }
}

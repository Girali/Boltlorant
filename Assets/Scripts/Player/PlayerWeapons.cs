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
    private Weapon[] _weapons = null;
    [SerializeField]
    private GameObject[] _weaponPrefabs = null;
    private int _weaponIndex = 0;
    private float _precisionFactor = 0;
    private NetworkRigidbody _networkRigidbody = null;
    private PlayerMotor _playerMotor = null;
    private PlayerCallback _playerCallback = null;
    private WeaponID _primary = WeaponID.Glock;
    private WeaponID _secondary = WeaponID.RPG;

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
        _playerCallback = GetComponent<PlayerCallback>();
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

    public void AddWeapon(WeaponDropToken token, bool init = false)
    {
        BoltConsole.Write(token.ID.ToString());
        
        if (token.ID == WeaponID.None)
            return;

        GameObject prefab = null;

        foreach (GameObject w in _weaponPrefabs)
        {
            if (w.GetComponent<Weapon>().WeaponStat.ID == token.ID)
            {
                prefab = w;
                break;
            }
        }

        prefab = Instantiate(prefab, _weaponsCam.transform.position, Quaternion.LookRotation(_weaponsCam.transform.forward), _weaponsCam.transform);

        if ((int)token.ID <= 3)
        {
            _primary = token.ID;
            _weapons[1] = prefab.GetComponent<Weapon>();
        }
        else
        {
            _secondary = token.ID;
            _weapons[2] = prefab.GetComponent<Weapon>();
        }

        prefab.GetComponent<Weapon>().Init(this);
        prefab.GetComponent<Weapon>().InitAmmo(token.currentAmmo, token.totalAmmp);

        if (entity.IsOwner && !init)
            _playerCallback.AddWeapon(token);
    }

    public void RemoveWeapon(int i)
    {
        Destroy(_weapons[i].gameObject);
        _weapons[i] = null;

        if (i == 1)
            _primary = WeaponID.None;
        else if (i == 2)
            _secondary = WeaponID.None;
    }

    public void DropWeapon(WeaponID toRemove,bool random)
    {
        if (toRemove == WeaponID.None)
            return;

        if (toRemove == _primary)
        {
            if (entity.IsOwner)
            {
                WeaponDropToken token = new WeaponDropToken();
                token.ID = toRemove;
                token.currentAmmo = _weapons[1].CurrentAmmo;
                token.totalAmmp = _weapons[1].TotalAmmo;

                GameObject g = null;

                if (random)
                    g = BoltNetwork.Instantiate(_weapons[1].WeaponStat.drop, token, _weaponsCam.transform.position, Quaternion.identity);
                else
                    g = BoltNetwork.Instantiate(_weapons[1].WeaponStat.drop, token, _weaponsCam.transform.position + _weaponsCam.transform.forward, Quaternion.LookRotation(_weaponsCam.transform.forward));

                g.GetComponent<WeaponDrop>().Init(_playerMotor);
            }
            Destroy(_weapons[1].gameObject);
            _weapons[1] = null;
            _primary = WeaponID.None;

            if (entity.IsOwner)
                _playerCallback.RemoveWeapon(1);
        }
        else if(toRemove == _secondary)
        {
            if (entity.IsOwner)
            {
                WeaponDropToken token = new WeaponDropToken();
                token.ID = toRemove;
                token.currentAmmo = _weapons[2].CurrentAmmo;
                token.totalAmmp = _weapons[2].TotalAmmo;

                GameObject g = null;

                if (random)
                    g = BoltNetwork.Instantiate(_weapons[2].WeaponStat.drop, token, _weaponsCam.transform.position, Quaternion.identity);
                else
                    g = BoltNetwork.Instantiate(_weapons[2].WeaponStat.drop, token, _weaponsCam.transform.position + _weaponsCam.transform.forward, Quaternion.LookRotation(Random.onUnitSphere));
            
                g.GetComponent<WeaponDrop>().Init(_playerMotor);
            }
            Destroy(_weapons[2].gameObject);
            _weapons[2] = null;
            _secondary = WeaponID.None;

            if (entity.IsOwner)
                _playerCallback.RemoveWeapon(2);
        }
        else
        {
            //TODO
        }
    }

    public void OnDeath(bool b)
    {
        if (entity.IsControllerOrOwner)
        {
            DropWeapon(_primary, true);
            DropWeapon(_secondary, true);
        }
    }

    public void Init()
    {
        if (!entity.HasControl)
            _weaponsCam.enabled = false;

        _weapons[0].Init(this);


        Weapon prefab = null;

        if (_primary != WeaponID.None)
        {
            foreach (GameObject w in _weaponPrefabs)
            {
                if (w.GetComponent<Weapon>().WeaponStat.ID == _primary)
                {
                    prefab = w.GetComponent<Weapon>();
                    break;
                }
            }

            WeaponDropToken token = new WeaponDropToken();
            token.ID = prefab.WeaponStat.ID;
            token.currentAmmo = prefab.WeaponStat.magazin;
            token.totalAmmp = prefab.WeaponStat.totalMagazin;

            AddWeapon(token, true);
        }

        if (_secondary != WeaponID.None)
        {
            foreach (GameObject w in _weaponPrefabs)
            {
                if (w.GetComponent<Weapon>().WeaponStat.ID == _secondary)
                {
                    prefab = w.GetComponent<Weapon>();
                    break;
                }
            }

            WeaponDropToken token = new WeaponDropToken();
            token.ID = prefab.WeaponStat.ID;
            token.currentAmmo = prefab.WeaponStat.magazin;
            token.totalAmmp = prefab.WeaponStat.totalMagazin;

            AddWeapon(token, true);
        }

        SetWeapon(_weaponIndex);
    }

    private bool _dropUp = false;
    private bool _dropDown = false;
    private bool _dropPressed = false;

    public void DropCurrent(bool drop)
    {
        _dropUp = false;
        _dropDown = false;
        if (drop)
        {
            if (_dropPressed == false)
            {
                _dropPressed = true;
                _dropDown = true;
            }
        }
        else
        {
            if (_dropPressed)
            {
                _dropPressed = false;
                _dropUp = true;
            }
        }

        if (_dropDown)
        {
            if(_weaponIndex != 0)
            {
                if (_weaponIndex == 1)
                    DropWeapon(_primary, false);
                else if (_weaponIndex == 2)
                    DropWeapon(_secondary, false);
                else if (_weaponIndex == 3)
                    Debug.Log("Drop Bomb");

                if (entity.IsOwner)
                    state.WeaponIndex = CalculateIndex(1);
            }
        }
    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload, bool drop, int wheel,int seed)
    {
        if (wheel != state.WeaponIndex)
        {
            if(_weapons[wheel] != null)
                if(entity.IsOwner)
                    state.WeaponIndex = wheel;
        }

        DropCurrent(drop);

        if(_weapons[_weaponIndex])
            _weapons[_weaponIndex].ExecuteCommand(fire, aiming, reload, seed);
    }

    public void FireEffect(int seed,float precision)
    {
        _weapons[_weaponIndex].FireEffect(seed,precision);
    }

    public void SetWeapon(int index)
    {
        _weaponIndex = index;

        for (int i = 0; i < _weapons.Length; i++)
            if (_weapons[i] != null)
                _weapons[i].gameObject.SetActive(false);

        _weapons[_weaponIndex].gameObject.SetActive(true);
    }

    public int CalculateIndex(float valueToAdd)
    {
        int i = _weaponIndex;
        int factor = 0;

        if (valueToAdd > 0)
            factor = 1;
        else if (valueToAdd < 0)
            factor = -1;

        i += factor;

        if (i == -1)
            i = 3;

        if (i == 4)
            i = 0;

        while (_weapons[i] == null)
        {
            i += factor;

            if (i == -1)
                i = 3;

            if (i == 4)
                i = 0;
        }

        return i;
    }
}

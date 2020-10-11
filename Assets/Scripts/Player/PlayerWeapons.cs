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
    private WeaponID _primary = WeaponID.None;
    private WeaponID _secondary = WeaponID.None;

    public int WeaponIndex
    {
        get => _weaponIndex;
    }

    public Camera Cam { get => _cam; }
    public float PrecisionFactor { get => _precisionFactor; set => _precisionFactor = value; }
    public bool HasBomb { get => _weapons[3] != null; }

    private void Awake()
    {
        _networkRigidbody = GetComponent<NetworkRigidbody>();
        _playerMotor = GetComponent<PlayerMotor>();
        _playerCallback = GetComponent<PlayerCallback>();
    }

    public void FixedUpdate()
    {
        _precisionFactor = Mathf.Lerp(_precisionFactor, _networkRigidbody.MoveVelocity.magnitude / _playerMotor.Speed, BoltNetwork.FrameDeltaTime * 5f);
    }

    public bool CanAddWeapon(WeaponID toAdd)
    {
        bool addIt = true;

        PlayerToken pt = (PlayerToken)entity.AttachToken;
        if(pt.team == Team.AT && toAdd == WeaponID.Bomb)
            addIt = false;

        if (_primary != WeaponID.None)
            if ((int)toAdd <= 3)
                addIt = false;
        if (_secondary != WeaponID.None)
            if ((int)toAdd > 3)
                addIt = false;
        

        return addIt;
    }

    public void InitAmmo(int i, int current, int total)
    {
        _weapons[i].InitAmmo(current, total);
    }

    public void RefillWeapon(WeaponID id)
    {
        Weapon prefab = null;

        foreach (GameObject w in _weaponPrefabs)
        {
            if (w.GetComponent<Weapon>().WeaponStat.ID == id)
            {
                prefab = w.GetComponent<Weapon>();
                break;
            }
        }

        int i = 0;

        if ((int)id <= 3)
        {
            i = 1;
        }
        else
        {
            i = 2;
        }

        state.Weapons[i].CurrentAmmo = prefab.WeaponStat.magazin;
        state.Weapons[i].TotalAmmo = prefab.WeaponStat.totalMagazin;
    }

    public void AddWeaponEvent(WeaponID id)
    {
        if (id == WeaponID.None)
            return;

        Weapon prefab = null;

        foreach (GameObject w in _weaponPrefabs)
        {
            if (w.GetComponent<Weapon>().WeaponStat.ID == id)
            {
                prefab = w.GetComponent<Weapon>();
                break;
            }
        }

        int i = 0;

        if ((int)id <= 3)
        {
            i = 1;
        }
        else if(prefab.WeaponStat.ID != WeaponID.Bomb)
        {
            i = 2;
        }
        else
        {
            i = 3;
        }

        state.Weapons[i].ID = 1 + (int)id;
        state.Weapons[i].CurrentAmmo = prefab.WeaponStat.magazin;
        state.Weapons[i].TotalAmmo = prefab.WeaponStat.totalMagazin;
    }

    public void AddWeaponEvent(int i, int ca , int ta)
    {
        if (i <= 3)
        {
            state.Weapons[1].ID = 1 + i;
            state.Weapons[1].CurrentAmmo = ca;
            state.Weapons[1].TotalAmmo = ta;
        }
        else if(i != (int)WeaponID.Bomb)
        {
            state.Weapons[2].ID = 1 + i;
            state.Weapons[2].CurrentAmmo = ca;
            state.Weapons[2].TotalAmmo = ta;
        }
        else
        {
            state.Weapons[3].ID = 1 + i;
            state.Weapons[3].CurrentAmmo = ca;
            state.Weapons[3].TotalAmmo = ta;
        }
    }

    public void AddWeapon(WeaponID id)
    {
        if (id == WeaponID.None)
            return;

        GameObject prefab = null;

        foreach (GameObject w in _weaponPrefabs)
        {
            if (w.GetComponent<Weapon>().WeaponStat.ID == id)
            {
                prefab = w;
                break;
            }
        }

        prefab = Instantiate(prefab, _weaponsCam.transform.position, Quaternion.LookRotation(_weaponsCam.transform.forward), _weaponsCam.transform);

        if ((int)id <= 3)
        {
            if (_primary != WeaponID.None)
            {
                if (entity.IsOwner)
                {
                    WeaponDropToken token = new WeaponDropToken();
                    token.ID = _primary;
                    token.currentAmmo = _weapons[1].CurrentAmmo;
                    token.totalAmmp = _weapons[1].TotalAmmo;

                    GameObject g = BoltNetwork.Instantiate(_weapons[1].WeaponStat.drop, token, _weaponsCam.transform.position + _weaponsCam.transform.forward, Quaternion.LookRotation(_weaponsCam.transform.forward));

                    g.GetComponent<WeaponDrop>().Init(_playerMotor);
                }

                Destroy(_weapons[1].gameObject);
            }

            _primary = id;
            _weapons[1] = prefab.GetComponent<Weapon>();
        }
        else if(id != WeaponID.Bomb)
        {
            if (_secondary != WeaponID.None)
            {
                if (entity.IsOwner)
                {
                    WeaponDropToken token = new WeaponDropToken();
                    token.ID = _secondary;
                    token.currentAmmo = _weapons[2].CurrentAmmo;
                    token.totalAmmp = _weapons[2].TotalAmmo;

                    GameObject g = BoltNetwork.Instantiate(_weapons[2].WeaponStat.drop, token, _weaponsCam.transform.position + _weaponsCam.transform.forward, Quaternion.LookRotation(_weaponsCam.transform.forward));

                    g.GetComponent<WeaponDrop>().Init(_playerMotor);
                }
                Destroy(_weapons[2].gameObject);
            }

            _secondary = id;
            _weapons[2] = prefab.GetComponent<Weapon>();
        }
        else
        {
            _weapons[3] = prefab.GetComponent<Weapon>();
        }


        prefab.GetComponent<Weapon>().Init(this);
    }

    public void RemoveWeapon(int i)
    {
        if(_weapons[i])
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

                state.Weapons[1].ID = -1;
            }

            Destroy(_weapons[1].gameObject);
            _weapons[1] = null;
            _primary = WeaponID.None;
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

                state.Weapons[2].ID = -1;
            }

            Destroy(_weapons[2].gameObject);
            _weapons[2] = null;
            _secondary = WeaponID.None;
        }
        else if(_weapons[3] != null)
        {
            if (entity.IsOwner)
            {
                WeaponDropToken token = new WeaponDropToken();
                token.ID = toRemove;

                GameObject g = null;

                if (random)
                    g = BoltNetwork.Instantiate(_weapons[3].WeaponStat.drop, token, _weaponsCam.transform.position, Quaternion.identity);
                else
                    g = BoltNetwork.Instantiate(_weapons[3].WeaponStat.drop, token, _weaponsCam.transform.position + _weaponsCam.transform.forward, Quaternion.LookRotation(Random.onUnitSphere));

                g.GetComponent<WeaponDrop>().Init(_playerMotor);

                state.Weapons[3].ID = -1;
            }

            Destroy(_weapons[3].gameObject);
            _weapons[3] = null;
        }
    }

    public void OnDeath(bool b)
    {
        if (b)
        {
            if (entity.IsControllerOrOwner)
            {
                DropWeapon(_primary, true);
                DropWeapon(_secondary, true);
                DropWeapon(WeaponID.Bomb, true);
            }

            if (entity.IsOwner)
            {
                state.WeaponIndex = CalculateIndex(1);
            }
        }
        else
        {
            if (entity.IsOwner)
            {
                if (state.Weapons[1].ID == -1)
                    AddWeaponEvent(WeaponID.Glock);
                else
                    RefillWeapon((WeaponID)(state.Weapons[1].ID - 1));

                if (state.Weapons[2].ID != -1)
                    RefillWeapon((WeaponID)(state.Weapons[2].ID - 1));
            }
        }
    }

    public void Init()
    {
        if (!entity.HasControl)
            _weaponsCam.enabled = false;

        _weapons[0].Init(this);

        if (entity.IsOwner)
        {
            AddWeaponEvent(WeaponID.Glock);
            AddWeaponEvent(WeaponID.RPG);
        }

        SetWeapon(_weaponIndex);
    }

    private bool _dropPressed = false;

    public void DropCurrent(bool drop)
    {
        if (drop)
        {
            if (_dropPressed == false)
            {
                _dropPressed = true;

                if (_weaponIndex != 0)
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
        else
        {
            if (_dropPressed)
            {
                _dropPressed = false;
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

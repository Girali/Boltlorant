using System.Collections;
using UnityEngine;
using Bolt;
using System.Collections.Generic;

public class Weapon : EntityBehaviour<IPlayerState>
{
    protected Transform _camera;
    [SerializeField]
    protected WeaponStats _weaponStat = null;
    [SerializeField]
    protected Animator _aniamtor = null;
    [SerializeField]
    protected GameObject _weapon = null;


    protected int _currentAmmo = 0;
    protected int _currentTotalAmmo = 0;
    protected bool _isReloading = false;
    protected PlayerWeapons _playerWeapons;
    protected PlayerController _playerController;
    protected PlayerCallback _playerCallback;
    protected NetworkRigidbody _networkRigidbody;

    [SerializeField]
    protected ParticleSystem _muzzleFlash = null;
    protected int _fireFrame = 0;
    protected float _basePrecision = 0;
    protected float _precision = 0;
    protected float _recoil = 0f;
    private Coroutine _reloadCrt = null;

    private bool _scoping = false;
    private float _baseSensitivity;
    private float _scopeSensitivity;

    protected Dictionary<PlayerMotor, int> _dmgCounter;

    protected int _fireInterval
    {
        get
        {
            int rps = _weaponStat.rpm / 60;
            return BoltNetwork.FramesPerSecond / rps;
        }
    }

    private void OnEnable()
    {
        if (_playerWeapons.entity.HasControl)
        {
            GUI_Controller.Current.UpdateAmmo(_currentAmmo, _currentTotalAmmo);
            GUI_Controller.Current.InitCrossair(_weaponStat.crossairLimits);
        }

        if (_playerWeapons.entity.IsControllerOrOwner)
        {
            if (_currentAmmo == 0)
                _reloadCrt = StartCoroutine(Reloading());
        }
    }

    private void OnDisable()
    {
        if (_isReloading)
        {
            _isReloading = false;
            StopCoroutine(_reloadCrt);
        }
    }

    private void FixedUpdate()
    {
        //_precision = _weaponStat.precision * (_playerWeapons.PrecisionFactor * _weaponStat.precisionMoveFactor);
        _precision = _playerWeapons.PrecisionFactor + _recoil;
        BoltConsole.Write(_recoil.ToString());
        if (_recoil != 0f)
            if (_recoil < 0.1f)
                _recoil = 0f;
            else
                _recoil = Mathf.Lerp(_recoil, 0f, BoltNetwork.FrameDeltaTime * 10);
        if (_scoping)
            _precision *= _weaponStat.scopePrecision;

        if (_playerWeapons.entity.HasControl)
        {
            GUI_Controller.Current.UpdateCrossair(_precision * _weaponStat.precisionMoveFactor);
        }
    }

    public virtual void Init(PlayerWeapons pw)
    {
        _playerWeapons = pw;

        if (!_playerWeapons.entity.HasControl)
            gameObject.layer = 0;

        _playerController = pw.GetComponent<PlayerController>();
        _playerCallback = pw.GetComponent<PlayerCallback>();
        _networkRigidbody = pw.GetComponent<NetworkRigidbody>();
        _camera = _playerWeapons.Cam.transform;

        _basePrecision = _weaponStat.precision * _weaponStat.precisionMoveFactor;
        _currentAmmo = _weaponStat.magazin;
        _currentTotalAmmo = _weaponStat.totalMagazin;
        _baseSensitivity = _playerController.mouseSensitivity;
        _scopeSensitivity = _baseSensitivity * _weaponStat.scopeSensitivity;
    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload,int seed)
    {
        if (!_isReloading)
        {
            if (reload && _currentAmmo != _weaponStat.magazin && _currentTotalAmmo > 0)
            {
                _Reload();
            }
            else
            {
                if (fire)
                {
                    _Fire(seed);
                }
                if(_weaponStat.canScope)
                {
                    if(_scoping != aiming)
                    {
                        _scoping = aiming;
                        _Aiming(_scoping);
                    }
                }
            }
        }
    }

    protected virtual void _Fire(int seed)
    {
        if (_currentAmmo >= _weaponStat.ammoPerShot)
        {
            if (_fireFrame + _fireInterval <= BoltNetwork.ServerFrame)
            {
                int dmg = 0;
                _fireFrame = BoltNetwork.ServerFrame;
                _playerCallback.CreateFireEffect(seed, _precision);
                FireEffect(seed, _precision);

                _currentAmmo -= _weaponStat.ammoPerShot;
                GUI_Controller.Current.UpdateAmmo(_currentAmmo, _currentTotalAmmo);
                Random.InitState(seed);

                _dmgCounter = new Dictionary<PlayerMotor, int>();
                for (int i = 0; i < _weaponStat.multiShot; i++)
                {
                    Vector2 rnd = Random.insideUnitSphere * _precision * _basePrecision;
                    BoltConsole.Write(rnd.ToString());
                    Ray r = new Ray(_camera.position, _camera.forward + (_camera.up * rnd.y) + (_camera.right * rnd.x));
                    RaycastHit rh;

                    if (Physics.Raycast(r, out rh, _weaponStat.maxRange))
                    {
                        PlayerMotor target = rh.transform.GetComponent<PlayerMotor>();
                        if (target != null)
                        {
                            if (target.IsHeadshot(rh.collider))
                                dmg = (int)(_weaponStat.dmg * 1.5f);
                            else
                                dmg = _weaponStat.dmg;
                            if (!_dmgCounter.ContainsKey(target))
                                _dmgCounter.Add(target, dmg);
                            else
                                _dmgCounter[target] += dmg;
                        }
                    }
                }

                foreach (PlayerMotor pm in _dmgCounter.Keys)
                    pm.Life -= _dmgCounter[pm];
                _recoil += _weaponStat.recoil;
            }
        }
        else if (_currentTotalAmmo > 0)
        {
            _Reload();
        }
    }

    public virtual void FireEffect(int seed, float precision)
    {
        Random.InitState(seed);
        for (int i = 0; i < _weaponStat.multiShot; i++)
        {
            Vector2 rnd = Random.insideUnitSphere * precision * _basePrecision;
            Ray r = new Ray(_camera.position, _camera.forward + (_camera.up * rnd.y) + (_camera.right * rnd.x));
            RaycastHit rh;
            _aniamtor.SetTrigger("Fire");

            if (Physics.Raycast(r, out rh))
            {
                if (_weaponStat.impactPrefab)
                    Instantiate(_weaponStat.impactPrefab, rh.point, Quaternion.LookRotation(rh.normal));
                if (_weaponStat.trailPrefab)
                {
                    var trailGo = Instantiate(_weaponStat.trailPrefab, _muzzleFlash.transform.position, Quaternion.identity);
                    var trail = trailGo.GetComponent<LineRenderer>();

                    trail.SetPosition(0, _muzzleFlash.transform.position);
                    trail.SetPosition(1, rh.point);
                }
            }
            else if (_weaponStat.trailPrefab)
            {
                var trailGo = Instantiate(_weaponStat.trailPrefab, _muzzleFlash.transform.position, Quaternion.identity);
                var trail = trailGo.GetComponent<LineRenderer>();

                trail.SetPosition(0, _muzzleFlash.transform.position);
                trail.SetPosition(1, _camera.forward * _weaponStat.maxRange + _camera.position);
            }
        }
        if (_muzzleFlash != null)
            _muzzleFlash.Play(true);
    }

    private void _Aiming(bool aim)
    {
        if(entity.HasControl)
        {
            GUI_Controller.Current.ShowScope(aim);
            _weapon.SetActive(!aim);
            if (aim)
            {
                _camera.GetComponent<Camera>().fieldOfView = 40;
                _playerController.mouseSensitivity = _scopeSensitivity;
            }
            else
            {
                _camera.GetComponent<Camera>().fieldOfView = 75;
                _playerController.mouseSensitivity = _baseSensitivity;
            }
        }
    }
    
    protected void _Reload()
    {
        _reloadCrt = StartCoroutine(Reloading());
    }

    
    IEnumerator Reloading()
    {
        _isReloading = true;
        yield return new WaitForSeconds(_weaponStat.reloadTime);
        GUI_Controller.Current.UpdateAmmo(_currentAmmo, _currentTotalAmmo);
        _currentTotalAmmo += _currentAmmo;
        int _ammo = Mathf.Min(_currentTotalAmmo, _weaponStat.magazin);
        _currentTotalAmmo -= _ammo;
        _currentAmmo = _ammo;
        _isReloading = false;
        GUI_Controller.Current.UpdateAmmo(_currentAmmo, _currentTotalAmmo);
    }
}

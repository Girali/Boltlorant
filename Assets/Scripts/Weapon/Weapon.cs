using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Transform _camera;
    [SerializeField]
    protected WeaponStats _weaponStat = null;
    [SerializeField]
    protected Animator _aniamtor = null;
    //private bool _reloading = false;
    //private bool _reloadNeed = false;
    //private Coroutine _reloadCoroutine = null;
    protected int _currentAmmo = 0;
    protected int _currentTotalAmmo = 0;
    protected bool _isReloading = false;
    protected PlayerWeapons _playerWeapons;
    protected PlayerCallback _playerCallback;
    protected NetworkRigidbody _networkRigidbody;

    [SerializeField]
    protected ParticleSystem _muzzleFlash = null;
    protected int _fireFrame = 0;
    protected float _basePrecision = 0;
    protected float _precision = 0;
    private Coroutine _reloadCrt = null;

    protected int _fireInterval
    {
        get
        {
            int rps = (_weaponStat.rpm / 60);
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
        _precision = _playerWeapons.PrecisionFactor;
        
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

        _playerCallback = pw.GetComponent<PlayerCallback>();
        _networkRigidbody = pw.GetComponent<NetworkRigidbody>();
        _camera = _playerWeapons.Cam.transform;

        _basePrecision = _weaponStat.precision * _weaponStat.precisionMoveFactor;
        _currentAmmo = _weaponStat.magazin;
        _currentTotalAmmo = _weaponStat.totalMagazin;
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
                if (aiming)
                {
                    _Aiming();
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
                _fireFrame = BoltNetwork.ServerFrame;
                _playerCallback.CreateFireEffect(seed, _precision);
                FireEffect(seed, _precision);

                _currentAmmo -= _weaponStat.ammoPerShot;
                GUI_Controller.Current.UpdateAmmo(_currentAmmo, _currentTotalAmmo);

                Random.InitState(seed);
                Vector2 rnd = Random.insideUnitSphere * _precision * _basePrecision;
                Ray r = new Ray(_camera.position, _camera.forward + (_camera.up * rnd.y) + (_camera.right * rnd.x));
                RaycastHit rh;

                if (Physics.Raycast(r, out rh, _weaponStat.maxRange))
                {
                    PlayerMotor target = rh.transform.GetComponent<PlayerMotor>();
                    if (target != null)
                    {
                        if(target.IsHeadshot(rh.collider))
                            target.Life -= (int)(_weaponStat.dmg * 1.5f);
                        else
                            target.Life -= _weaponStat.dmg;
                    }
                }
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
        if (_muzzleFlash != null)
            _muzzleFlash.Play(true);
    }

    private void _Aiming()
    {
        //TODO aim
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

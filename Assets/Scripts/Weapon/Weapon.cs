using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private IPlayerState _playerState;
    private Transform _camera;
    [SerializeField]
    private WeaponStats _weaponStat = null;

    //private bool _reloading = false;
    //private bool _reloadNeed = false;
    //private Coroutine _reloadCoroutine = null;
    private int _currentAmmo = 0;
    private int _currentTotalAmmo = 0;
    private bool _isReloading = false;

    [SerializeField]
    private ParticleSystem _muzzleFlash = null;

    private int _fireInterval
    {
        get
        {
            int rps = (_weaponStat.rpm / 60);
            return BoltNetwork.FramesPerSecond / rps;
        }
    }
    private int _fireFrame = 0;

    private void Start()
    {
        _currentAmmo = _weaponStat.magazin;
        _currentTotalAmmo = _weaponStat.totalMagazin;
    }

    public void Init(BoltEntity be, Transform camera)
    {
        _playerState = be.GetState<IPlayerState>();
        _camera = camera;
    }

    public void ExecuteCommand(bool fire, bool aiming, bool reload)
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
                    _Fire();
                }
                if (aiming)
                {
                    _Aiming();
                }
            }
        }
    }

    private void _Fire()
    {
        if (_currentAmmo > 0)
        {
            if (_fireFrame + _fireInterval <= BoltNetwork.ServerFrame)
            {
                _fireFrame = BoltNetwork.ServerFrame;
                _playerState.Fire();

                _currentAmmo--;
                Ray r = new Ray(_camera.position, _camera.rotation * Vector3.forward);
                RaycastHit rh;

                if (Physics.Raycast(r, out rh,_weaponStat.maxRange))
                {
                    PlayerMotor target = rh.transform.GetComponent<PlayerMotor>();
                    if (target != null)
                    {
                        //TODO hit
                    }
                }
            }
        }
        else if (_currentTotalAmmo > 0)
        {
            _Reload();
        }
    }

    public void FireEffect()
    {
        Ray r = new Ray(_camera.position, _camera.rotation * Vector3.forward);
        RaycastHit rh;

        if (Physics.Raycast(r, out rh))
        {
            if (_weaponStat.impactPrefab)
                GameObject.Instantiate(_weaponStat.impactPrefab, rh.point, Quaternion.LookRotation(rh.normal));
            if (_weaponStat.trailPrefab)
            {
                var trailGo = GameObject.Instantiate(_weaponStat.trailPrefab, _muzzleFlash.transform.position, Quaternion.identity) as GameObject;
                var trail = trailGo.GetComponent<LineRenderer>();

                trail.SetPosition(0, _muzzleFlash.transform.position);
                trail.SetPosition(1, rh.point);
            }
        }
        else if (_weaponStat.trailPrefab)
        {
            var trailGo = GameObject.Instantiate(_weaponStat.trailPrefab, _muzzleFlash.transform.position, Quaternion.identity) as GameObject;
            var trail = trailGo.GetComponent<LineRenderer>();

            trail.SetPosition(0, _muzzleFlash.transform.position);
            trail.SetPosition(1, _camera.forward * _weaponStat.maxRange + _camera.position);
        }
        _muzzleFlash.Play();
    }

    private void _Aiming()
    {
        //TODO aim
    }
    
    private void _Reload()
    {
        StartCoroutine(Reloading());
    }

    
    IEnumerator Reloading()
    {
        _isReloading = true;
        yield return new WaitForSeconds(_weaponStat.reloadTime);
        _currentTotalAmmo += _currentAmmo;
        int _ammo = Mathf.Min(_currentTotalAmmo, _weaponStat.magazin);

        _currentTotalAmmo -= _ammo;
        _currentAmmo = _ammo;
        _isReloading = false;
    }
    
}

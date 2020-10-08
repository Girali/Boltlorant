using UnityEngine;

public class LauncherWeapon : Weapon
{
    [SerializeField]
    private GameObject _rocket = null;

    protected override void _Fire(int seed)
    {
        if (_currentAmmo > 0)
        {
            if (_fireFrame + _fireInterval <= BoltNetwork.ServerFrame)
            {
                _fireFrame = BoltNetwork.ServerFrame;
                _playerCallback.CreateFireEffect(seed,0);
                FireEffect(seed,0);
                _currentAmmo -= _weaponStat.ammoPerShot;
                GUI_Controller.Current.UpdateAmmo(_currentAmmo, _currentTotalAmmo);
                if (_playerWeapons.entity.IsOwner)
                {
                    GameObject g = BoltNetwork.Instantiate(_rocket, _muzzleFlash.transform.position, _camera.rotation);
                    g.GetComponent<Rocket>().Init(_playerMotor);
                }
            }
        }
        else if (_currentTotalAmmo > 0)
        {
            base._Reload();
        }
    }

    public override void FireEffect(int seed,float precision)
    {
        _muzzleFlash.Play(true);
        _aniamtor.SetTrigger("Fire");
    }
}

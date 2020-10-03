using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Weapon
{
    [SerializeField]
    private GameObject _rocket = null;

    protected override void _Fire()
    {
        if (_currentAmmo > 0)
        {
            if (_fireFrame + _fireInterval <= BoltNetwork.ServerFrame)
            {
                _fireFrame = BoltNetwork.ServerFrame;
                _playerState.Fire();
                _currentAmmo--;
                GUI_Controller.Current.UpdateAmmo(_currentAmmo, _currentTotalAmmo);
                if (entity.IsOwner)
                {
                    BoltNetwork.Instantiate(_rocket, transform.position, _camera.rotation);
                }
            }
        }
        else if (_currentTotalAmmo > 0)
        {
            base._Reload();
        }
    }

    public override void FireEffect()
    {
    }

}

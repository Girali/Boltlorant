using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    private static float SPEED_MULTIPLIER = 1.5f;
    private static int DAMAGE_MULTIPLIER = 2;
    private static float BACK_ANGLE_THRESHOLD = 20f;
    private void OnEnable()
    {
        _playerWeapons.GetComponent<PlayerMotor>().speed *= SPEED_MULTIPLIER;
    }

    private void OnDisable()
    {
        _playerWeapons.GetComponent<PlayerMotor>().speed /= SPEED_MULTIPLIER;
    }

    protected override void _Fire(int seed)
    {
        if (_fireFrame + _fireInterval <= BoltNetwork.ServerFrame)
        {
            _fireFrame = BoltNetwork.ServerFrame;
            _playerCallback.CreateFireEffect(seed);
            FireEffect(seed);

            Ray r = new Ray(_camera.position, _camera.forward);
            RaycastHit rh;

            if (Physics.Raycast(r, out rh, _weaponStat.maxRange))
            {
                PlayerMotor target = rh.transform.GetComponent<PlayerMotor>();
                if (target != null)
                {
                    int dmg = _weaponStat.dmg;
                    BoltConsole.Write(Vector3.Angle(Vector3.Scale(_camera.forward, Vector3.up), target.transform.forward).ToString());
                    if (Vector3.Angle(Vector3.Scale(_camera.forward,Vector3.up), target.transform.forward) < BACK_ANGLE_THRESHOLD)
                        dmg *= DAMAGE_MULTIPLIER;
                    target.Life -= dmg;
                }
            }
        }
    }
}

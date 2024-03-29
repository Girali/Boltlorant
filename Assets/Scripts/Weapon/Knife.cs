﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    private static float SPEED_MULTIPLIER = 1.5f;
    private static int DAMAGE_MULTIPLIER = 2;
    private static float BACK_ANGLE_THRESHOLD = 60f;
    private static Vector3 VECTOR_SCALE = new Vector3(1, 0, 1);

    private void OnEnable()
    {
        _playerMotor.ChangeSpeed(_playerMotor.baseSpeed * SPEED_MULTIPLIER);
    }

    private void OnDisable()
    {
        _playerMotor.ChangeSpeed(_playerMotor.baseSpeed);
    }

    protected override void _Fire(int seed)
    {
        if (_fireFrame + _fireInterval <= BoltNetwork.ServerFrame)
        {
            _fireFrame = BoltNetwork.ServerFrame;
            _playerCallback.CreateFireEffect(seed,0);
            FireEffect(seed,0);

            Ray r = new Ray(_camera.position, _camera.forward);
            RaycastHit rh;

            if (Physics.Raycast(r, out rh, _weaponStat.maxRange))
            {
                PlayerMotor target = rh.transform.GetComponent<PlayerMotor>();
                if (target != null)
                {
                    int dmg = _weaponStat.dmg;
                    if (Vector3.Angle(Vector3.Scale(_camera.forward, VECTOR_SCALE).normalized, target.transform.forward) < BACK_ANGLE_THRESHOLD)
                        dmg *= DAMAGE_MULTIPLIER;
                    target.Life(_playerMotor,-dmg);
                }
            }
        }
    }
}

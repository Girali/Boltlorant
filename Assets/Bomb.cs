using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Weapon
{
    private float _plantingTime = 7.5f;
    private GameController _gameController = null;
    private bool _pressed = false;
    private bool _buttonUp;
    private bool _buttonDown;
    private float _plantedTime = 0;

    public override void Init(PlayerWeapons pw)
    {
        base.Init(pw);
        if (_gameController == null)
            _gameController = FindObjectOfType<GameController>();
    }

    public override void ExecuteCommand(bool fire, bool aiming, bool reload, int seed)
    {
        base.ExecuteCommand(fire, aiming, reload, seed);

        if (_gameController.IsInSite)
        {
            _buttonUp = false;
            _buttonDown = false;
            if (fire)
            {
                if (_pressed == false)
                {
                    _pressed = true;
                    _buttonDown = true;
                    _plantedTime = BoltNetwork.ServerTime + _plantingTime;
                }

                if (_plantedTime < BoltNetwork.ServerTime && _plantedTime != 0)
                {
                    if (_playerWeapons.entity.IsOwner)
                    {
                        BoltNetwork.Instantiate(BoltPrefabs.BombGoal, transform.position, Quaternion.identity);
                        _gameController.Planted();
                    }
                }
            }
            else
            {
                if (_pressed)
                {
                    _pressed = false;
                    _buttonUp = true;
                    _plantedTime = 0;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameZone : MonoBehaviour
{
    private Dictionary<int, float> _playerHitted = new Dictionary<int, float>();
    private int _dmg = 10;
    private float _time = 0.2f;
    private float _interval
    {
        get => _time * BoltNetwork.FramesPerSecond;
    }
    private PlayerMotor _launcher = null;
    public PlayerMotor laucher { set => _launcher = value; }

    private void OnTriggerStay(Collider other)
    {
        if (_launcher != null)
        {
            PlayerMotor player = other.GetComponent<PlayerMotor>();
            if (player != null)
            {
                if (!_playerHitted.ContainsKey(player.GetHashCode()))
                {
                    _playerHitted.Add(player.GetHashCode(), BoltNetwork.ServerFrame);
                    player.Life(_launcher, -_dmg);
                }
                else
                {
                    if(_playerHitted[player.GetHashCode()] + _interval <= BoltNetwork.ServerFrame)
                    {
                        _playerHitted[player.GetHashCode()] = BoltNetwork.ServerFrame;
                        player.Life(_launcher, -_dmg);
                    }
                }
            }
        }
    } 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Photon.Realtime;

public class PlayerCallback : EntityBehaviour<IPlayerState>
{
    PlayerMotor _playerMotor;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
    }

    public override void Attached()
    {
        state.AddCallback("Life", UpdatePlayerLife);
    }

    public void UpdatePlayerLife()
    {
        if (entity.HasControl)
            GUI_Controller.Current.UpdateLife(state.Life, _playerMotor.TotalLife);
    }
}

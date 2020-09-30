using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Bolt.EntityBehaviour<IPlayerState>
{
    private PlayerMotor _playerMotor;
    private PlayerWeapons _playerWeapons;

    bool _forward;
    bool _backward;
    bool _left;
    bool _right;
    bool _jump;
    bool _fire;
    bool _aiming;
    bool _reload;
    bool _ability;

    float _yaw;
    float _pitch;
    int _wheel = 0;

    float _mouseSensitivity = 5f;

    public override void Attached()
    {
        _playerMotor.Init();
        _playerWeapons.Init();
        state.AddCallback("WeaponIndex",_SetWeapon);
    }

    public override void ControlGained()
    {
        GUI_Controller.Current.Show(true);
        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
    }

    public void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _playerWeapons = GetComponent<PlayerWeapons>();
    }

    void Update()
    {
        _PollKeys();
    }

    private void _PollKeys()
    {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);
        _jump = Input.GetKey(KeyCode.Space);
        _fire = Input.GetMouseButton(0);
        _aiming = Input.GetMouseButton(1);
        _reload = Input.GetKey(KeyCode.R);
        _ability = Input.GetKey(KeyCode.Q);

        _yaw += Input.GetAxisRaw("Mouse X") * _mouseSensitivity;
        _yaw %= 360f;
        _pitch += -Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -85, 85);

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _wheel = _playerWeapons.CalculateIndex(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    public override void SimulateController()
    {
        IPlayerCommandInput input = PlayerCommand.Create();
        input.Forward = _forward;
        input.Backward = _backward;
        input.Right = _right;
        input.Left = _left;
        input.Jump = _jump;
        input.Fire = _fire;
        input.Aiming = _aiming;
        input.Reload = _reload;
        input.Ability = _ability;
        input.Yaw = _yaw;
        input.Pitch = _pitch;
        input.Wheel = _wheel;

        entity.QueueInput(input);

        _playerMotor.ExecuteCommand(_forward, _backward, _left, _right, _jump, _ability, _yaw, _pitch);
        _playerWeapons.ExecuteCommand(_fire, _aiming, _reload, _wheel);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        PlayerCommand cmd = (PlayerCommand)command;

        if (resetState)
        {
            _playerMotor.SetState(cmd.Result.Position, cmd.Result.Rotation);
        }
        else
        {
            PlayerMotor.State motorState = new PlayerMotor.State();
            if (!entity.HasControl)
            {
                motorState = _playerMotor.ExecuteCommand(
                cmd.Input.Forward,
                cmd.Input.Backward,
                cmd.Input.Left,
                cmd.Input.Right,
                cmd.Input.Jump,
                cmd.Input.Ability,
                cmd.Input.Yaw,
                cmd.Input.Pitch);

                _playerWeapons.ExecuteCommand(
                    cmd.Input.Fire, 
                    cmd.Input.Aiming,
                    cmd.Input.Reload,
                    cmd.Input.Wheel);
            }

            if (entity.IsOwner)
                state.WeaponIndex = _playerWeapons.weaponIndex;

            cmd.Result.Position = motorState.position;
            cmd.Result.Rotation = cmd.Input.Yaw;
        }
    }

    private void _SetWeapon()
    {
        if (!entity.HasControl && !entity.IsOwner)
            _playerWeapons.SetWeapon(state.WeaponIndex);
    }
}

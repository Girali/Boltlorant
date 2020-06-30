using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Bolt.EntityBehaviour<IPlayerState>
{
    private PlayerMotor playerMotor;

    bool _forward;
    bool _backward;
    bool _left;
    bool _right;

    float _yaw;
    float _pitch;

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        if (!entity.HasControl)
            playerMotor.DisableCamera();
    }

    public void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        PollKeys();
    }

    void PollKeys()
    {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);

        _yaw += Input.GetAxisRaw("Mouse X");
        _yaw %= 360f;
        _pitch += -Input.GetAxisRaw("Mouse Y");
    }

    public override void SimulateController()
    {
        IPlayerCommandInput input = PlayerCommand.Create();
        input.Forward = _forward;
        input.Backward = _backward;
        input.Right = _right;
        input.Left = _left;
        input.Yaw = _yaw;
        input.Pitch = _pitch;

        entity.QueueInput(input);

        playerMotor.Move(_forward, _backward, _left, _right, _yaw, _pitch);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        PlayerCommand cmd = (PlayerCommand)command;

        if (resetState)
        {
            // we got a correction from the server, reset (this only runs on the client)
            playerMotor.SetState(cmd.Result.Position, cmd.Result.Rotation);
        }
        else
        {

            PlayerMotor.State motorState = new PlayerMotor.State();
            if (!entity.HasControl)
                motorState = playerMotor.Move(
                cmd.Input.Forward,
                cmd.Input.Backward,
                cmd.Input.Left,
                cmd.Input.Right,
                cmd.Input.Yaw,
                cmd.Input.Pitch);


            cmd.Result.Position = motorState.position;
            cmd.Result.Rotation = cmd.Input.Yaw;

        }
    }
}

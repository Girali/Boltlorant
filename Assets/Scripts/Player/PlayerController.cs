using Bolt;
using UnityEngine;

public class PlayerController : Bolt.EntityBehaviour<IPlayerState>
{
    private PlayerMotor _playerMotor;
    private PlayerWeapons _playerWeapons;
    private PlayerRenderer _playerRenderer;

    bool _forward;
    bool _backward;
    bool _left;
    bool _right;
    bool _jump;
    bool _fire;
    bool _aiming;
    bool _reload;
    bool _drop;
    bool _ability1;
    bool _ability2;

    float _yaw;
    float _pitch;
    int _wheel = 0;
    int _seed = 0;
    float _mouseSensitivity = 5f;
    public float mouseSensitivity
    {
        get => _mouseSensitivity;
        set => _mouseSensitivity = value;
    }

    public int Wheel { get => _wheel; set => _wheel = value; }

    public override void Attached()
    {
        var token = (PlayerToken)entity.AttachToken;
        _playerMotor.Init(token.characterClass);
        _playerWeapons.Init();
        _playerRenderer.Init();

        if (entity.HasControl)
            Cursor.lockState = CursorLockMode.Locked;
    }

    public override void ControlGained()
    {
        GUI_Controller.Current.Show(true);
    }

    public void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _playerWeapons = GetComponent<PlayerWeapons>();
        _playerRenderer = GetComponent<PlayerRenderer>();
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
        if (_fire)
            _seed = Random.Range(0, 1023);
        _aiming = Input.GetMouseButton(1);
        _reload = Input.GetKey(KeyCode.R);
        _drop = Input.GetKey(KeyCode.G);
        _ability1 = Input.GetKey(KeyCode.Q);
        _ability2 = Input.GetKey(KeyCode.E);

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
        input.Drop = _drop;
        input.Ability1 = _ability1;
        input.Ability2 = _ability2;
        input.Yaw = _yaw;
        input.Pitch = _pitch;
        input.Wheel = _wheel;
        input.Seed = _seed;
        
        entity.QueueInput(input);

        _playerMotor.ExecuteCommand(_forward, _backward, _left, _right, _jump, _ability1, _ability2, _yaw, _pitch);
        _playerWeapons.ExecuteCommand(_fire, _aiming, _reload, _drop, _wheel, _seed);
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
                cmd.Input.Ability1,
                cmd.Input.Ability2,
                cmd.Input.Yaw,
                cmd.Input.Pitch);

                _playerWeapons.ExecuteCommand(
                    cmd.Input.Fire, 
                    cmd.Input.Aiming,
                    cmd.Input.Reload,
                    cmd.Input.Drop,
                    cmd.Input.Wheel,
                    cmd.Input.Seed);
            }

            cmd.Result.Position = motorState.position;
            cmd.Result.Rotation = cmd.Input.Yaw;
        }
    }
}

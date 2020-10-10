using Bolt;
using UnityEngine;
using System.Collections;

public class PlayerMotor : EntityBehaviour<IPlayerState>
{

    [SerializeField]
    private Camera _cam = null;
    private NetworkRigidbody _networkBody = null;
    private bool _jumpPressed=false;
    private float _jumpForce = 9f;

    private float _speed = 7f;
    private float _baseSpeed = 7f;
    public float baseSpeed
    {
        get => _baseSpeed;
    }

    [SerializeField]
    private int _totalLife = 250;
    private float _maxAngle = 45f;
    private bool _isGrounded = false;
    private SphereCollider _headCollider = null;
    private CapsuleCollider _capsuleCollider = null;

    [SerializeField]
    private Ability _ability1 = null;
    [SerializeField]
    private Ability _ability2 = null;
    private bool _isEnemy = true;

    private Vector3 _lastServerPos = Vector3.zero;
    private bool _firstState = true;

    public void ChangeSpeed(float speed)
    {
        _speed = speed;
    }

    void Awake()
    {
        _networkBody = GetComponent<NetworkRigidbody>();
        _headCollider = GetComponent<SphereCollider>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void OnDeath(bool b)
    {
        _headCollider.enabled = !b;
        _capsuleCollider.enabled = !b;
        _networkBody.enabled = !b;
    }

    public bool IsHeadshot(Collider c)
    {
        return c == _headCollider;
    }

    public void Life(PlayerMotor killer, int life)
    {
        if (entity.IsOwner)
        {
            int value = state.Life + life;

            if (value < 0)
            {
                state.Life = 0;
                state.IsDead = true;

                if (killer.state.Money < 8000)
                    killer.state.Money += 600;
                else if (killer.state.Money + 600 > 8000)
                    killer.state.Money = 8000;
            }
            else if(value > _totalLife)
            {
                state.Life = _totalLife;
            }
            else
            {
                state.Life = value;
            }
        }
    }

    public int TotalLife { get => _totalLife; set => _totalLife = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public bool IsEnemy { get => _isEnemy; }

    public void Init(CharacterClass characterClass)
    {

        if (entity.IsOwner)
        {
            state.Life = _totalLife;
        }

        if (entity.HasControl)
        {
            tag = "LocalPlayer";
            GUI_Controller.Current.UpdateLife(_totalLife,_totalLife);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject go in players)
            {
                go.GetComponent<PlayerMotor>().TeamCheck();
                go.GetComponent<PlayerRenderer>().Init();
            }
        }
        else
        {
            _cam.enabled = false;
        }

        TeamCheck();
    }

    public void TeamCheck()
    {
        GameObject localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
        Team t = Team.AT;
        PlayerToken pt = (PlayerToken)entity.AttachToken;

        if (localPlayer)
        {
            PlayerToken lpt = (PlayerToken)localPlayer.GetComponent<PlayerMotor>().entity.AttachToken;
            t = lpt.team;
        }

        if (pt.team == t)
            _isEnemy = false;
        else
            _isEnemy = true;
    }

    private void FixedUpdate()
    {
        if (entity.IsAttached)
        {
            if (entity.IsControllerOrOwner)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.3f))
                {
                    float slopeNormal = Mathf.Abs(Vector3.Angle(hit.normal, new Vector3(hit.normal.x, 0, hit.normal.z)) - 90) % 90;

                    if (_networkBody.MoveVelocity.y < 0)
                        _networkBody.MoveVelocity = Vector3.Scale(_networkBody.MoveVelocity, new Vector3(1, 0, 1));

                    if (!_isGrounded && slopeNormal <= _maxAngle)
                    {
                        _isGrounded = true;
                    }
                }
                else
                {
                    if (_isGrounded)
                    {
                        _isGrounded = false;
                    }
                }
            }
        }
    }

    public State ExecuteCommand(bool forward, bool backward, bool left, bool right, bool jump, bool ability1,bool ability2, float yaw, float pitch)
    {
        Vector3 movingDir = Vector3.zero;
        if (forward ^ backward)
        {
            movingDir += forward ? transform.forward : -transform.forward;
        }
        if (left ^ right)
        {
            movingDir += right ? transform.right : -transform.right;
        }

        movingDir.Normalize();
        movingDir *= _speed;

        if (jump)
        {
            if (_jumpPressed == false && _isGrounded)
            {
                _isGrounded = false;
                _jumpPressed = true;
                _networkBody.MoveVelocity += Vector3.up * _jumpForce;
            }
        }
        else
        {
            if (_jumpPressed)
                _jumpPressed = false;
        }

        _networkBody.MoveVelocity = new Vector3(movingDir.x, _networkBody.MoveVelocity.y, movingDir.z);
        _cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        _ability1.UpdateAbility(ability1);
        _ability2.UpdateAbility(ability2);

        State stateMotor = new State();
        stateMotor.position = transform.position;
        stateMotor.rotation = yaw;
        
        return stateMotor;
    }

    public void SetState(Vector3 position, float rotation)
    {
        if (Mathf.Abs(rotation - transform.rotation.y) > 5f)
            transform.rotation = Quaternion.Euler(0, rotation, 0);

        if (_firstState)
        {
            if (position != Vector3.zero)
            {
                transform.position = position;
                _firstState = false;
                _lastServerPos = Vector3.zero;
            }
        }
        else
        {
            if (position != Vector3.zero)
            {
                _lastServerPos = position;
            }

            transform.position += (_lastServerPos - transform.position) * 0.5f;
        }
    }

    public struct State
    {
        public Vector3 position;
        public float rotation;
    }
}

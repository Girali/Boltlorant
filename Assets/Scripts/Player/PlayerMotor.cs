using Bolt;
using UnityEngine;

public class PlayerMotor : EntityBehaviour<IPlayerState>
{


    [SerializeField]
    private Camera _cam = null;
    private NetworkRigidbody _networkBody = null;
    private bool _jumpPressed=false;
    private float _speed = 7f;
    public float speed
    {
        get => _speed;
        set => _speed = value;
    }
    private float _jumpForce = 9f;
    [SerializeField]
    private int _totalLife = 250;
    private float _maxAngle = 45f;
    private bool _isGrounded = false;
    private SphereCollider _headCollider = null;
    private CapsuleCollider _capsuleCollider = null;

    private Ability _ability1 = null;
    private Ability _ability2 = null;
    private bool _isEnemy = true;

    private Vector3 _lastServerPos = Vector3.zero;
    private bool _firstState = true;

    void Awake()
    {
        _networkBody = GetComponent<NetworkRigidbody>();
        _headCollider = GetComponent<SphereCollider>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public bool IsHeadshot(Collider c)
    {
        return c == _headCollider;
    }

    public int Life
    {
        set
        {
            if (entity.IsOwner)
                state.Life = value;
        }

        get
        {
            return state.Life;
        }
    }

    public int TotalLife { get => _totalLife; set => _totalLife = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public bool IsEnemy { get => _isEnemy; }

    public void Init(CharacterClass characterClass)
    {
        foreach (Ability a in GetComponents<Ability>())
            a.enabled = false;
        switch (characterClass)
        {
            case CharacterClass.Soldier:
                _ability1 = GetComponent<Dash>();
                _ability2 = GetComponent<Wall>();
                break;
            case CharacterClass.Medic:
                _ability1 = GetComponent<Dash>();
                _ability2 = GetComponent<Wall>();
                break;
            case CharacterClass.Heavy:
                _ability1 = GetComponent<Dash>();
                _ability2 = GetComponent<Wall>();
                break;
        }

        _ability1.enabled = true;
        _ability2.enabled = true;

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
                go.GetComponent<PlayerRenderer>().Init();
            }
        }
        else
        {
            _cam.enabled = false;
        }

        GameObject localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
        Team t = Team.AT;
        if (localPlayer)
        {
            PlayerToken lpt = (PlayerToken)localPlayer.GetComponent<PlayerMotor>().entity.AttachToken;
            t = lpt.team;
        }
        PlayerToken pt = (PlayerToken)entity.AttachToken;

        if (pt.team == t)
            _isEnemy = false;
    }

    private void FixedUpdate()
    {
        if (entity.IsAttached)
        {
            if (entity.IsControllerOrOwner)
            { 
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
                {
                    float slopeNormal = Mathf.Abs(Vector3.Angle(hit.normal, new Vector3(hit.normal.x, 0, hit.normal.z)) - 90) % 90;

                    if (!_isGrounded && slopeNormal <= _maxAngle)
                    {
                        _isGrounded = true;
                        _networkBody.UseGravity = false;
                        if(_networkBody.MoveVelocity.y < 0)
                            _networkBody.MoveVelocity = Vector3.Scale(_networkBody.MoveVelocity,new Vector3(1,0,1));
                    }
                    //else if (_isGrounded)
                    //{
                    //    if (_networkBody.MoveVelocity.y < 0f)
                    //        _networkBody.MoveVelocity += new Vector3(0, 1.5f * _networkBody.GravityForce , 0);
                    //    else if (_networkBody.MoveVelocity.y > 0f)
                    //        _networkBody.MoveVelocity += new Vector3(0, 1f * _networkBody.GravityForce , 0);
                    //}
                }
                else
                {
                    if (_isGrounded)
                    {
                        _networkBody.UseGravity = true;
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
            if (Vector3.Distance(transform.position, _lastServerPos) > 0.1f)
            {
                transform.position += (_lastServerPos - transform.position) * BoltNetwork.FrameDeltaTime * 5f;
            }
        }
    }

    public struct State
    {
        public Vector3 position;
        public float rotation;
    }
}

using UnityEngine;

public class Wall : Ability
{
    [SerializeField]
    private Transform _cam = null;
    [SerializeField]
    private LayerMask _layerMask = 0;
    private static float MAX_DISTANCE = 5f;
    private static float VERTICAL_THRESHOLD = 0.4f;

    [SerializeField]
    private GameObject _wallPreset = null;
    private GameObject _wallInstatiated = null;
    [SerializeField]
    private GameObject _stateMachine = null;
    private GameObject _preview = null;
    private bool _previewMode = false;

    private MeshRenderer _renderer;
    private bool _tooFar=false;

    private Color _red;
    private Color _blue;

    private UI_Cooldown _UI_cooldown;
    private RaycastHit hit;

    public void Awake()
    {
        _cooldown = 10;
        _red = new Color(255 / 255, 81 / 255, 0, 40 / 255);
        _blue = new Color(0, 183 / 255, 255 / 255, 40 / 255);
        _cost = 2;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);
        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - _cost) >= 0)
        {
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, Mathf.Infinity, _layerMask))
            {
                if (entity.HasControl)
                {
                    if (_preview == null)
                    {
                        _preview = GameObject.Instantiate(_stateMachine, hit.point, _cam.transform.rotation);
                        _renderer = _preview.GetComponent<MeshRenderer>();
                        _renderer.material.SetColor("_Color", _blue);
                        //_renderer.material.color = _blue;
                    }
                    else
                    {
                        if (!_tooFar)
                        {
                            _UI_cooldown.StartCooldown();
                            _timer = BoltNetwork.ServerFrame;
                        }
                        _tooFar = false;
                        GameObject.Destroy(_preview);
                    }
                }
                if (entity.IsOwner)
                {
                    if (_previewMode)
                    {
                        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, Mathf.Infinity, _layerMask))
                        {
                            if (hit.distance < MAX_DISTANCE)
                            {
                                if (entity.IsOwner)
                                    state.Energy -= _cost;

                                _timer = BoltNetwork.ServerFrame;
                                if (_wallInstatiated != null)
                                    BoltNetwork.Destroy(_wallInstatiated);
                                _wallInstatiated = BoltNetwork.Instantiate(_wallPreset, hit.point, Quaternion.Euler(Vector3.Scale(_cam.transform.eulerAngles, Vector3.up)));
                            }
                        }
                    }
                    _previewMode ^= true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (_preview != null)
        {
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, Mathf.Infinity, _layerMask))
            {
                _preview.transform.rotation = Quaternion.Euler(Vector3.Scale(_cam.transform.eulerAngles, Vector3.up));

                _preview.transform.position = hit.point;
                _preview.transform.Translate(Vector3.up * 0.5f);

                if (hit.normal.y > VERTICAL_THRESHOLD)
                {
                    if ((hit.distance > MAX_DISTANCE) != _tooFar)
                    {
                        _tooFar ^= true;
                        _renderer.material.SetColor("_Color", (_tooFar) ? _red : _blue);
                        //_renderer.material.color = (_tooFar) ? _red : _blue;
                    }
                }
                else
                {
                    _tooFar = true;
                    _renderer.material.color = _red;
                }
            }
        }
    }
}

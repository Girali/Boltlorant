using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medikit : Ability
{
    [SerializeField]
    private Transform _cam = null;
    [SerializeField]
    private LayerMask _layerMask;
    private float _maxDistance = 5f;

    private void Awake()
    {
        _cooldown = 0;
        _UI_cooldown = GUI_Controller.Current.Cooldown1;
        _UI_cooldown.InitView(_abilityInterval);
        _cost = 0;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);
        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - _cost) >= 0)
        {
            _timer = BoltNetwork.ServerFrame;
            if (entity.HasControl)
                _UI_cooldown.StartCooldown();
            if (entity.IsOwner)
            {
                state.Energy -= _cost;
                RaycastHit hit;
                if (Physics.Raycast(_cam.position, _cam.transform.forward, out hit, _maxDistance, _layerMask))
                {
                    BoltNetwork.Instantiate(BoltPrefabs.Medc_kit, hit.point, Quaternion.identity).GetComponent<AOE>().laucher=GetComponent<PlayerMotor>();
                }
                else
                    BoltNetwork.Instantiate(BoltPrefabs.Medc_kit, transform.position, Quaternion.identity).GetComponent<AOE>().laucher = GetComponent<PlayerMotor>();

            }
        }
    }
}

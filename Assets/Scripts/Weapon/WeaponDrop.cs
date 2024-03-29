﻿using System.Collections;
using UnityEngine;
using Bolt;

public class WeaponDrop : EntityBehaviour<IPhysicState>
{
    private NetworkRigidbody _networkRigidbody = null;
    private BoxCollider _boxCollider = null;
    private SphereCollider _sphereCollider = null;
    [SerializeField]
    private GameObject _render = null;
    private bool _inited = false;
    private WeaponDropToken _dropToken = null;
    private PlayerMotor _launcher = null;
    private float _time = 0;

    private void Awake()
    {
        _networkRigidbody = GetComponent<NetworkRigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _sphereCollider = GetComponent<SphereCollider>();
        _time = Time.time + 2f;
    }

    private IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();
        _inited = true;
    }

    public void Init(PlayerMotor playerMotor)
    {
        _launcher = playerMotor;
    }

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            if (transform.rotation == Quaternion.identity)
            {
                _networkRigidbody.MoveVelocity = Random.onUnitSphere * 10f;
                transform.eulerAngles = Random.insideUnitSphere * 360f;
            }
            else
            {
                _networkRigidbody.MoveVelocity = transform.forward * 10f;
            }
        }

        _dropToken = (WeaponDropToken)entity.AttachToken;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(entity.IsAttached)
            if (entity.IsOwner && (_inited || !collision.gameObject.GetComponent<PlayerMotor>()))
                _networkRigidbody.MoveVelocity *= 0.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_inited && entity.IsAttached && entity.IsOwner) 
        {
            if (other.GetComponent<PlayerMotor>())
            {
                if (other.GetComponent<PlayerWeapons>().CanAddWeapon(_dropToken.ID))
                {
                    if (other.GetComponent<PlayerMotor>() == _launcher && _time < Time.time)
                    {
                        other.GetComponent<PlayerWeapons>().AddWeaponEvent((int)_dropToken.ID, _dropToken.currentAmmo, _dropToken.totalAmmp);
                        BoltNetwork.Destroy(entity);
                        _networkRigidbody.enabled = false;
                        _boxCollider.enabled = false;
                        _render.SetActive(false);
                        _sphereCollider.enabled = false;
                    }
                    else if (other.GetComponent<PlayerMotor>() != _launcher)
                    {
                        other.GetComponent<PlayerWeapons>().AddWeaponEvent((int)_dropToken.ID, _dropToken.currentAmmo, _dropToken.totalAmmp);
                        BoltNetwork.Destroy(entity);
                        _networkRigidbody.enabled = false;
                        _boxCollider.enabled = false;
                        _render.SetActive(false);
                        _sphereCollider.enabled = false;
                    }
                }
            }
        }
    }
}

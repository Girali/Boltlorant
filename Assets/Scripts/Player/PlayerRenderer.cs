using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class PlayerRenderer : EntityBehaviour<IPlayerState>
{
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private TextMesh _textMesh;
    private PlayerMotor _playerMotor;
    [SerializeField]
    private Color _enemyColor;
    [SerializeField]
    private Color _allyColor;

    [SerializeField]
    private Transform _camera;
    private Transform _sceneCamera;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
    }

    public void OnDeath(bool b)
    {
        if (b)
        {
            if (entity.HasControl)
                _sceneCamera.gameObject.SetActive(true);

            _camera.gameObject.SetActive(false);
            _meshRenderer.gameObject.SetActive(false);
            _textMesh.gameObject.SetActive(false);
        }
        else
        {
            if (entity.IsControllerOrOwner)
                _camera.gameObject.SetActive(true);

            if (entity.HasControl)
            {
                _sceneCamera.gameObject.SetActive(false);
            }
            else
            {
                _meshRenderer.gameObject.SetActive(true);

                if (!_playerMotor.IsEnemy)
                {
                    _textMesh.gameObject.SetActive(true);
                }
            }
        }
    }

    public void Init()
    {
        if (entity.IsControllerOrOwner)
            _camera.gameObject.SetActive(true);

        if (entity.HasControl) 
        {
            _sceneCamera = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerSetupController>().SceneCamera;
            _sceneCamera.gameObject.SetActive(false);
        }
        else
        {
            _meshRenderer.gameObject.SetActive(true);

            if (_playerMotor.IsEnemy)
            {
                _meshRenderer.material.color = _enemyColor;
            }
            else
            {
                _textMesh.gameObject.SetActive(true);
                PlayerToken pt = (PlayerToken)entity.AttachToken;
                _textMesh.text = pt.name;
                _meshRenderer.material.color = _allyColor;
            }
        }
    }
}

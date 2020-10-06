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

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
    }

    public void Init()
    {
        if (entity.HasControl) 
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerSetupController>().SceneCamera.gameObject.SetActive(false);
            _camera.gameObject.SetActive(true);
        }
        else
        {
            _textMesh.gameObject.SetActive(true);
            _meshRenderer.gameObject.SetActive(true);

            if (_playerMotor.IsEnemy)
            {
                _meshRenderer.material.color = _enemyColor;
                _textMesh.gameObject.SetActive(false);
            }
            else
            {
                _meshRenderer.material.color = _allyColor;
                PlayerToken pt = (PlayerToken)entity.AttachToken;
                _textMesh.text = pt.name;
            }

        }

    }
}

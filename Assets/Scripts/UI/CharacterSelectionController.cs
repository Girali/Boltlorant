using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField]
    PlayerSetupController _playerSetupController;

    public void InstantiatePlayer()
    {
        _playerSetupController.RaiseSpawnPlayerEvent();
        gameObject.SetActive(false);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField]
    PlayerSetupController playerSetupController;

    public void InstantiatePlayer()
    {
        playerSetupController.RaiseSpawnPlayerEvent();
        gameObject.SetActive(false);
    }


}

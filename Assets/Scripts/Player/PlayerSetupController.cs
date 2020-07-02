﻿using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetupController : Bolt.GlobalEventListener
{
    [SerializeField]
    private GameObject _classSelector = null;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
            _classSelector.SetActive(true);
    }

    public void RaiseSpawnPlayerEvent()
    {
        SpawnPlayerEvent spawn = SpawnPlayerEvent.Create(Bolt.GlobalTargets.OnlyServer);
        spawn.Send();
    }

    public override void OnEvent(SpawnPlayerEvent evnt)
    {
        Vector3 v = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Player, new Vector3(0f, 1f, 0f) + v, Quaternion.identity);
        entity.AssignControl(evnt.RaisedBy);
    }
}
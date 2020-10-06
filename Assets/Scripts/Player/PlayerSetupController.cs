using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetupController : Bolt.GlobalEventListener
{
    [SerializeField]
    private GameObject _teamSelector = null;
    private Team _selectedTeam = Team.AT;
    [SerializeField]
    private Transform _TTBase = null;
    [SerializeField]
    private Transform _ATBase = null;

    [SerializeField]
    private Transform _sceneCamera = null;

    public Transform SceneCamera { get => _sceneCamera; set => _sceneCamera = value; }

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
            _teamSelector.SetActive(true);
    }

    public void SetTeam(int i)
    {
        _selectedTeam = (Team)i;
    }

    public void RaiseSpawnPlayerEvent(int index)
    {
        SpawnPlayerEvent spawn = SpawnPlayerEvent.Create(Bolt.GlobalTargets.OnlyServer);
        spawn.PlayerName = AppManager.Instance.Username;
        spawn.Team = (short)_selectedTeam;
        spawn.Class = index;
        spawn.Send();
    }

    public override void OnEvent(SpawnPlayerEvent evnt)
    {
        var token = new PlayerToken();
        token.name = evnt.PlayerName;
        token.team = (Team)evnt.Team;
        token.characterClass = (CharacterClass)evnt.Class;

        Vector3 v = Vector3.zero;

        v.y += 9;

        if (token.team == Team.TT)
        {
            v = _TTBase.transform.position;
            v.x += Random.Range(-_TTBase.localScale.x / 2f, _TTBase.localScale.x / 2f);
            v.z += Random.Range(-_TTBase.localScale.z / 2f, _TTBase.localScale.z / 2f);
        }
        else
        {
            v = _ATBase.transform.position;
            v.x += Random.Range(-_ATBase.localScale.x / 2f, _ATBase.localScale.x / 2f);
            v.z += Random.Range(-_ATBase.localScale.z / 2f, _ATBase.localScale.z / 2f);
        }

        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Player, token, v, Quaternion.identity);
        entity.AssignControl(evnt.RaisedBy);
    }
}

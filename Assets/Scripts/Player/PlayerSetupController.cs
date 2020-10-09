using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupController : Bolt.GlobalEventListener
{
    [SerializeField]
    private GameObject _teamSelector = null;
    [SerializeField]
    private GameObject _classSelector = null;
    private Team _selectedTeam = Team.AT;
    [SerializeField]
    private Transform _TTBase = null;
    [SerializeField]
    private Transform _ATBase = null;
    [SerializeField]
    private Text _TTCountText = null;
    [SerializeField]
    private Text _ATCountText = null;
    [SerializeField]
    private Button _TTButton = null;
    [SerializeField]
    private Button _ATButton = null;
    private int _TTCount = 0;
    private int _ATCount = 0;
    [SerializeField]
    private Transform _sceneCamera = null;
    public Transform SceneCamera { get => _sceneCamera; set => _sceneCamera = value; }
    private System.Guid _eventID = System.Guid.Empty;

    public void AddTeamCount(Team t)
    {
        if (t == Team.TT)
            _TTCount++;
        else
            _ATCount++;

        _TTCountText.text = _TTCount.ToString();
        _ATCountText.text = _ATCount.ToString();

        UpdateTeamButtons();
    }

    private void UpdateTeamButtons()
    {
        if (_TTCount == _ATCount)
        {
            _ATButton.interactable = true;
            _TTButton.interactable = true;
        }
        else
        {
            if (_TTCount < _ATCount)
            {
                _ATButton.interactable = false;
                _TTButton.interactable = true;
            }
            else
            {
                _ATButton.interactable = true;
                _TTButton.interactable = false;
            }
        }
    }

    public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
    {
        if (BoltNetwork.IsServer)
        {
            UpdateTeamCountEvent evnt = UpdateTeamCountEvent.Create(connection, ReliabilityModes.ReliableOrdered);
            evnt.AT = _ATCount;
            evnt.TT = _TTCount;
            evnt.Send();
        }
    }

    public override void OnEvent(UpdateTeamCountEvent evnt)
    {
        _ATCount = evnt.AT;
        _TTCount = evnt.TT;

        _TTCountText.text = _TTCount.ToString();
        _ATCountText.text = _ATCount.ToString();

        UpdateTeamButtons();
    }

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
            _teamSelector.SetActive(true);
    }

    public void SetTeam(int t)
    {
        _ATButton.interactable = false;
        _TTButton.interactable = false;
        _selectedTeam = (Team)t;
        ChoseTeamEvent evnt = ChoseTeamEvent.Create(ReliabilityModes.ReliableOrdered);
        evnt.Team = t;
        _eventID = System.Guid.NewGuid();
        evnt.ID = _eventID;
        evnt.Send();
    }

    public override void OnEvent(ChoseTeamEvent evnt)
    {
        if (BoltNetwork.IsServer)
        { 
            bool accepted = true;

            if ((Team)evnt.Team == Team.AT)
            {
                if (_ATCount > _TTCount)
                    accepted = false;
            }
            else
            {
                if (_ATCount < _TTCount)
                    accepted = false;
            }


            AddTeamCount((Team)evnt.Team);

            ConfirmTeamEvent evntT = ConfirmTeamEvent.Create(ReliabilityModes.ReliableOrdered);
            evntT.Team = evnt.Team;
            evntT.Accepted = accepted;
            evntT.ID = evnt.ID;
            evntT.Send();
        }
    }

    public override void OnEvent(ConfirmTeamEvent evnt)
    {
        if (evnt.Accepted)
        {
            if (_eventID == evnt.ID)
            {
                _teamSelector.SetActive(false);
                _classSelector.SetActive(true);
            }


            if (BoltNetwork.IsClient)
                AddTeamCount((Team)evnt.Team);
        }
        else
        {
            UpdateTeamButtons();
        }
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
        BoltEntity entity;
        switch ((CharacterClass)evnt.Class)
        {
            case CharacterClass.Soldier:
                entity = BoltNetwork.Instantiate(BoltPrefabs.Soldier, token, v, Quaternion.identity);
                break;
            case CharacterClass.Medic:
                entity = BoltNetwork.Instantiate(BoltPrefabs.Medic, token, v, Quaternion.identity);
                break;
            default:
                entity = BoltNetwork.Instantiate(BoltPrefabs.Heavy, token, v, Quaternion.identity);
                break;
        }
        entity.AssignControl(evnt.RaisedBy);
    }

    public Vector3 GetSpawnPoint(Team t)
    {
        Vector3 v = Vector3.zero;

        if (t == Team.TT)
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

        return v;
    }
}

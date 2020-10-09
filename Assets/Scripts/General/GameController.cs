using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class GameController : EntityBehaviour<IGameModeState>
{
    #region Singleton
    private static GameController _instance = null;

    public static GameController Current
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameController>();

            return _instance;
        }
    }

    #endregion

    GamePhase _currentPhase = GamePhase.WaitForPlayers;
    int _playerCountTarget = 4;
    float _nextEvent = 0;
    public GamePhase CurrentPhase { get => _currentPhase; }

    public override void Attached()
    {
        state.AddCallback("AlivePlayers", UpdatePlayersAlive);
        state.AddCallback("TTPoints", UpdatePoints);
        state.AddCallback("ATPoints", UpdatePoints);
        state.AddCallback("Timer", UpdateTime);
    }

    public void UpdatePlayersAlive()
    {
        if (entity.IsOwner)
        {
            if (GamePhase.WaitForPlayers == _currentPhase)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    player.GetComponent<PlayerCallback>().RoundReset();
                }
            }
        }
    }

    public void UpdatePoints()
    {

    }

    public void UpdateTime()
    {

    }

    private void Update()
    {
        switch (_currentPhase)
        {
            case GamePhase.WaitForPlayers:
                break;
            case GamePhase.Starting:
                if (_nextEvent < BoltNetwork.ServerTime)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject player in players)
                    {
                        player.GetComponent<PlayerCallback>().RoundReset();
                    }
                    _currentPhase = GamePhase.StartRound;
                    UpdateGameState();
                }
                break;
            case GamePhase.StartRound:
                if (_nextEvent < BoltNetwork.ServerTime)
                {
                    _currentPhase = GamePhase.AT_Defending;
                    UpdateGameState();
                }
                break;
            case GamePhase.AT_Defending:
                break;
            case GamePhase.TT_Planted:
                break;
            case GamePhase.EndGame:
                break;
            default:
                break;
        }

    }

    public void UpdateGameState()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        switch (_currentPhase)
        {
            case GamePhase.WaitForPlayers:
                if(_playerCountTarget == players.Length)
                {
                    _currentPhase = GamePhase.Starting;
                    _nextEvent = BoltNetwork.ServerTime + 10f;
                }
                break;
            case GamePhase.Starting:
                break;
            case GamePhase.StartRound:
                //TODO Up wall
                foreach (GameObject player in players)
                {
                    player.GetComponent<PlayerCallback>().RoundReset();
                }
                _nextEvent = BoltNetwork.ServerTime + 10f;
                break;
            case GamePhase.AT_Defending:
                //TODO Down walls
                break;
            case GamePhase.TT_Planted:
                break;
            case GamePhase.EndGame:
                break;
            default:
                break;
        }
    }
}

public enum GamePhase
{
    WaitForPlayers,
    Starting,
    StartRound,
    AT_Defending,
    TT_Planted,
    EndGame
}

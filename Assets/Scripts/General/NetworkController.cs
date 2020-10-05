using UnityEngine;
using UdpKit;
using System;
using Bolt;
using Bolt.Matchmaking;

public class NetworkController : GlobalEventListener
{
    public UnityEngine.UI.Text feedback;
    public UnityEngine.UI.Text username;

    public void Connect()
    {
        if (username.text != "")
        {
            AppManager.Instance.username = username.text;
            BoltLauncher.StartClient();
            FeedbackUser("Starting");
        }
        else
            FeedbackUser("Enter a valid name");
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        FeedbackUser("Searching ...");

        BoltMatchmaking.JoinSession(HeadlessServerManager.RoomID());
    }

    public void FeedbackUser(string text)
    {
        feedback.gameObject.SetActive(true);
        feedback.text = text;
    }
}
using UnityEngine;
using UdpKit;
using System;
using Bolt;
using Bolt.Matchmaking;

public class NetworkController : GlobalEventListener
{
    public UnityEngine.UI.Text feedback;

    public void Connect()
    {
        FeedbackUser("Starting");
        BoltLauncher.StartClient();
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        FeedbackUser("Searching ...");

        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                BoltMatchmaking.JoinSession(photonSession);
            }
        }
    }

    public void FeedbackUser(string text)
    {
        feedback.gameObject.SetActive(true);
        feedback.text = text;
    }
}
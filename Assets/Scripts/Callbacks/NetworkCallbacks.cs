using Bolt;
using UnityEngine;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (BoltNetwork.IsServer)
        {
            if (scene == HeadlessServerManager.Map())
            {
                if(!GameController.Current)
                    BoltNetwork.Instantiate(BoltPrefabs.GameController);
            }
        }
    }

    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PlayerToken>();
        BoltNetwork.RegisterTokenClass<WeaponDropToken>();
    }
}

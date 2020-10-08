using UnityEngine;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PlayerToken>();
        BoltNetwork.RegisterTokenClass<WeaponDropToken>();
    }

}

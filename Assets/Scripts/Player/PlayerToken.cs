using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToken : Bolt.IProtocolToken
{
    public string name;
    public Team team;
    public CharacterClass characterClass;

    public void Write(UdpKit.UdpPacket packet)
    {
        packet.WriteString(name);
        packet.WriteShort((short)team);
        packet.WriteShort((short)characterClass);
    }

    public void Read(UdpKit.UdpPacket packet)
    {
        name = packet.ReadString();
        team = (Team)packet.ReadShort();
        characterClass = (CharacterClass)packet.ReadShort();
    }
}

public enum CharacterClass
{
    Scout,
    Heavy,
    Medic
}
public enum Team
{
    T,
    AT
}

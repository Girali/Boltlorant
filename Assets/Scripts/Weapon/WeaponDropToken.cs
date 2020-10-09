using System.Collections;
using Bolt;
using UdpKit;

public class WeaponDropToken : Bolt.IProtocolToken
{
    public WeaponID ID;
    public int currentAmmo;
    public int totalAmmp;

    void IProtocolToken.Read(UdpPacket packet)
    {
        ID = (WeaponID)packet.ReadShort();
        currentAmmo = packet.ReadInt();
        totalAmmp = packet.ReadInt();
    }

    void IProtocolToken.Write(UdpPacket packet)
    {
        packet.WriteShort((short)ID);
        packet.WriteInt(currentAmmo);
        packet.WriteInt(totalAmmp);
    }
}

public enum WeaponID
{
    None = -1,
    Glock,
    Revolver,
    DoubleBeretas,
    GlockSilencer,
    Shotgun,
    RPG,
    HMG,
    Sniper,
    SMG,
    AK_47
}
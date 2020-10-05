using Bolt;

public class PlayerCallback : EntityEventListener<IPlayerState>
{
    PlayerMotor _playerMotor;
    PlayerWeapons _playerWeapons;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _playerWeapons = GetComponent<PlayerWeapons>();
    }

    public override void Attached()
    {
        state.AddCallback("Life", UpdatePlayerLife);
        state.AddCallback("WeaponIndex", UpdateWeaponIndex);
    }

    public void CreateFireEffect(int seed)
    {
        if (entity.IsOwner)
        {
            FireEffectEvent evnt = FireEffectEvent.Create(entity, EntityTargets.EveryoneExceptOwnerAndController);
            evnt.Seed = seed;
            evnt.Send();
        }
    }

    public override void OnEvent(FireEffectEvent evnt)
    {
        _playerWeapons.FireEffect(evnt.Seed);
    }

    public void UpdateWeaponIndex()
    {
        _playerWeapons.SetWeapon(state.WeaponIndex);
    }

    public void UpdatePlayerLife()
    {
        if (entity.HasControl)
            GUI_Controller.Current.UpdateLife(state.Life, _playerMotor.TotalLife);
    }
}

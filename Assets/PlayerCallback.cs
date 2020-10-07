using Bolt;
using UnityEngine;
public class PlayerCallback : EntityEventListener<IPlayerState>
{
    private PlayerMotor _playerMotor;
    private PlayerWeapons _playerWeapons;
    private PlayerRenderer _playerRenderer;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _playerWeapons = GetComponent<PlayerWeapons>();
        _playerRenderer = GetComponent<PlayerRenderer>();
    }

    public override void Attached()
    {
        state.AddCallback("Life", UpdatePlayerLife);
        state.AddCallback("WeaponIndex", UpdateWeaponIndex);
        state.AddCallback("Energy", UpdateEnergy);
        if (entity.IsOwner)
        {
            state.IsDead = false;
            state.Energy = 1;
        }
        state.AddCallback("IsDead", UpdateDeathState);
    }

    private void UpdateDeathState()
    {
        _playerMotor.OnDeath(state.IsDead);
        _playerRenderer.OnDeath(state.IsDead);
    }

    public void UpdateEnergy()
    {
        GUI_Controller.Current.UpdateAbilityView(state.Energy);
    }

    public void CreateFireEffect(int seed, float precision)
    {
        BoltConsole.Write(precision.ToString());
        if (entity.IsOwner)
        {
            FireEffectEvent evnt = FireEffectEvent.Create(entity, EntityTargets.EveryoneExceptOwnerAndController);
            evnt.Seed = seed;
            evnt.Precision = precision;
            evnt.Send();
        }
    }

    public override void OnEvent(FireEffectEvent evnt)
    {
        BoltConsole.Write(evnt.Precision.ToString());
        _playerWeapons.FireEffect(evnt.Seed,evnt.Precision);
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

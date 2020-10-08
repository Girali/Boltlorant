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

        state.AddCallback("IsDead", UpdateDeathState);
        state.AddCallback("Money", UpdateMoney);

        if (entity.IsOwner)
        {
            state.IsDead = false;
            state.Energy = 1;
            state.Money = 250;
        }
    }

    private void UpdateMoney()
    {
        if(entity.HasControl)
            GUI_Controller.Current.UpdateMoney(state.Money);
    }

    private void UpdateDeathState()
    {
        _playerMotor.OnDeath(state.IsDead);
        _playerRenderer.OnDeath(state.IsDead);
        _playerWeapons.OnDeath(state.IsDead);
    }

    public void UpdateEnergy()
    {
        if(entity.HasControl)
            GUI_Controller.Current.UpdateAbilityView(state.Energy);
    }

    public void CreateFireEffect(int seed, float precision)
    {
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
        _playerWeapons.FireEffect(evnt.Seed,evnt.Precision);
    }

    public void RemoveWeapon(int i)
    {
        RemoveWeaponEvent evnt = RemoveWeaponEvent.Create(entity,EntityTargets.EveryoneExceptOwnerAndController);
        evnt.Index = i;
        evnt.Send();
    }

    public override void OnEvent(RemoveWeaponEvent evnt)
    {
        _playerWeapons.RemoveWeapon(evnt.Index);
    }

    public void AddWeapon(WeaponDropToken w)
    {
        AddWeaponEvent evnt = AddWeaponEvent.Create(entity, EntityTargets.EveryoneExceptOwner);
        evnt.Token = w;
        evnt.Send();
    }

    public override void OnEvent(AddWeaponEvent evnt)
    {
        _playerWeapons.AddWeapon((WeaponDropToken)evnt.Token);
    }

    public void UpdateWeaponIndex()
    {
        GetComponent<PlayerController>().Wheel = state.WeaponIndex;
        _playerWeapons.SetWeapon(state.WeaponIndex);
    }

    public void UpdatePlayerLife()
    {
        if (entity.HasControl)
            GUI_Controller.Current.UpdateLife(state.Life, _playerMotor.TotalLife);
    }
}

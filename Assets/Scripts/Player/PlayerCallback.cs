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
        state.AddCallback("Weapons[].ID", UpdateWeaponList);
        state.AddCallback("Weapons[].CurrentAmmo", UpdateWeaponAmmo);
        state.AddCallback("Weapons[].TotalAmmo", UpdateWeaponAmmo);

        if (entity.IsOwner)
        {
            state.IsDead = false;
            state.Energy = 1;
            state.Money = 800;
            state.Life = _playerMotor.TotalLife;
            GameController.Current.UpdateGameState();
            GameController.Current.state.AlivePlayers++;
        }
        else
        {
            FindObjectOfType<PlayerSetupController>().UpdateClassView();
        }
    }

    public void RoundReset(Team winner)
    {
        if (entity.IsOwner)
        {
            if (GameController.Current.CurrentPhase != GamePhase.Starting)
            {
                if (state.IsDead == true)
                {
                    state.IsDead = false;
                    if (GameController.Current.CurrentPhase == GamePhase.WaitForPlayers)
                    {
                        state.Energy = 1;

                        state.Life = _playerMotor.TotalLife;
                        state.SetTeleport(state.Transform);
                        PlayerToken token = (PlayerToken)entity.AttachToken;
                        transform.position = FindObjectOfType<PlayerSetupController>().GetSpawnPoint(token.team);
                    }
                }

                if (GameController.Current.CurrentPhase == GamePhase.StartRound)
                {
                    if (state.Energy < 4)
                        state.Energy += 1;

                    if (state.Money < 8000)
                        state.Money += 800;
                    else if(state.Money + 800 > 8000)
                        state.Money = 8000;

                    PlayerToken token = (PlayerToken)entity.AttachToken;

                    if (token.team == winner)
                    {
                        if (state.Money < 8000)
                            state.Money += 500;
                        else if (state.Money + 500 > 8000)
                            state.Money = 8000;
                    }

                    state.Life = _playerMotor.TotalLife;
                    state.SetTeleport(state.Transform);
                    transform.position = FindObjectOfType<PlayerSetupController>().GetSpawnPoint(token.team);
                }
            }
            else
            {
                state.Money = 0;
                state.Energy = 0;
            }
        }
    }

    public void UpdateWeaponList(IState state, string propertyPath, ArrayIndices arrayIndices)
    {
        int index = arrayIndices[0];
        IPlayerState s = (IPlayerState)state;

        if (s.Weapons[index].ID == -1)
            _playerWeapons.RemoveWeapon(index);
        else
            _playerWeapons.AddWeapon((WeaponID)s.Weapons[index].ID-1);
    }

    public void UpdateWeaponAmmo(IState state, string propertyPath, ArrayIndices arrayIndices)
    {
        int index = arrayIndices[0];
        IPlayerState s = (IPlayerState)state;
        _playerWeapons.InitAmmo(index,s.Weapons[index].CurrentAmmo, s.Weapons[index].TotalAmmo);
    }

    private void UpdateMoney()
    {
        if (entity.HasControl)
        {
            GUI_Controller.Current.UpdateMoney(state.Money);
            GUI_Controller.Current.UpdateShop(state.Energy, state.Money);
        }
    }

    private void UpdateDeathState()
    {
        if (entity.IsOwner)
        {
            if (state.IsDead)
                GameController.Current.state.AlivePlayers--;
            else
                GameController.Current.state.AlivePlayers++;
        }

        if (entity.HasControl)
            GUI_Controller.Current.Show(false);

        _playerMotor.OnDeath(state.IsDead);
        _playerRenderer.OnDeath(state.IsDead);
        _playerWeapons.OnDeath(state.IsDead);
    }

    public void UpdateEnergy()
    {
        if (entity.HasControl)
        {
            GUI_Controller.Current.UpdateShop(state.Energy, state.Money);
            GUI_Controller.Current.UpdateAbilityView(state.Energy);
        }
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

    public void RaiseFlashEvent()
    {
        FlashEvent evnt = FlashEvent.Create(entity, EntityTargets.OnlyController);
        evnt.Send();
    }

    public override void OnEvent(FlashEvent evnt)
    {
        GUI_Controller.Current.Flash();
    }
}

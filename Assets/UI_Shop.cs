using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    [SerializeField]
    private ShopItem[] shopItems;
    [SerializeField]
    private Text _energyCount;
    [SerializeField]
    private UI_ShopItem _energyButton;
    private int[] _energyCost = { 300, 600, 900, 1200, 2000 };
    private string[] _energyView = { "", "*", "**", "***", "****", "*****", "******" };
    private int _currentEnergy = 0;
    private int _currentMoney = 0;

    private void OnEnable()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].button.Init(shopItems[i].ID, shopItems[i].cost);
        }
        //_energyButton.Init(WeaponID.None, _energyCost[GUI_Controller.Current.localPlayer.GetComponent<BoltEntity>().GetState<IPlayerState>().Energy]);
    }

    public void BuyWeapon(int index)
    {
        GUI_Controller.Current.localPlayer.GetComponent<PlayerCallback>().RaiseBuyWeaponEvent(index+1);
    }

    public void UpdateView(int energy, int money)
    {
        _energyCount.text = _energyView[energy];
        _energyButton.Init(WeaponID.None, _energyCost[energy]);
        _energyButton.Interactable(_energyCost[energy] <= money);
        _currentEnergy = energy;

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].button.Interactable(shopItems[i].cost <= money);
        }
    }

    public void BuyEnergy()
    {
        BoltConsole.Write("sjbgvlksrejzbklzsrekjgfze js;g");
        GUI_Controller.Current.localPlayer.GetComponent<PlayerCallback>().RaiseBuyEnergyEvent();
    }

    public int ItemCost(int index)
    {
        return shopItems[index].cost;
    }

    public int EnergyCost()
    {
        return _energyCost[_currentEnergy];
    }
}

[System.Serializable]
public struct ShopItem
{
    public UI_ShopItem button;
    public WeaponID ID;
    public int cost;
}
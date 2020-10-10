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
    }

    public void BuyWeapon(int index)
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].button.Interactable(false);
        }

        //TODO
    }

    public void UpdateView(int energy, int money)
    {
        _energyCount.text = _energyView[energy];
        _energyButton.Init(WeaponID.None, _energyCost[energy]);
        _energyButton.Interactable(_energyCost[energy] <= money);

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].button.Interactable(shopItems[i].cost <= money);
        }
    }

    public void BuyEnergy()
    {
        //TODO
    }
}

[System.Serializable]
public struct ShopItem
{
    public UI_ShopItem button;
    public WeaponID ID;
    public int cost;
}
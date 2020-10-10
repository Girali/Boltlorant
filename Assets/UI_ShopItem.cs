using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopItem : MonoBehaviour
{
    [SerializeField]
    private Text _title;
    [SerializeField]
    private Text _Textcost;
    private int _cost;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Image _icon;

    public void Init(WeaponID id, int cost)
    {
        _Textcost.text = "$ " + cost;
        _cost = cost;
        //Interactable(GUI_Controller.Current.localPlayer.GetComponent<BoltEntity>().GetState<IPlayerState>().Money >= cost);
    }

    public void Interactable(bool b)
    {
        _button.interactable = b;

        if (!b)
        {
            if(_icon)
                _icon.color = Color.white;
            _title.color = Color.white;
            _Textcost.color = Color.red;
        }
        else
        {
            if(_icon)
                _icon.color = Color.black;
            _title.color = Color.black;
            _Textcost.color = Color.green;
        }
    }
}

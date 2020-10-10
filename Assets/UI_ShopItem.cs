using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopItem : MonoBehaviour
{
    [SerializeField]
    private Text _title;
    [SerializeField]
    private Text _cost;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Image _icon;

    public void Init(WeaponID id, int cost)
    {
        _cost.text = "$ " + cost;
    }

    public void Interactable(bool b)
    {
        _button.interactable = b;

        if (!b)
        {
            if(_icon)
                _icon.color = Color.white;
            _title.color = Color.white;
            _cost.color = Color.red;
        }
        else
        {
            if(_icon)
                _icon.color = Color.black;
            _title.color = Color.black;
            _cost.color = Color.green;
        }
    }
}

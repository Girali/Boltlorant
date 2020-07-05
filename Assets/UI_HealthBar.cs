using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField]
    private Gradient _gradient;

    [SerializeField]
    private Image _bg;
    [SerializeField]
    private Image _bar;
    [SerializeField]
    private Text _text;

    public void UpdateLife(int hp,int totalHp)
    {
        float f = (float)hp / (float)totalHp;
        _bar.fillAmount = f;
        Color c = _gradient.Evaluate(f);
        _bg.color = new Color(c.r, c.g, c.b, _bg.color.a);
        _bar.color = c;
        _text.text = hp.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class GUI_Controller : MonoBehaviour
{
    #region Singleton
    private static GUI_Controller _instance = null;

    public static GUI_Controller Current
    {
        get
        {
            if(_instance == null)
                _instance = FindObjectOfType<GUI_Controller>();

            return _instance;
        }
    }

    public UI_Cooldown Cooldown1 { get => _cooldown1; }
    public UI_Cooldown Cooldown2 { get => _cooldown2; }
    #endregion

    [SerializeField]
    private UI_Crossair _crossair = null;
    [SerializeField]
    private UI_HealthBar _healthBar = null;
    [SerializeField]
    private UI_AmmoPanel _ammoPanel = null;

    [SerializeField]
    private Text _energyCount = null;

    [SerializeField]
    private UI_Cooldown _cooldown1 = null;
    [SerializeField]
    private UI_Cooldown _cooldown2 = null;

    private void Start()
    {
        Show(false);
    }

    public void Show(bool active)
    {
        _crossair.gameObject.SetActive(active);
        _healthBar.gameObject.SetActive(active);
        _ammoPanel.gameObject.SetActive(active);
        _energyCount.transform.parent.gameObject.SetActive(active);
        _cooldown1.gameObject.SetActive(active);
        _cooldown2.gameObject.SetActive(active);
    }

    public void UpdateAbilityView(int i)
    {
        _energyCount.text = i.ToString();
        _cooldown1.UpdateCost(i);
        _cooldown2.UpdateCost(i);
    }

    public void UpdateLife(int current, int total)
    {
        _healthBar.UpdateLife(current, total);
    }

    public void UpdateAmmo(int current, int total)
    {
        _ammoPanel.UpdateLife(current, total);
    }

    public void InitCrossair(Vector2 v)
    {
        _crossair.Init(v);
    }

    public void UpdateCrossair(float t)
    {
        _crossair.UpdateView(t);
    }
}

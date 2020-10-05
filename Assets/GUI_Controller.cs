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

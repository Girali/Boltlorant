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
    public Team GuiTeam { set => _guiTeam = value; }
    #endregion

    Team _guiTeam = Team.AT;

    [SerializeField]
    private UI_Crossair _crossair = null;
    [SerializeField]
    private GameObject _scope = null;
    [SerializeField]
    private UI_HealthBar _healthBar = null;
    [SerializeField]
    private UI_AmmoPanel _ammoPanel = null;

    [SerializeField]
    private Text _energyCount = null;

    [SerializeField]
    private Text _money = null;

    [SerializeField]
    private UI_Cooldown _cooldown1 = null;
    [SerializeField]
    private UI_Cooldown _cooldown2 = null;

    [SerializeField]
    private Sprite[] _icons = null;

    [SerializeField]
    private UI_PlayerPlate[] _allayPlates = null;
    [SerializeField]
    private UI_PlayerPlate[] _enemyPlates = null;

    [SerializeField]
    private Text _allayScore;
    [SerializeField]
    private Text _enemyScore;

    [SerializeField]
    private UI_Timer _timer; 

    private void Start()
    {
        Show(false);
    }

    public void UpdatePlayersPlate(GameObject[] players, GameObject localPlayer)
    {
        PlayerMotor pm;
        PlayerToken pt;

        if (localPlayer != null)
        {
            pm = localPlayer.GetComponent<PlayerMotor>();
            pt = (PlayerToken)pm.entity.AttachToken;

            _allayPlates[(int)pt.playerSquadID].Init(_icons[(int)pt.characterClass]);
            _allayPlates[(int)pt.playerSquadID].Death(pm.state.IsDead);
        }

        foreach (GameObject p in players)
        {
            pm = p.GetComponent<PlayerMotor>();
            pt = (PlayerToken)pm.entity.AttachToken;

            if (pm.IsEnemy)
            {
                _enemyPlates[(int)pt.playerSquadID].Init(_icons[(int)pt.characterClass]);
                _enemyPlates[(int)pt.playerSquadID].Death(pm.state.IsDead);
            }
            else
            {
                _allayPlates[(int)pt.playerSquadID].Init(_icons[(int)pt.characterClass]);
                _allayPlates[(int)pt.playerSquadID].Death(pm.state.IsDead);
            }
        }
    }

    public void UpdatePoints(int AT,int TT)
    {
        if(_guiTeam == Team.AT)
        {
            _allayScore.text = AT.ToString();
            _enemyScore.text = TT.ToString();
        }
        else
        {
            _allayScore.text = TT.ToString();
            _enemyScore.text = AT.ToString();
        }
    }

    public void UpdateTimer(float f)
    {
        _timer.Init(f);
    }

    public void UpdateMoney(int i)
    {
        _money.text = "$ " + i;
    }

    public void Show(bool active)
    {
        _crossair.gameObject.SetActive(active);
        _healthBar.gameObject.SetActive(active);
        _ammoPanel.gameObject.SetActive(active);
        _energyCount.transform.parent.gameObject.SetActive(active);
        _money.transform.parent.gameObject.SetActive(active);
        if (_scope.gameObject.activeSelf)
            _scope.gameObject.SetActive(active);
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

    public void ShowScope(bool show)
    {
        _scope.SetActive(show);
    }
}

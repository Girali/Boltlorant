using UnityEngine;
using UnityEngine.UI;
using Bolt;

public class UI_Cooldown : MonoBehaviour
{
    [SerializeField]
    private Image _fill = null;
    [SerializeField]
    private Text _timer = null;
    [SerializeField]
    private Text _shortcut = null;
    [SerializeField]
    private Text _cost1 = null;
    [SerializeField]
    private Text _cost2 = null;
    [SerializeField]
    private Image _BGS = null;
    [SerializeField]
    private Color _gray = Color.gray;
    [SerializeField]
    private Color _blue = Color.blue;
    [SerializeField]
    private Color _darkGray = Color.black;

    float _time;
    float _cdTimer;
    bool _counting = false;

    private void Update()
    {
        if (_counting)
        {
            if (_time > BoltNetwork.ServerFrame)
            {
                _timer.text = FloatToTime(-(BoltNetwork.ServerFrame - _time) / BoltNetwork.FramesPerSecond, "00.0");
                _fill.fillAmount = -(BoltNetwork.ServerFrame - _time) / _cdTimer;
            }
            else
            {
                _counting = false;
                _shortcut.color = _blue;
                _timer.text = "READY";
                _timer.color = _blue;
                _fill.color = _blue;
                _fill.fillAmount = 1;
            }
        }
    }

    public void InitView(float cd)
    {
        _cdTimer = cd;
        _counting = false;
        _shortcut.color = _blue;
        _timer.text = "READY";
        _timer.color = _blue;
        _fill.color = _blue;
        _fill.fillAmount = 1;
    }

    public void StartCooldown()
    {
        Debug.Log("Test");
        _time = _cdTimer + BoltNetwork.ServerFrame;
        _counting = true;
        _shortcut.color = Color.black;
        _timer.text = FloatToTime(_time / BoltNetwork.FramesPerSecond, "00.0");
        _timer.color = Color.white;
        _fill.color = _darkGray;
        _fill.fillAmount = 0;
    }

    public void UpdateCost(int i)
    {
        if(i == 0)
        {
            _cost1.color = _gray;
            if(_cost2)
                _cost2.color = _gray;
        }
        else if(i == 1)
        {
            _cost1.color = _blue;
            if (_cost2)
                _cost2.color = _gray;
        }
        else
        {
            _cost1.color = _blue;
            if (_cost2)
                _cost2.color = _blue;
        }
    }
    public static string FloatToTime(float toConvert, string format)
    {
        switch (format)
        {
            case "00.0":
                return string.Format("{0:00}:{1:0}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
            case "#0.0":
                return string.Format("{0:#0}:{1:0}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
            case "00.00":
                return string.Format("{0:00}:{1:00}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
            case "00.000":
                return string.Format("{0:00}:{1:000}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
            case "#00.000":
                return string.Format("{0:#00}:{1:000}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
            case "#0:00":
                return string.Format("{0:#0}:{1:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60);//seconds
            case "#00:00":
                return string.Format("{0:#00}:{1:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60);//seconds
            case "0:00.0":
                return string.Format("{0:0}:{1:00}.{2:0}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
            case "#0:00.0":
                return string.Format("{0:#0}:{1:00}.{2:0}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
            case "0:00.00":
                return string.Format("{0:0}:{1:00}.{2:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
            case "#0:00.00":
                return string.Format("{0:#0}:{1:00}.{2:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
            case "0:00.000":
                return string.Format("{0:0}:{1:00}.{2:000}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
            case "#0:00.000":
                return string.Format("{0:#0}:{1:00}.{2:000}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
        }
        return "error";
    }
}

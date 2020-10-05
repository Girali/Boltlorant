using UnityEngine;
using UnityEngine.UI;
using Bolt;

public class UI_Cooldown : MonoBehaviour
{
    [SerializeField]
    private Image _fill;
    [SerializeField]
    private Text _timer;
    [SerializeField]
    private Text _shortcut;
    [SerializeField]
    private Text _cost1;
    [SerializeField]
    private Text _cost2;
    [SerializeField]
    private Image _BGS;
    [SerializeField]
    private Color _gray;
    [SerializeField]
    private Color _blue;
    [SerializeField]
    private Color _darkGray;

    float _time;
    float _cdTimer;
    bool _counting = false;

    private void Update()
    {
        if (_counting)
        {
            if (_time < BoltNetwork.ServerFrame)
            {
                _timer.text = FloatToTime(-(BoltNetwork.ServerFrame - _time) / BoltNetwork.FramesPerSecond, "00.0");
            }
            else
            {
                _counting = false;
                _shortcut.color = _blue;
                _timer.text = "READY";
                _timer.color = _blue;
                _fill.color = _blue;
            }
        }
    }

    public void InitView(float cd)
    {
        _cdTimer = cd;
    }

    public void StartCooldown()
    {
        _time = _cdTimer + BoltNetwork.ServerFrame;
        _counting = false;
        _shortcut.color = Color.black;
        _timer.text = FloatToTime(_time / BoltNetwork.FramesPerSecond, "00.0"); ;
        _timer.color = Color.white;
        _fill.color = _darkGray;
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

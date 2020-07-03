using UnityEngine;

public class Ability : MonoBehaviour
{
    protected bool _pressed = false;
    protected bool _buttonUp;
    protected bool _buttonDown;

    [SerializeField]
    protected int _cooldown = 0;
    protected float _timer = 0f;
    protected int _abilityInterval
    {
        get { return _cooldown * BoltNetwork.FramesPerSecond; }
    }

    public virtual void UpdateAbility(bool button)
    {
        _buttonUp = false;
        _buttonDown = false;
        if (button)
        {
            if (_pressed == false)
            {
                _pressed = true;
                _buttonDown = true;
            }
        }
        else
        {
            if (_pressed)
            {
                _pressed = false;
                _buttonUp = true;
            }
        }
    }

    public virtual void ShowVisualEffect() { }
}

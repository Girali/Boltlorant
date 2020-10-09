using UnityEngine;

public class Ability : Bolt.EntityEventListener<IPlayerState>
{
    protected bool _pressed = false;
    protected bool _buttonUp;
    protected bool _buttonDown;

    protected int _cooldown = 0;
    protected float _timer = 0f;
    protected int _cost = 0;

    protected UI_Cooldown _UI_cooldown;

    private void Awake()
    {
        _UI_cooldown = GUI_Controller.Current.Cooldown1;
        _UI_cooldown.InitView(_abilityInterval);
    }

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

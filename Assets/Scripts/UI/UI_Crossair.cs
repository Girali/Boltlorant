using UnityEngine;

public class UI_Crossair : MonoBehaviour
{
    [SerializeField]
    private RectTransform _up = null;
    [SerializeField]
    private RectTransform _down = null;
    [SerializeField]
    private RectTransform _right = null;
    [SerializeField]
    private RectTransform _left = null;
    [SerializeField]
    private Vector2 _limits = new Vector2(5,40);

    public void Init(Vector2 v)
    {
        _limits = v;
    }

    public void UpdateView(float t)
    {
        _up.anchoredPosition = new Vector2(0, Mathf.LerpUnclamped(_limits.x, _limits.y, t));
        _down.anchoredPosition = new Vector2(0, Mathf.LerpUnclamped(-_limits.x, -_limits.y, t));
        _right.anchoredPosition = new Vector2(Mathf.LerpUnclamped(_limits.x, _limits.y, t), 0);
        _left.anchoredPosition = new Vector2(Mathf.LerpUnclamped(-_limits.x, -_limits.y, t), 0);
    }
}

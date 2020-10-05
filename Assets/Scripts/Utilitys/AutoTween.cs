using UnityEngine;

public class AutoTween : MonoBehaviour
{
    [SerializeField]
    private Vector3 _toVector;

    void Start()
    {
        GetComponent<Jun_TweenRuntime>().firstTween.fromVector = Vector3.zero;
        GetComponent<Jun_TweenRuntime>().firstTween.toVector = _toVector;
        GetComponent<Jun_TweenRuntime>().Play();
    }
}

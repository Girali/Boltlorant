using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour
{
    [SerializeField]
    private float _decay = 2f;
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_decay);
        GameObject.Destroy(gameObject);
    }
}

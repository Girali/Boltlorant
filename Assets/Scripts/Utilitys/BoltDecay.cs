using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltDecay : MonoBehaviour
{
    [SerializeField]
    private float _decay = 60f;
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_decay);
        if (GetComponent<BoltEntity>().IsOwner)
            BoltNetwork.Destroy(gameObject);
    }
}

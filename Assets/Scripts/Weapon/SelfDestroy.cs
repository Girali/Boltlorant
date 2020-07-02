using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField]
    private float lifeDuration = 1;
    private float timer = 0f;

    private void FixedUpdate()
    {
        if (timer >= lifeDuration)
            Destroy(gameObject);
        timer += Time.deltaTime;
    }
}

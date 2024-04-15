using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour
{
    public float duration=0.5f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DestroyMe), duration);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}

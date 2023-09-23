using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    void Awake()
    {
        DisableDestroying();
    }

    public void DisableDestroying()
    {
        DontDestroyOnLoad(this);
    }
}

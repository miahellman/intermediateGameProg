using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletLifetime = 1f;

    //destory when bullet life runs out 
    private void Awake()
    {
        Destroy(gameObject, bulletLifetime);
    }
}
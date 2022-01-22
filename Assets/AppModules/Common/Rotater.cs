using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float rotateMountPerSec = 5.0f;

    void Update()
    {
        transform.rotation =  transform.rotation * Quaternion.Euler(0.0f, rotateMountPerSec * Time.deltaTime, 0.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private Vector3 _defaultEularRotation;
    void Update()
    {
        Vector2 pointOnViewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        pointOnViewport = (pointOnViewport - Vector2.one * 0.5f);
        
        pointOnViewport.x *= 10.0f;
        pointOnViewport.y *= 4.0f;

        transform.rotation = Quaternion.Euler(_defaultEularRotation + new Vector3(pointOnViewport.y, pointOnViewport.x, 0.0f));
    }
}

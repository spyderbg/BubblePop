using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSize : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        const float worldWidth = 9.0f;
        
        _camera = Camera.main;
        if (_camera != null)
            _camera.orthographicSize = Screen.height * worldWidth * 0.5f / Screen.width;
    }
}

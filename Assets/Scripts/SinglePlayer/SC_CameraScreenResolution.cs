using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CameraScreenResolution : MonoBehaviour
{
    private bool maintainWidth;
    [Range(-1,1)]
    private int adaptPosition;
    private float defaultWidth, defaultHeight;
    private Vector3 cameraPos;

    void Awake(){Init();}

    /// <summary>
    /// Initialization of required variables for proper in-game camera behavior
    /// </summary>
    void Init()
    {
        maintainWidth = true;
        cameraPos = Camera.main.transform.position;
        defaultHeight = Camera.main.orthographicSize;
        defaultWidth = Camera.main.orthographicSize * Camera.main.aspect;
    }


    /// <summary>
    /// Changing camera orthographic size based on the current game window size and repositioning the camera.
    /// </summary>
	void Update ()
    {
        if (maintainWidth)
        {
            Camera.main.orthographicSize = defaultWidth / Camera.main.aspect;
            Camera.main.transform.position = new Vector3(cameraPos.x, adaptPosition * (defaultHeight - Camera.main.orthographicSize), cameraPos.z);
        }
        else
            Camera.main.transform.position = new Vector3(adaptPosition * (defaultWidth - Camera.main.orthographicSize * Camera.main.aspect), cameraPos.y, cameraPos.z);
	}
}

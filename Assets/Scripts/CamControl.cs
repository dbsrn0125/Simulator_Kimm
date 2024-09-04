using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera overheadCamera;
    bool cam_check = false;
    public float width;
    public float height;

    public void Awake()
    {
        firstPersonCamera.enabled = true;
        overheadCamera.enabled = true;
        Showfirstperson();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V)&& cam_check ==false)
        {
            Showfirstperson();
            cam_check = true;
        }
        else if(Input.GetKeyDown(KeyCode.V) && cam_check == true)
        {
            ShowOverhead();
            cam_check = false;
        }
    }

    void Showfirstperson()
    {
        overheadCamera.rect = new Rect(width, width, height, height);
        firstPersonCamera.rect = new Rect(0, 0, 1, 1);
        overheadCamera.depth = 1;
        firstPersonCamera.depth = -1;
    }
    void ShowOverhead()
    {
        firstPersonCamera.rect = new Rect(width, width, height, height);
        overheadCamera.rect = new Rect(0, 0, 1, 1);
        firstPersonCamera.depth = 1;
        overheadCamera.depth = -1;
    }
}

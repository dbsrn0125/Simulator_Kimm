using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//사용자의 마우스 입력에 따라 물체를 상하좌우로 회전시키고 싶다
public class CamRotate : MonoBehaviour
{
    //필요 속성 : 회전속도
    public float rotSpeed = 205;
    //우리가 직접 각도를 관리하자
    float mx;
    float my;
    // Start is called before the first frame update
    void Start()
    {
        //생성직후
        //시작할 때 사용자가 정해둔 각도 값으로 세팅
        mx = transform.eulerAngles.y;
        my = -transform.eulerAngles.x;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.gameObject != gameObject)
            {
                cam.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //생성이후
        //사용자의 마우스 입력에 따라 물체를 상하좌우로 회전시키고 싶다.
        //1. 사용자의 입력에 따라
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        mx += h * rotSpeed * Time.deltaTime;
        my += v * rotSpeed * Time.deltaTime;
        //-60~60으로 각도 제한걸기
        //x축 -> pitch, y축-> yaw, z축 -> Roll
        my = Mathf.Clamp(my, -60, 60);
        transform.eulerAngles = new Vector3(-my, mx,0);
        //2. 방향이 필요하다.
        //Vector3 dir = new Vector3(-v, h, 0);

        //3. 회전하고 싶다.
        //transform.eulerAngles += dir * rotSpeed * Time.deltaTime;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

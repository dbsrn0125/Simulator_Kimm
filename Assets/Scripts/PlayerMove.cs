using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 사용자 입력에 따라 앞뒤좌우로 이동하고 싶다.
// characterController를 이용해 이동시키고 싶다.
// 중력을 적용시키고 싶다.
public class PlayerMove : MonoBehaviour
{
// 필요속성 : 이동속도
    public float speed = 5;
    // 필요속성 : character Controller
    CharacterController cc;
    // 필요속성 : 중력값, 수직속도
    public float gravity = -9.81f;

    //float yVelocity = 0;
    
    void Start()
    {
        //cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // 사용자 입력에 따라 앞뒤좌우로 이동하고 싶다.
        //1. 사용자의 입력에 따라
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //2. 방향이 필요
        Vector3 dir = new Vector3(h, 0, v);
        //->내가 바라보는 방향을 기준으로 가고싶다.
        dir = Camera.main.transform.TransformDirection(dir);

        //v=v0 + at
        //수직속도 구하기
        //dir *= speed;
        //yVelocity += gravity * Time.deltaTime;
        //만약 바닥에 닿아있다면
        //if(cc.collisionFlags == CollisionFlags.Below)
        //{
        //    //->수직속도를 0으로 하고 싶다.
        //    yVelocity = 0;
        //}
        //dir.y = yVelocity;
        //3. 이동하고 싶다
        //P = P0 + vt
        
        //cc.Move(dir * speed * Time.deltaTime);
        transform.position += dir * speed * Time.deltaTime;
    }

}
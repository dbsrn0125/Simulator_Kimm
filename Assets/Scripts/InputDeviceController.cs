using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceController : MonoBehaviour
{
    private CarInputActions carInputActions;
    public enum Gear { Neutral, Forward, Reverse}
    public Gear currentGear = Gear.Neutral;

    public float steeringInput =0f;
    public float throttleInput=0f;
    public float brakeInput=0f;

    private float targetSteeringInput = 0f; // 목표 스티어링 입력
    private float targetThrottleInput = 0f; // 목표 가속 입력
    private float targetBrakeInput = 0f; // 목표 브레이크 입력

    public float steeringSpeed = 5f; // 조작 변화 속도
    public float throttleSpeed = 2f; // 가속 변화 속도
    public float brakeSpeed = 2f; // 브레이크 변화 속도
    private void Awake()
    {
        carInputActions = new CarInputActions();
    }
    private void OnEnable()
    {
        carInputActions.Enable();
    }
    private void OnDisable()
    {
        carInputActions.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Current Gear : {currentGear}");
    }

    // Update is called once per frame
    void Update()
    {
        bool isGamepadConnected = Gamepad.current!=null;
        //Debug.Log(isGamepadConnected);
        if(isGamepadConnected)
        {
            steeringInput = carInputActions.Car.Steering.ReadValue<Vector2>().x;
            throttleInput = carInputActions.Car.Throttle.ReadValue<float>();
            brakeInput = carInputActions.Car.Brake.ReadValue<float>();
        }
        else
        {
            // 키보드 입력을 사용
            // 목표 입력 값을 설정
            if (Keyboard.current.aKey.isPressed)
            {
                targetSteeringInput = -1f; // 왼쪽
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                targetSteeringInput = 1f; // 오른쪽
            }
            else
            {
                targetSteeringInput = 0f; // 중립
            }

            if (Keyboard.current.wKey.isPressed)
            {
                targetThrottleInput = 1f; // 가속
            }
            else
            {
                targetThrottleInput = 0f; // 가속 중지
            }

            if (Keyboard.current.sKey.isPressed)
            {
                targetBrakeInput = 1f; // 브레이크
            }
            else
            {
                targetBrakeInput = 0f; // 브레이크 해제
            }

            // 부드럽게 변화시키기
            steeringInput = Mathf.Lerp(steeringInput, targetSteeringInput, steeringSpeed * Time.deltaTime);
            throttleInput = Mathf.Lerp(throttleInput, targetThrottleInput, throttleSpeed * Time.deltaTime);
            brakeInput = Mathf.Lerp(brakeInput, targetBrakeInput, brakeSpeed * Time.deltaTime);

            // 입력 값이 목표 값에 가까워지면 0으로 설정
            if (Mathf.Abs(targetSteeringInput) < 0.01f && Mathf.Abs(steeringInput) < 0.01f)
            {
                steeringInput = 0f;
            }
            if (Mathf.Abs(targetThrottleInput) < 0.01f && Mathf.Abs(throttleInput) < 0.01f)
            {
                throttleInput = 0f;
            }
            if (Mathf.Abs(targetBrakeInput) < 0.01f && Mathf.Abs(brakeInput) < 0.01f)
            {
                brakeInput = 0f;
            }
        }
        // Gear Input: 전진 및 후진 확인
        bool isGearForward = isGamepadConnected ? carInputActions.Car.GearForward.triggered : Keyboard.current.leftCtrlKey.wasPressedThisFrame;
        bool isGearReverse = isGamepadConnected ? carInputActions.Car.GearReverse.triggered : Keyboard.current.leftShiftKey.wasPressedThisFrame;

        //Debug.Log($"Steering Input: {steeringInput}");
        //Debug.Log($"Throttle Input: {throttleInput}");
        //Debug.Log($"Brake Input: {brakeInput}");
        if (isGearForward && currentGear != Gear.Forward)
        {
            currentGear = Gear.Forward;
            Debug.Log("Gear Shifted to Forward");
        }
        else if (isGearReverse && currentGear != Gear.Reverse)
        {
            currentGear = Gear.Reverse;
            Debug.Log("Gear Shifted to Reverse");
        }
    }
}

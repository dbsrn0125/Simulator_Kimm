using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceController : MonoBehaviour
{
<<<<<<< Updated upstream
    private CarInputActions carInputActions;
    private CarCmd_ros autoCmd;
=======
    private InputActions inputActions;
    private CarCmdRos autoCmd;
>>>>>>> Stashed changes

    public enum Gear { Neutral, Forward, Reverse}
    public Gear currentGear = Gear.Neutral;

    public float steeringInput =0f;
    public float throttleInput=0f;
    public float brakeInput=0f;
    public float gearInput = 0f;

    private float targetSteeringInput = 0f; // ��ǥ ��Ƽ� �Է�
    private float targetThrottleInput = 0f; // ��ǥ ���� �Է�
    private float targetBrakeInput = 0f; // ��ǥ �극��ũ �Է�

    public bool is_auto = false;

    public float steeringSpeed = 5f; // ���� ��ȭ �ӵ�
    public float throttleSpeed = 5f; // ���� ��ȭ �ӵ�
    public float brakeSpeed = 5f; // �극��ũ ��ȭ �ӵ�
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
        autoCmd = transform.GetComponent<CarCmdRos>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isGamepadConnected = Gamepad.current!=null;
        
        // auto driving toggle
        if(isGamepadConnected)
        {
            // steering wheel 버튼으로 자율주행/메뉴얼 토글
        }
        else
        {
            if (Keyboard.current.bKey.isPressed)
            {
                is_auto = true;
            }
            else if (Keyboard.current.nKey.isPressed)
            {
                is_auto = false;
            }
        }
        
        if (is_auto){
            steeringInput = autoCmd.steering_angle;
            throttleInput = autoCmd.throttle;
            brakeInput = autoCmd.brake;
            
            if (autoCmd.gear == 0){
                currentGear = Gear.Forward;
                gearInput = 0;
            }
            else{
                currentGear = Gear.Reverse;
                gearInput = 1;
            }
        }

        else{
            if(isGamepadConnected)
            {
                steeringInput = carInputActions.Car.Steering.ReadValue<Vector2>().x;
                throttleInput = carInputActions.Car.Throttle.ReadValue<float>();
                brakeInput = carInputActions.Car.Brake.ReadValue<float>();
            }
            else
            {
                // steering input
                if (Keyboard.current.aKey.isPressed)
                {
                    targetSteeringInput = -1f; // ����
                }
                else if (Keyboard.current.dKey.isPressed)
                {
                    targetSteeringInput = 1f; // ������
                }
                else
                {
                    targetSteeringInput = 0f; // �߸�
                }

                // throttle
                if (Keyboard.current.wKey.isPressed)
                {
                    targetThrottleInput = 1f; // ����
                }
                else
                {
                    targetThrottleInput = 0f; // ���� ����
                }

                // brake
                if (Keyboard.current.sKey.isPressed)
                {
                    targetBrakeInput = 1f; // �극��ũ
                }
                else
                {
                    targetBrakeInput = 0f; // �극��ũ ����
                }

                steeringInput = Mathf.Lerp(steeringInput, targetSteeringInput, steeringSpeed * Time.deltaTime);
                throttleInput = Mathf.Lerp(throttleInput, targetThrottleInput, throttleSpeed * Time.deltaTime);
                brakeInput = Mathf.Lerp(brakeInput, targetBrakeInput, brakeSpeed * Time.deltaTime);

                // �Է� ���� ��ǥ ���� ��������� 0���� ����
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
            // Gear Input: ���� �� ���� Ȯ��
            bool isGearForward = isGamepadConnected ? carInputActions.Car.GearForward.triggered : Keyboard.current.leftShiftKey.wasPressedThisFrame;
            bool isGearReverse = isGamepadConnected ? carInputActions.Car.GearReverse.triggered : Keyboard.current.leftCtrlKey.wasPressedThisFrame;

            //Debug.Log($"Steering Input: {steeringInput}");
            //Debug.Log($"Throttle Input: {throttleInput}");
            //Debug.Log($"Brake Input: {brakeInput}");
            if (isGearForward && currentGear != Gear.Forward)
            {
                currentGear = Gear.Forward;
                gearInput = 0;
                Debug.Log("Gear Shifted to Forward");
            }
            else if (isGearReverse && currentGear != Gear.Reverse)
            {
                currentGear = Gear.Reverse;
                gearInput = 1;
                Debug.Log("Gear Shifted to Reverse");
            }
        }
    }
}

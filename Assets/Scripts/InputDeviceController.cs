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

    private float targetSteeringInput = 0f; // ��ǥ ��Ƽ� �Է�
    private float targetThrottleInput = 0f; // ��ǥ ���� �Է�
    private float targetBrakeInput = 0f; // ��ǥ �극��ũ �Է�

    public float steeringSpeed = 5f; // ���� ��ȭ �ӵ�
    public float throttleSpeed = 2f; // ���� ��ȭ �ӵ�
    public float brakeSpeed = 2f; // �극��ũ ��ȭ �ӵ�
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
            // Ű���� �Է��� ���
            // ��ǥ �Է� ���� ����
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

            if (Keyboard.current.wKey.isPressed)
            {
                targetThrottleInput = 1f; // ����
            }
            else
            {
                targetThrottleInput = 0f; // ���� ����
            }

            if (Keyboard.current.sKey.isPressed)
            {
                targetBrakeInput = 1f; // �극��ũ
            }
            else
            {
                targetBrakeInput = 0f; // �극��ũ ����
            }

            // �ε巴�� ��ȭ��Ű��
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

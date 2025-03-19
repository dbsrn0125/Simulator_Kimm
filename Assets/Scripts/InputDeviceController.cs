using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceController : MonoBehaviour
{
    private InputActions inputActions;
    private CarCmdRos autoCmd;

    public enum Gear { Neutral, Forward, Reverse}
    public Gear currentGear = Gear.Forward;

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
        inputActions = InputManager.Instance.InputActions;
    }

    private void OnEnable()
    {
        inputActions.Car.Enable();
        inputActions.Car.ResetCar.performed += ResetCar;
        inputActions.Car.Auto.performed += ChangeDriveMode;
        inputActions.Car.GearForward.performed += OnGearForward;
        inputActions.Car.GearReverse.performed += OnGearReverse;

    }
    private void OnDisable()
    {
        inputActions.Car.Disable();
        inputActions.Car.ResetCar.performed -= ResetCar;
        inputActions.Car.Auto.performed -= ChangeDriveMode;
        inputActions.Car.GearForward.performed -= OnGearForward;
        inputActions.Car.GearReverse.performed -= OnGearReverse;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Current Gear : {currentGear}");
        autoCmd = transform.GetComponent<CarCmdRos>();
    }
    private void ResetCar(InputAction.CallbackContext context)
    {
        transform.GetComponent<CarController>().ResetCarPosition();
    }
    private void ChangeDriveMode(InputAction.CallbackContext context)
    {
        is_auto = !is_auto;
    }
    private void OnGearForward(InputAction.CallbackContext context)
    {
        if (currentGear != Gear.Forward)
        {
            currentGear = Gear.Forward;
            gearInput = 0;
            Debug.Log("Gear Shifted to Forward");
        }
    }
    private void OnGearReverse(InputAction.CallbackContext context)
    {
        if (currentGear != Gear.Reverse)
        {
            currentGear = Gear.Reverse;
            gearInput = 1;
            Debug.Log("Gear Shifted to Reverse");
        }
    }
    // Update is called once per frame
    void Update()
    {
        bool isGamepadConnected = Gamepad.current!=null;
        
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
                steeringInput = inputActions.Car.Steering.ReadValue<Vector2>().x;
                throttleInput = inputActions.Car.Throttle.ReadValue<float>();
                brakeInput = inputActions.Car.Brake.ReadValue<float>();
            }
            else
            {
                // steering input
                if (Keyboard.current.aKey.isPressed)
                {
                    targetSteeringInput = -1f;
                }
                else if (Keyboard.current.dKey.isPressed)
                {
                    targetSteeringInput = 1f;
                }
                else
                {
                    targetSteeringInput = 0f;
                }

                // throttle
                if (Keyboard.current.wKey.isPressed)
                {
                    targetThrottleInput = 1f;
                }
                else
                {
                    targetThrottleInput = 0f;
                }

                // brake
                if (Keyboard.current.sKey.isPressed)
                {
                    targetBrakeInput = 1f;
                }
                else
                {
                    targetBrakeInput = 0f;
                }

                steeringInput = Mathf.Lerp(steeringInput, targetSteeringInput, steeringSpeed * Time.deltaTime);
                throttleInput = Mathf.Lerp(throttleInput, targetThrottleInput, throttleSpeed * Time.deltaTime);
                brakeInput = Mathf.Lerp(brakeInput, targetBrakeInput, brakeSpeed * Time.deltaTime);

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
            //bool isGearForward = isGamepadConnected ? inputActions.Car.GearForward.triggered : Keyboard.current.leftShiftKey.wasPressedThisFrame;
            //bool isGearReverse = isGamepadConnected ? inputActions.Car.GearReverse.triggered : Keyboard.current.leftCtrlKey.wasPressedThisFrame;

            //if (isGearForward && currentGear != Gear.Forward)
            //{
            //    currentGear = Gear.Forward;
            //    gearInput = 0;
            //    Debug.Log("Gear Shifted to Forward");
            //}
            //else if (isGearReverse && currentGear != Gear.Reverse)
            //{
            //    currentGear = Gear.Reverse;
            //    gearInput = 1;
            //    Debug.Log("Gear Shifted to Reverse");
            //}
        }
    }
}

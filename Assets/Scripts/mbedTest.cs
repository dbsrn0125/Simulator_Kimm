using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
public class mbedTest : MonoBehaviour
{
    public Button toggleButton;
    private bool ledState = false;
    private SerialPort serialPort;

    private string portName = "COM7";
    private int baudRate = 115200;
    // Start is called before the first frame update
    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error opening serial port:" + e.Message);
        }
        toggleButton.onClick.AddListener(ToggleLED);
    }

    void ToggleLED()
    {
        ledState = !ledState;
        SendDataToBluetooth(ledState);
    }

    void SendDataToBluetooth(bool state)
    {
        string message = state ? "1" : "0"; // 전송할 데이터
        if (serialPort.IsOpen)
        {
            serialPort.WriteLine(message); // 데이터 전송
            Debug.Log("Sending: " + message); // 콘솔에 로그 출력
        }
        else
        {
            Debug.LogError("Serial port not open!"); // 연결 오류 메시지
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close(); // 애플리케이션 종료 시 포트 닫기
            Debug.Log("Serial port closed");
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}

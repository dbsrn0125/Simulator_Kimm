using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;

public class ButtonToggle : MonoBehaviour
{
    public Button toggleButton;
    private bool toggleState = false;
    private SerialPort serialPort;

    void Start()
    {
        string portName = "COM7"; // 실제 포트 이름으로 변경
        serialPort = new SerialPort(portName, 9600);

        // 포트가 열려 있는지 확인
        if (!serialPort.IsOpen)
        {
            try
            {
                serialPort.Open();
                toggleButton.onClick.AddListener(OnButtonClick);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.LogError("포트에 접근할 수 없습니다. 포트가 사용 중인지 확인하세요.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("오류 발생: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("포트가 이미 열려 있습니다.");
        }
    }

    void OnButtonClick()
    {
        toggleState = !toggleState;
        SendToggleState(toggleState);
    }

    void SendToggleState(bool state)
    {
        if (serialPort.IsOpen)
        {
            serialPort.Write(state ? "1" : "0");
        }
        else
        {
            Debug.LogError("포트가 열려 있지 않습니다.");
        }
    }

    private void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}

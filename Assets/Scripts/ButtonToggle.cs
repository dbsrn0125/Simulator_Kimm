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
        string portName = "COM7"; // ���� ��Ʈ �̸����� ����
        serialPort = new SerialPort(portName, 9600);

        // ��Ʈ�� ���� �ִ��� Ȯ��
        if (!serialPort.IsOpen)
        {
            try
            {
                serialPort.Open();
                toggleButton.onClick.AddListener(OnButtonClick);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.LogError("��Ʈ�� ������ �� �����ϴ�. ��Ʈ�� ��� ������ Ȯ���ϼ���.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("���� �߻�: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("��Ʈ�� �̹� ���� �ֽ��ϴ�.");
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
            Debug.LogError("��Ʈ�� ���� ���� �ʽ��ϴ�.");
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

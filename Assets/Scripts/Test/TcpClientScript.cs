using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TcpClientScript : MonoBehaviour
{
    public Slider valueSlider;
    private TcpClient client;
    private NetworkStream stream;

    async void Start()
    {
        try
        {
            client = new TcpClient();
            await ConnectToServer();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to connect to server: {e.Message}");
        }

        // Add listener to the slider
        valueSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private async Task ConnectToServer()
    {
        await Task.Run(async () =>
        {
            await client.ConnectAsync("127.0.0.1", 5000);
            stream = client.GetStream();
            Debug.Log("Connected to server");
        });
    }

    private async void OnSliderValueChanged(float value)
    {
        if (stream == null)
        {
            Debug.LogError("Not connected to server");
            return;
        }

        string message = value.ToString("F2");
        byte[] data = Encoding.UTF8.GetBytes(message);

        try
        {
            await stream.WriteAsync(data, 0, data.Length);
            Debug.Log("Sent: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send data: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
public class SliderController : MonoBehaviour
{
    public Slider slider;
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private const string ipAddress = "127.0.0.1";
    private const int port = 9876;
    // Start is called before the first frame update
    void Start()
    {
        slider.minValue = -10;
        slider.maxValue = 10;
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }
    void OnSliderValueChanged(float value)
    {
        string message = value.ToString();
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, remoteEndPoint);
        Debug.Log("Sent: " + message);
    }
    void OnDestroy()
    {
        udpClient.Close();
    }
}

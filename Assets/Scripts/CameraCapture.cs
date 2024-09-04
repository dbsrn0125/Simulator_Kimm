using UnityEngine;
using System.Net.Sockets;
using System.IO;

public class bevCam2 : MonoBehaviour
{
    public Camera camera1;
    private Texture2D screenTexture;
    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        if (camera1 == null)
        {
            camera1 = Camera.main; // 메인 카메라를 자동으로 할당
        }
        screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ConnectToServer("127.0.0.1", 12345); // 서버 IP와 포트 설정
    }

    void Update()
    {
        Capture();
    }

    void Capture()
    {
        RenderTexture renderTex = new RenderTexture(Screen.width, Screen.height, 24);
        camera1.targetTexture = renderTex;
        camera1.Render();
        RenderTexture.active = renderTex;
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();
        camera1.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTex);

        SendImageToServer(screenTexture);
    }

    void ConnectToServer(string ip, int port)
    {
        try
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
        }
        catch (SocketException ex)
        {
            Debug.LogError($"서버에 연결하는 동안 오류 발생: {ex.Message}");
        }
    }

    void SendImageToServer(Texture2D texture)
    {
        if (stream == null)
        {
            Debug.LogError("전송할 수 없습니다. 스트림이 null입니다.");
            return;
        }

        byte[] imageBytes = texture.EncodeToPNG();
        byte[] lengthPrefix = System.BitConverter.GetBytes(imageBytes.Length);
        stream.Write(lengthPrefix, 0, lengthPrefix.Length); // 이미지 크기 전송
        stream.Write(imageBytes, 0, imageBytes.Length); // 이미지 데이터 전송
    }

    void OnApplicationQuit()
    {
        if (stream != null)
        {
            stream.Close();
        }
        if (client != null)
        {
            client.Close();
        }
    }
}

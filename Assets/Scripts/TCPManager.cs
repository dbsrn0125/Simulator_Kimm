using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;

public class TCPManager
{
    byte[] rB = new byte[1000]; // 버퍼의 크기 필요한 만큼 크기를 정하도록 하자 작을수록 좋음
    Thread ServerCheck_thread; // 서버에서 보내는 패킷을 체크하기 위한 스레드
    Thread CheckforClient_thread;
    Queue<string> netBuffer = new Queue<string>(); // 버퍼를 저장하기 위한 큐
    string strIP = "127.0.0.1"; // 서버 아이피를 적자, <127.0.0.1  은 자기자신의 컴퓨터 ip를 지칭>
    public bool isServerOn = false;

    int port_listen, port_target;  // 접속 할 서버의 포트를 적자
    TcpClient client;
    public NetworkStream stream;
    IPAddress ip;
    IPEndPoint endPoint_listen, endPoint_target;

    object buffer_lock = new object(); // queue 처리 충돌 방지용 lock

    public delegate void ReceiveCallback(string result);

    ReceiveCallback callback;

    public TCPManager(string strIP_, int port_listen_, int port_target_, MonoBehaviour target)
    {
        strIP = strIP_;
        port_listen = port_listen_;
        port_target = port_target_;
        serverOn();
        target.StartCoroutine(buffer_update());
    }

    public TCPManager(string strIP_, int port_listen_, int port_target_, MonoBehaviour target, ReceiveCallback callback_)
    {
        strIP = strIP_;
        port_listen = port_listen_;
        port_target = port_target_;
        callback = callback_;
        serverOn();
        target.StartCoroutine(buffer_update());
    }

    public void OnQuit() // 유니티가 종료될 때
    {
        ServerCheck_thread?.Abort(); // 스레드를 종료시킨다,  만약 스레드를 정상적으로 종료시키지않을경우  유니티가 멈춘다
        CheckforClient_thread?.Abort();
        client.Close(); // 소켓을 닫는다.
        Debug.Log("TCP Closed");
    }

    IEnumerator buffer_update()
    {
        while (true)
        {
            yield return null; // 코루틴에서 반복문을 쓸수있게 해준다. 없으면 유니티 멈춤
            BufferSystem();
        }
    }

    void serverOn()
    {
        //sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ip = IPAddress.Parse(strIP);
        endPoint_listen = new IPEndPoint(ip, port_listen);
        endPoint_target = new IPEndPoint(ip, port_target);
        client = new TcpClient(endPoint_listen);
        ServerCheck_thread = new Thread(ServerCheck);
        CheckforClient_thread = new Thread(CheckforClient);
        CheckforClient_thread.Start();
    }

    void CheckforClient()
    {
        int count = 0;

        while (count <= 10)
        {
            try
            {
                client.Connect(endPoint_target);
            }
            catch
            {
                count++;
                Thread.Sleep(10);
            }
        }
        if (client.Connected)
        {
            stream = client.GetStream();
            ServerCheck_thread.Start(); //서버 체크 시스템 시작, 스레드를 사용하지않으면 서버체크 Receive 에서 유니티가 멈춰버림 중요!
            isServerOn = true;
            Debug.Log("윈폼 접속 성공");
        }
        else
        {
            Debug.Log("윈폼 서버 접속 실패");
            return;
        }
    }

    void ServerCheck()
    {
        while (true)
        {
            try
            {
                stream.Read(rB, 0, rB.Length); //서버에서 온 패킷을 버퍼에 담기

                string t = Encoding.Default.GetString(rB);  // 큐에 버퍼를 넣을 준비
                t = t.Replace("\0", string.Empty); // 버퍼 마지막에 공백이 있는지 검색하고 공백을 삭제
                lock (buffer_lock) // queue 충돌 방지
                {
                    netBuffer.Enqueue(t); // 큐에 버퍼를 저장한다
                }

                Array.Clear(rB, 0, rB.Length);  // 버퍼를 사용후 초기화 시키도록 하자 , 이걸 안하면 이전 패킷량보다 다음 패킷량이 적을 경우  다음패킷값이 이상하게 나옴
                //Thread.Sleep(0);
            }
            catch { } // try - catch 로 정해놓은 버퍼의 크기 이상의 패킷량이 왔을경우를 대비
        }
    }

    void BufferSystem()
    {
        while (netBuffer.Count != 0) // 큐의 크기가 0이 아니면 작동,   만약 while를 안하면 프레임마다 버퍼를 처리 하는데 많은 패킷을 처리할때는 처리 되는 량보다 쌓이는 량이 많아져 작동이 제대로 이루어지지않음
        {
            string b = null;
            lock (buffer_lock)
            {
                b = netBuffer.Dequeue();// 큐에 담겨있는 버퍼를 스트링에 넣고 사용하기
            }

            // Debug.Log("server ->" + b);
            callback?.Invoke(b);
        }
    }

    // 서버로 패킷 전송
    public void ServerSend(string str)
    {
        byte[] sbuff = Encoding.UTF8.GetBytes(str); // 보낼 정보를 byte로 바꾼다
        // client.Send(sbuff, 0, sbuff.Length, SocketFlags.None);  // 서버로 전송
        stream.Write(sbuff, 0, sbuff.Length);
    }

    public void ServerSend(byte[] bytes)
    {
        //byte[] sbuff = Encoding.UTF8.GetBytes(str); // 보낼 정보를 byte로 바꾼다
        //client.Send(sbuff, 0, sbuff.Length, SocketFlags.None);  // 서버로 전송
        stream.Write(bytes, 0, bytes.Length);
    }

}

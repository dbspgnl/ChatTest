using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;


public class Client : MonoBehaviour
{
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    [SerializeField] private InputField HostInput;
    [SerializeField] private InputField PortInput;
    [SerializeField] private string host = "127.0.0.1";
    [SerializeField] private int port = 6321;

    private void Awake()
    {
        HostInput.text = host;
        PortInput.text = port.ToString();
    }


    public void ConnectToserver()
    {
        // 만약 이미 연결되었다면 이 기능은 무시
        if (socketReady)
        {
            return;
        }

        // 디폴트 호스트와 포트 
       

        // 해당 상자에 무언가가 있는 경우 기본 호스트/포트 값을 덮어씁니다.
        string h = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (string.IsNullOrEmpty(h))
            host = h;

        var portInput = GameObject.Find("PortInput").GetComponent<InputField>().text; 
        if (int.TryParse(portInput, out var p))  
            port = p;

        // 소켓 생성
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }



    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }
    }

    private void OnIncomingData(string data)
    {
        Debug.Log("Server : " + data);
    }
}

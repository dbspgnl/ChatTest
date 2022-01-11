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
        // ���� �̹� ����Ǿ��ٸ� �� ����� ����
        if (socketReady)
        {
            return;
        }

        // ����Ʈ ȣ��Ʈ�� ��Ʈ 
       

        // �ش� ���ڿ� ���𰡰� �ִ� ��� �⺻ ȣ��Ʈ/��Ʈ ���� ����ϴ�.
        string h = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (string.IsNullOrEmpty(h))
            host = h;

        var portInput = GameObject.Find("PortInput").GetComponent<InputField>().text; 
        if (int.TryParse(portInput, out var p))  
            port = p;

        // ���� ����
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

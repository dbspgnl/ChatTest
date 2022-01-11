using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    public int port = 6321;
    private TcpListener server;
    private bool serverStarted;

    private void Start()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            startListening();
            serverStarted = true;
            Debug.Log("Server has been started on port " + port);
        }
        catch(Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    private void Update()
    {
        if (!serverStarted) return;

        foreach(ServerClient c in clients)
        {
            // ������ Ŭ���̾�Ʈ�� ������?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            // �ش� Ŭ���̾�Ʈ�� �޽����� Ȯ��
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if(data != null)
                    {
                        OnIncomingData(c, data);
                    }

                }
            }
        }
    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if(c !=null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else return false;
        }
        catch
        {
            return false;
        }

    }

    private void startListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        startListening();
        // ��ο��� �޼����� ������, ������ �����ߴٰ� ���ϱ�
        Broadcast(clients[clients.Count - 1].clientName + " has connected", clients);

    }

    private void OnIncomingData(ServerClient c, string data)
    {
        Debug.Log(c.clientName + " has sent the follwing message : " + data);
    }
    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Write error : " + e.Message + " to client " + c.clientName);
                throw;
            }
        }
    }


}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}

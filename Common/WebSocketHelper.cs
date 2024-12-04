using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;
using Unity.VisualScripting;
using System.Collections.Concurrent;
using Message;
using Google.Protobuf;
public class WebSocketSingletonHelper : Singleton<WebSocketSingletonHelper>, IDisposable
{
    ClientWebSocket ws;
    public WebSocketState? State { get => ws?.State; }
    public string socket = "ws://115.29.206.36:30590/websocket";
    public LinkedList<byte> StraemData {
        get { return streamData; }
    }
    public ClientWebSocket Ws => ws;

    public Action closeCallBack;

    CancellationToken cancelToken;
    public CancellationToken CancelToken
    {
        get
        {
            return cancelToken;
        }
    }

    public async Task<ClientWebSocket> CreatWebSocket()
    {
        isUserClose = false;
        ws = new ClientWebSocket();
        try
        {
            await ws.ConnectAsync(new(socket), CancellationToken.None);
            reciveCancel = new CancellationTokenSource();
            receivingThread = Task.Run(ReceiveMessage, reciveCancel.Token);
            StartSenderLoop();
        }
        catch (Exception ex)
        {
            CustomLog.LogError("Creat WebSocket Error:" + ex);
        }
        // ������Ϣ�������¼�
        return ws;
    }

    public void TestHeartbeat(int pingId)
    {
        PingCmd pingCmd = new PingCmd();
        pingCmd.PingId = pingId;
        PresssureMessage(pingCmd.ToByteArray());
    }


    LinkedList<byte> streamData = new LinkedList<byte>();
    CancellationTokenSource reciveCancel;
    CancellationTokenSource sendCancel;
    public async Task<ClientWebSocket> CreatWebSocket(string socket)
    {
        isUserClose = false;
        ws = new ClientWebSocket();
        try
        {
            await ws.ConnectAsync(new(socket), CancellationToken.None);
            reciveCancel = new CancellationTokenSource();
            receivingThread = Task.Run(ReceiveMessage, reciveCancel.Token);
            StartSenderLoop();
        }
        catch (Exception ex)
        {
            CustomLog.LogError("Creat WebSocket Error:" + ex);
        }
        // ������Ϣ�������¼�
        return ws;
    }
    Task receivingThread;


    bool isUserClose;

    public async Task ReceiveMessage()
    {
        while (true)
        {
            Memory<byte> cahce = new Memory<byte>(new byte[65535]);
            ValueWebSocketReceiveResult result;
            try
            {
                result = await ws.ReceiveAsync(cahce, CancellationToken.None);
                CustomLog.Log("ResultMessageResult:" + result.Count);
            }
            catch (Exception ex)
            {
                CustomLog.LogError(ex.ToString());
                break;
            }
            if (result.MessageType != WebSocketMessageType.Close)
            {
                streamData.AddRange(cahce.Slice(0, result.Count).ToArray());
            }
            else
            {
                CustomLog.LogError("���յ�close��Ϣ");
                closeCallBack?.Invoke();
            }
        }
    }

    ConcurrentQueue<byte[]> messageQueue = new ConcurrentQueue<byte[]>();
    public void PresssureMessage(byte[] data)
    {
        messageQueue.Enqueue(data);
    }


    public void StartSenderLoop()
    {
        sendCancel = new CancellationTokenSource();
        Task.Run(() =>
        {
            while (true)
            {
                byte[] data;
                if (messageQueue.TryDequeue(out data))
                {
                    bool result =  Send(data);
                }
            }
        }, sendCancel.Token);
    }

    // <summary>
    /// ʹ�����ӷ����ı���Ϣ
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="mess"></param>
    /// <returns>�Ƿ����˷���</returns>
    bool Send(string mess)
    {
        if (ws.State != WebSocketState.Open)
            return false;

        Task.Run(async () =>
        {
            var replyMess = Encoding.UTF8.GetBytes(mess);
            //������Ϣ
            await ws.SendAsync(new ArraySegment<byte>(replyMess), WebSocketMessageType.Text, true, CancellationToken.None);
        });
        return true;
    }


    /// <summary>
    /// ʹ�����ӷ����ֽ���Ϣ
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="mess"></param>
    /// <returns>�Ƿ����˷���</returns>
    public bool Send(byte[] bytes)
    {
        if (ws.State != WebSocketState.Open)
            return false;
        Task.Run(async () =>
        {
            //������Ϣ
            try
            {
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                CustomLog.LogError("SendError:" + ex.ToString());
            }
        });
        return true;
    }

    /// <summary>
    /// �ر�����
    /// </summary>
    public void Close()
    {
        isUserClose = true;
        reciveCancel.Cancel();
        sendCancel.Cancel();
        Close(WebSocketCloseStatus.NormalClosure, "�û��ֶ��ر�");

    }

    public void Close(WebSocketCloseStatus closeStatus, string statusDescription)
    {
        Task.Run(async () =>
        {
            try
            {
                //�ر�WebSocket���ͻ��˷���
                await ws.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
            }
            catch (Exception ex)
            {
                CustomLog.LogError("CloseException:" + ex.ToString());
            }
            ws.Abort();
            ws.Dispose();
            //if (OnClose != null)
            //    OnClose(ws, new EventArgs());
        });
    }

    public void Dispose()
    {
        reciveCancel.Cancel();
        sendCancel.Cancel();
        Close();
    }


}

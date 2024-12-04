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
        // 监听消息和连接事件
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
        // 监听消息和连接事件
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
                CustomLog.LogError("接收到close信息");
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
    /// 使用连接发送文本消息
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="mess"></param>
    /// <returns>是否尝试了发送</returns>
    bool Send(string mess)
    {
        if (ws.State != WebSocketState.Open)
            return false;

        Task.Run(async () =>
        {
            var replyMess = Encoding.UTF8.GetBytes(mess);
            //发送消息
            await ws.SendAsync(new ArraySegment<byte>(replyMess), WebSocketMessageType.Text, true, CancellationToken.None);
        });
        return true;
    }


    /// <summary>
    /// 使用连接发送字节消息
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="mess"></param>
    /// <returns>是否尝试了发送</returns>
    public bool Send(byte[] bytes)
    {
        if (ws.State != WebSocketState.Open)
            return false;
        Task.Run(async () =>
        {
            //发送消息
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
    /// 关闭连接
    /// </summary>
    public void Close()
    {
        isUserClose = true;
        reciveCancel.Cancel();
        sendCancel.Cancel();
        Close(WebSocketCloseStatus.NormalClosure, "用户手动关闭");

    }

    public void Close(WebSocketCloseStatus closeStatus, string statusDescription)
    {
        Task.Run(async () =>
        {
            try
            {
                //关闭WebSocket（客户端发起）
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

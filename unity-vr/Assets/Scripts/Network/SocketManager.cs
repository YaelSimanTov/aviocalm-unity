using UnityEngine;
using SocketIOClient;
using System;
using System.Collections.Generic;

// A standardized wrapper for ALL messages sent to the server
[Serializable]
public class SystemPayload
{
    public string deviceId;
    public string content;
}

public class SocketManager : MonoBehaviour
{
    public static SocketManager instance;
    public SocketIOUnity socket;

    [HideInInspector] public string deviceId;

    // A structured queue to hold events that trigger before the socket is fully connected
    private struct QueuedEvent
    {
        public string EventName;
        public string JsonPayload;
    }
    private Queue<QueuedEvent> pendingEvents = new Queue<QueuedEvent>();

    void Awake()
    {
        // Singleton pattern and device ID initialization unified
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Auto-detect the device ID at runtime
            deviceId = SystemInfo.deviceUniqueIdentifier;
            Debug.Log($"[Dev] Device ID identified as: {deviceId}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Replace with your active connection URL
        var uri = new Uri("https://backdrop-felt-tip-domelike.ngrok-free.dev");

        var options = new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            EIO = SocketIOClient.EngineIO.V4
        };

        socket = new SocketIOUnity(uri, options);

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("[Dev] Connected to Node.js server successfully!");

            while (pendingEvents.Count > 0)
            {
                var evt = pendingEvents.Dequeue();
                socket.Emit(evt.EventName, evt.JsonPayload);
            }
        };

        socket.Connect();
    }

    public void SendEvent(string eventName, string messageContent)
    {
        SystemPayload payload = new SystemPayload
        {
            deviceId = this.deviceId,
            content = messageContent
        };

        string jsonString = JsonUtility.ToJson(payload);

        if (socket != null && socket.Connected)
        {
            socket.Emit(eventName, jsonString);

            if (eventName != "vr_system_log")
            {
                Debug.Log($"[Dev] [Socket] Sent to server: {eventName}");
            }
        }
        else
        {
            pendingEvents.Enqueue(new QueuedEvent { EventName = eventName, JsonPayload = jsonString });
        }
    }

    void OnApplicationQuit()
    {
        if (socket != null && socket.Connected)
        {
            SystemPayload payload = new SystemPayload { deviceId = this.deviceId, content = "" };
            socket.Emit("SESSION_END", JsonUtility.ToJson(payload));
            Debug.Log("[System Event] Emergency SESSION_END sent on application quit.");
        }
    }

    void OnDestroy()
    {
        if (socket != null)
        {
            socket.Disconnect();
        }
    }
}
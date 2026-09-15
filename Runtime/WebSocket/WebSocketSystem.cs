using Virtuademy.SDK.Core.SystemFramework;
using Virtuademy.SDK.WebSocket;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

/// <summary>
/// The framework's way to reach a WebSocket: a system wrapping one <see cref="WebSocketClient"/>.
/// </summary>
/// <remarks>
/// <para>
/// Every line of behaviour that used to be here is in that client now, in the transport package.
/// What is left is the part that is genuinely a system — a serialized field, an <c>Init</c> the
/// framework calls, and an entry in <c>SM</c> — so a caller inside the application keeps resolving
/// a socket the way it always did while a caller outside the framework can new the client up.
/// </para>
/// <para>
/// This is the same split <c>ApiSystemBase</c> already has over <c>ApiClientBase</c>, and it is
/// what lets the realtime package stop asking <c>SM</c> for a socket.
/// </para>
/// </remarks>
[CreateAssetMenu(menuName = "Virtuademy/SDK-WebSocket/WebSocketSystem", fileName = "WebSocketSystem")]
public class WebSocketSystem : BaseSystem, IWebSocketSystem
{
    [SerializeField] private bool secureConnection = true;

    private readonly WebSocketClient client = new();

    public override Task Init()
    {
        base.Init();

        client.Label = name;
        client.SecureConnection = secureConnection;
        client.Initialize();

        return Task.CompletedTask;
    }

    public Task<bool> ConnectAsync(string url, Dictionary<string, string> queryParams,
                                   Action<string> onWebSocketOpenError = null)
        => client.ConnectAsync(url, queryParams, onWebSocketOpenError);

    public void AddListener(string url, IWebSocketListener webSocketListener)
        => client.AddListener(url, webSocketListener);

    public void RemoveListener(string url, IWebSocketListener webSocketListener)
        => client.RemoveListener(url, webSocketListener);

    public Task DisconnectAsync(string url) => client.DisconnectAsync(url);

    public Task SendMessageAsync(string url, string message) => client.SendMessageAsync(url, message);

    public Task SendBufferMessageAsync(string url, byte[] buffer)
        => client.SendBufferMessageAsync(url, buffer);
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    /// <summary>
    /// Catches the identity provider's redirect on a loopback port, for a host that has no other
    /// way to be sent back to — the editor, and a desktop build.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is RFC 8252's loopback redirect: the application listens on <c>127.0.0.1</c>, the
    /// browser is sent there, and the code arrives in a plain GET. The provider ignores the port
    /// for a loopback address, so one registration of <c>http://localhost</c> covers every port
    /// this ever picks — which is why it picks a free one rather than insisting on a fixed one
    /// that something else may already hold.
    /// </para>
    /// <para>
    /// <b>Desktop only, and it says so rather than pretending.</b> <see cref="HttpListener"/> is
    /// not supported on Android, iOS or WebGL. A mobile application receives its redirect as a
    /// deep link on its own scheme and a web build reads its own address; both are already
    /// addresses, so they go straight to <c>IdpLogin.CompleteWithRedirect</c> with no listener in
    /// between.
    /// </para>
    /// </remarks>
    public sealed class LoopbackRedirect : IRedirectReceiver
    {
        private readonly HttpListener listener;

        /// <param name="port">A port to listen on, or 0 to take a free one.</param>
        public LoopbackRedirect(int port = 0)
        {
            if (!HttpListener.IsSupported)
            {
                throw new PlatformNotSupportedException(
                    "This platform cannot listen on a loopback port. A mobile build receives the "
                    + "redirect as a deep link on its own scheme, and a web build reads its own "
                    + "address — pass either to IdpLogin.CompleteWithRedirect directly.");
            }

            Port = port > 0 ? port : FreePort();
            RedirectUri = $"http://localhost:{Port}/";

            listener = new HttpListener();
            listener.Prefixes.Add(RedirectUri);
            listener.Start();

            // Said out loud because the failure this catches is invisible otherwise: the browser
            // comes back to an address nobody is holding and shows "cannot reach localhost", with
            // no way to tell whether the port moved, the listener died, or the provider sent the
            // user somewhere else entirely.
            Debug.Log($"[{nameof(LoopbackRedirect)}] listening on {RedirectUri} — the provider "
                      + "must send the user back to exactly this address, this port included.");
        }

        /// <summary>The port in use, once one has been settled.</summary>
        public int Port { get; }

        /// <summary>
        /// The address to give the provider. It carries the port, and the provider matches a
        /// loopback redirect without it — measured against this tenant's Azure B2C, which accepts
        /// <c>http://localhost</c> on any port and refuses <c>127.0.0.1</c> outright.
        /// </summary>
        /// <inheritdoc />
        public string RedirectUri { get; }

        /// <summary>
        /// Waits for the browser to arrive and answers it, then hands back the address it came to
        /// — query included, which is what carries the code and the state.
        /// </summary>
        /// <remarks>
        /// The page the browser is left on matters more than it looks: the user's attention is in
        /// the browser at that moment, and without a page saying so they have no way to know the
        /// application is waiting for them.
        /// </remarks>
        /// <inheritdoc cref="IRedirectReceiver.WaitForRedirect" />
        public Task<string> WaitForRedirect() => WaitForRedirect(null);

        /// <param name="message">What the browser is left showing. See the remarks.</param>
        public async Task<string> WaitForRedirect(string message)
        {
            HttpListenerContext context = await listener.GetContextAsync();

            Debug.Log($"[{nameof(LoopbackRedirect)}] the browser came back to "
                      + $"{context.Request.Url?.GetLeftPart(UriPartial.Path)} with "
                      + $"{context.Request.Url?.Query?.Length ?? 0} characters of query.");

            string body =
                "<!doctype html><meta charset=\"utf-8\">"
                + "<title>Signed in</title>"
                + "<body style=\"font:16px system-ui;margin:4rem;color:#222\">"
                + "<p>" + (message ?? "Signed in. You can close this tab and go back to the application.") + "</p>";

            byte[] bytes = Encoding.UTF8.GetBytes(body);

            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = bytes.Length;
            await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            context.Response.Close();

            return context.Request.Url?.ToString();
        }

        public void Dispose()
        {
            try
            {
                // Closing while the browser is still out is what turns a slow sign-in into a lost
                // redirect, so it is worth knowing when it happens.
                if (listener != null && listener.IsListening)
                {
                    Debug.Log($"[{nameof(LoopbackRedirect)}] {RedirectUri} released.");
                }

                listener?.Close();
            }
            catch (Exception e)
            {
                // Closing a listener that already faulted is not worth failing a login over, but
                // it is worth knowing about: a port left held is a login that cannot be retried.
                Debug.LogWarning($"[{nameof(LoopbackRedirect)}] Could not close the listener: {e.Message}");
            }
        }

        /// <summary>
        /// A port the OS says is free. There is a gap between letting it go and claiming it, which
        /// is the standard way to do this and is only a problem on a machine racing itself.
        /// </summary>
        private static int FreePort()
        {
            TcpListener probe = new(IPAddress.Loopback, 0);
            probe.Start();

            try
            {
                return ((IPEndPoint)probe.LocalEndpoint).Port;
            }
            finally
            {
                probe.Stop();
            }
        }
    }
}

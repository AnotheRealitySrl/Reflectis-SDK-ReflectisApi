using System;
using System.Threading.Tasks;

using UnityEngine;

using Virtuademy.SDK.Core.ApiSystem;

namespace Virtuademy.SDK.ApiData
{
    /// <summary>
    /// Where the identity provider sends the user back, and how this application hears about it.
    /// </summary>
    /// <remarks>
    /// The sign-in happens in a browser the application does not own, so every host needs an
    /// address the provider can reach and a way to be told the browser arrived. There is no
    /// portable answer: a desktop application can hold a loopback port, a mobile one cannot listen
    /// at all and is instead handed the address by the OS as a deep link.
    /// </remarks>
    public interface IRedirectReceiver : IDisposable
    {
        /// <summary>The address to give the provider. It must be registered there, exactly.</summary>
        string RedirectUri { get; }

        /// <summary>The address the browser was sent to, query included.</summary>
        Task<string> WaitForRedirect();
    }

    /// <summary>
    /// Picks the receiver this platform can actually use.
    /// </summary>
    public static class RedirectReceiver
    {
        /// <summary>
        /// The path the provider returns to on the application's own scheme. Any path works — the
        /// Android intent filter matches on the scheme alone — but a fixed one keeps the sign-in
        /// return distinguishable from a launch at a glance, in a log or in a provider's
        /// registration list.
        /// </summary>
        public const string AuthPath = "auth";

        /// <summary>
        /// A receiver for this host: the application's own scheme where the OS delivers deep
        /// links, a loopback port where it does not.
        /// </summary>
        /// <param name="appId">
        /// The application's id, which is also its client id at the provider and the value its
        /// scheme is derived from — so the redirect and the launch arrive on the same scheme, the
        /// one the build already claims.
        /// </param>
        /// <remarks>
        /// The editor is deliberately on the loopback side even when the build target is Android:
        /// there is no OS-level scheme registration for a project that has not been built, so a
        /// deep-link receiver in the editor would wait for something that cannot arrive.
        /// </remarks>
        public static IRedirectReceiver ForThisPlatform(Guid? appId)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            string scheme = LaunchScheme.Derive(appId)
                ?? throw new InvalidOperationException(
                    "No app id, so no scheme to be sent back to. On a mobile build the redirect "
                    + "cannot be a loopback address: the application has to be reachable by name.");

            return new DeepLinkRedirect(scheme);
#else
            _ = appId;
            return new LoopbackRedirect();
#endif
        }
    }

    /// <summary>
    /// Receives the provider's redirect as a deep link on the application's own scheme, which is
    /// the only way a mobile application can be sent back to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The scheme is the derived one, so the same intent filter the build already claims for
    /// platform launches serves this too — there is nothing extra to declare on the Android side.
    /// <b>The provider is another matter</b>: <c>scheme://auth</c> has to be registered against
    /// this client id under the provider's mobile/desktop redirect URIs, or the sign-in is refused
    /// before it starts (Azure B2C answers AADB2C90006).
    /// </para>
    /// <para>
    /// <b>What this cannot catch.</b> If the OS kills the application while the browser is in
    /// front, the redirect arrives as a cold start instead of an event, and the login that was
    /// waiting no longer exists to be resumed. Keeping the application alive behind the browser is
    /// the host's business — a custom tab rather than a full browser hand-off — and is not
    /// something this type can arrange.
    /// </para>
    /// </remarks>
    public sealed class DeepLinkRedirect : IRedirectReceiver
    {
        private readonly TaskCompletionSource<string> arrived =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        private readonly string scheme;

        public DeepLinkRedirect(string scheme, string path = RedirectReceiver.AuthPath)
        {
            this.scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            RedirectUri = $"{scheme}://{path}";

            Application.deepLinkActivated += OnDeepLink;

            Debug.Log($"[{nameof(DeepLinkRedirect)}] waiting for {RedirectUri} — the provider must "
                      + "have this exact address registered for this client, under its mobile and "
                      + "desktop redirect URIs.");

#if UNITY_IOS && !UNITY_EDITOR
            // Android gets its intent filter from the build step that claims the launch scheme;
            // iOS has no equivalent, and an undeclared scheme is not a failure on iOS — the OS
            // simply never delivers, so this would wait forever with nothing to read. Said once,
            // here, because the alternative is discovering it as a login that hangs.
            Debug.LogWarning($"[{nameof(DeepLinkRedirect)}] on iOS the scheme has to be declared "
                             + "in CFBundleURLTypes, and nothing in this package does that yet. "
                             + "Without it the redirect is never delivered and this waits forever.");
#endif
        }

        /// <inheritdoc />
        public string RedirectUri { get; }

        /// <inheritdoc />
        public Task<string> WaitForRedirect() => arrived.Task;

        public void Dispose() => Application.deepLinkActivated -= OnDeepLink;

        /// <remarks>
        /// Only addresses on this application's scheme are taken. The same event carries the
        /// platform's launch links, which are on the same scheme but carry a session rather than a
        /// code — <see cref="IdpLogin.CompleteWithRedirect"/> rejects those on the state, so the
        /// two cannot be confused for one another, but answering one login with another's address
        /// would still be a mistake worth not making.
        /// </remarks>
        private void OnDeepLink(string url)
        {
            Debug.Log($"[{nameof(DeepLinkRedirect)}] deep link: {url?.Split('?')[0]}");

            if (!string.IsNullOrEmpty(url)
                && url.StartsWith($"{scheme}://", StringComparison.OrdinalIgnoreCase))
            {
                arrived.TrySetResult(url);
            }
        }
    }
}

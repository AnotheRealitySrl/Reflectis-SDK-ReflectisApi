using System;
using System.Collections.Generic;

using UnityEngine;

using Virtuademy.SDK.Interface;

namespace Virtuademy.SDK.ApiData
{
    /// <summary>
    /// Reads what the platform put in the address it launched this application with.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The launching half has existed for a while: the application's <c>AppManager</c> builds
    /// <c>?authSessionHash=…&amp;worldId=…&amp;experienceId=…</c> and opens the app's scheme with it,
    /// on Android through an <c>ACTION_VIEW</c> intent and elsewhere through the web link. The
    /// receiving half existed only as a sample in the framework package, which an external
    /// application does not install and could not use anyway — the sample restores the session
    /// through <c>SM.GetSystem&lt;IAuthenticationSystem&gt;()</c>. So an app outside the framework
    /// was handed a session and had nothing to read it with. This is that half.
    /// </para>
    /// <para>
    /// <b>Why <see cref="Application.absoluteURL"/> and not the deep-link parsers in
    /// SPACS-Utility.</b> Those are <c>MonoBehaviour</c>s: using one means putting a component in a
    /// scene and wiring a serialized reference, which is a lot of ceremony for reading a string
    /// available statically. Unity fills <c>absoluteURL</c> with the deep link on Android and iOS
    /// and with the page address on WebGL, which is exactly the two ways the platform launches
    /// anything. The parsers stay where they are; nothing here replaces them.
    /// </para>
    /// <para>
    /// <b>It is a session hash, not a token.</b> What arrives identifies a session the user has
    /// already established in the platform application; the tokens are minted against it by
    /// <see cref="ProfileClient.GetTokens"/> when the session is restored. That is why the launch
    /// carries the hash and not a bearer: a token would arrive already expiring and could not be
    /// renewed, while a session can mint tokens for as long as it lives.
    /// </para>
    /// </remarks>
    public static class PlatformLaunch
    {
        /// <summary>The authentication session to restore. Same spelling the launcher writes.</summary>
        public const string SessionHashKey = "authSessionHash";

        /// <summary>The world the user was in when they left the platform application.</summary>
        public const string WorldIdKey = "worldId";

        /// <summary>The experience they launched this application from.</summary>
        public const string ExperienceIdKey = "experienceId";

        /// <summary>
        /// What this application was launched with, or null when it was started on its own.
        /// </summary>
        /// <remarks>
        /// Empty in the editor, where nothing launched anything — use <see cref="FromUrl"/> with an
        /// address you paste in to exercise the path without a build.
        /// </remarks>
        public static PlatformLaunchData Current => FromUrl(Application.absoluteURL);

        /// <summary>
        /// Raised when the platform launches this application while it is already running, which on
        /// Android is an intent delivered to the live activity rather than a fresh start.
        /// </summary>
        /// <remarks>
        /// A second launch carries a second session, and it can be a different user's: the platform
        /// application is where the account lives, so a handover that ignored this would leave an
        /// app running as whoever happened to be first. Restore from it exactly as from
        /// <see cref="Current"/>.
        /// </remarks>
        public static event Action<PlatformLaunchData> Received;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Subscribe()
        {
            Application.deepLinkActivated -= OnDeepLink;
            Application.deepLinkActivated += OnDeepLink;
        }

        private static void OnDeepLink(string url)
        {
            PlatformLaunchData data = FromUrl(url);

            if (data != null)
            {
                Received?.Invoke(data);
            }
        }

        /// <summary>
        /// Reads an address the platform launched with.
        /// </summary>
        /// <returns>
        /// The launch data, or null when the address carries none of the three parameters — which
        /// is the ordinary case for an application the user started themselves.
        /// </returns>
        public static PlatformLaunchData FromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            int query = url.IndexOf('?');

            if (query < 0 || query == url.Length - 1)
            {
                return null;
            }

            // A fragment is not part of the query, and a scheme-based launch can carry one.
            int fragment = url.IndexOf('#', query + 1);
            string parameters = fragment >= 0
                ? url.Substring(query + 1, fragment - query - 1)
                : url.Substring(query + 1);

            return FromParameters(Parse(parameters));
        }

        /// <summary>
        /// Builds the launch data from parameters read elsewhere — a host that already has them,
        /// or one of the SPACS-Utility parsers.
        /// </summary>
        public static PlatformLaunchData FromParameters(IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return null;
            }

            parameters.TryGetValue(SessionHashKey, out string sessionHash);

            int? worldId = ReadId(parameters, WorldIdKey);
            int? experienceId = ReadId(parameters, ExperienceIdKey);

            if (string.IsNullOrEmpty(sessionHash) && worldId == null && experienceId == null)
            {
                return null;
            }

            return new PlatformLaunchData(sessionHash, worldId, experienceId);
        }

        /// <summary>
        /// An id that is present but unreadable is reported and dropped, rather than failing the
        /// launch: the session hash is what an application cannot do without, and losing the whole
        /// handover because an id was malformed would turn a cosmetic fault into a sign-in failure.
        /// </summary>
        private static int? ReadId(IReadOnlyDictionary<string, string> parameters, string key)
        {
            if (!parameters.TryGetValue(key, out string raw) || string.IsNullOrEmpty(raw))
            {
                return null;
            }

            if (int.TryParse(raw, out int value))
            {
                return value;
            }

            Debug.LogWarning($"[{nameof(PlatformLaunch)}] {key} is not a number ('{raw}'); ignoring it.");

            return null;
        }

        private static Dictionary<string, string> Parse(string parameters)
        {
            Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);

            foreach (string pair in parameters.Split('&'))
            {
                if (string.IsNullOrEmpty(pair))
                {
                    continue;
                }

                int separator = pair.IndexOf('=');

                if (separator <= 0)
                {
                    continue;
                }

                string key = Uri.UnescapeDataString(pair.Substring(0, separator));

                // Everything after the first '=' is the value — a hash can carry its own '=' — and
                // it is unescaped with URI rules, not form rules. The launcher encodes with
                // UnityWebRequest.EscapeURL, which is form encoding: a real '+' in the hash arrives
                // as %2B, so decoding it back to '+' is right, while treating a bare '+' as a space
                // would quietly corrupt the one value the handover cannot do without.
                string value = Uri.UnescapeDataString(pair.Substring(separator + 1));

                result[key] = value;
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

using Virtuademy.SDK.Core.ApiSystem;
using Virtuademy.SDK.Http;
using Virtuademy.SDK.TenantConfiguration;

using SPACS.Utilities;

namespace Virtuademy.SDK.ApiData
{
    /// <summary>
    /// Signs a user in at the tenant's identity provider and hands back the access token the
    /// platform will accept.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Authorization code with PKCE</b>, written out rather than taken from a library. MSAL is
    /// what the editor uses, and it is a desktop .NET library: an application that ships to a
    /// headset or a browser cannot take it. The protocol itself is four things — a random
    /// verifier, its hash in the authorize URL, the code that comes back, and one form POST — and
    /// all four are portable, which the library is not.
    /// </para>
    /// <para>
    /// <b>No client secret, by construction.</b> A public client cannot keep one, which is what
    /// PKCE exists for: the verifier is generated per login and never leaves the app, so an
    /// authorization code intercepted on its way back is useless to whoever took it.
    /// </para>
    /// <para>
    /// <b>Everything comes from the tenant.</b> Authority, policy, the API the token is for, and
    /// the client id — which is the application's own app id, the same GUID that signs its HMAC
    /// requests. Nothing about an identity provider is compiled into an application.
    /// </para>
    /// <para>
    /// <b>What this does not do is receive the redirect.</b> Only the host knows how: a desktop
    /// application can listen on a loopback port, a mobile one is handed the address as a deep
    /// link, a web build reads its own location. Hand the address you received to
    /// <see cref="CompleteWithRedirect"/>; the flow is otherwise identical.
    /// </para>
    /// </remarks>
    public sealed class IdpLogin
    {
        private readonly string authority;
        private readonly string clientId;
        private readonly string[] scopes;
        private readonly string codeVerifier;
        private readonly string state;

        private IdpLogin(string authority, string clientId, string[] scopes, string redirectUri)
        {
            this.authority = authority;
            this.clientId = clientId;
            this.scopes = scopes;

            RedirectUri = redirectUri;

            codeVerifier = RandomUrlSafe(64);
            state = RandomUrlSafe(16);
        }

        /// <summary>
        /// Builds a login for the identity provider the tenant configuration names.
        /// </summary>
        /// <param name="tenant">An initialised configuration client — this reads the tenant it fetched.</param>
        /// <param name="redirectUri">
        /// Where the provider sends the user back. It must be registered against this client id in
        /// the provider, and the provider refuses the request outright when it is not. Defaults to
        /// what the tenant records, which today is the loopback address the editor uses.
        /// </param>
        public static IdpLogin ForTenant(TenantConfigurationClient tenant, string redirectUri = null)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            AzureB2CConfig auth = tenant.TenantConfiguration?.Config?.AuthConfig
                ?? throw new InvalidOperationException(
                    "The tenant carries no authentication configuration, so there is no identity "
                    + "provider to sign in at. Call Init on the configuration client first.");

            Guid? appId = tenant.AppIdentification?.Credential?.AppId;

            if (appId == null || appId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "No app id: the application's own credential is also its client id at the "
                    + "identity provider, so a login cannot be built without one.");
            }

            return new IdpLogin(AuthorityFor(auth),
                                appId.Value.ToString("D"),
                                ScopesFor(auth),
                                redirectUri ?? auth.RedirectUri);
        }

        /// <summary>Where the provider sends the user back, exactly as it was requested.</summary>
        public string RedirectUri { get; }

        /// <summary>
        /// The address to open in a browser. The user signs in there; the provider then sends them
        /// to <see cref="RedirectUri"/> with a code in the query.
        /// </summary>
        public string AuthorizeUrl
        {
            get
            {
                Dictionary<string, string> query = new()
                {
                    { "client_id", clientId },
                    { "response_type", "code" },
                    { "redirect_uri", RedirectUri },
                    { "response_mode", "query" },
                    { "scope", string.Join(" ", scopes) },
                    { "state", state },
                    { "nonce", RandomUrlSafe(16) },
                    { "code_challenge", Challenge(codeVerifier) },
                    { "code_challenge_method", "S256" },
                };

                return $"{authority}/oauth2/v2.0/authorize?{Encode(query)}";
            }
        }

        /// <summary>
        /// Turns the address the provider sent the user back to into an access token.
        /// </summary>
        /// <param name="redirectUrl">
        /// The whole address, query included. Anything else — a bare code, a truncated address —
        /// loses the state, and the state is the only thing that says this response belongs to
        /// this login.
        /// </param>
        /// <returns>The access token, to hand to <see cref="IPlatformAuthentication.CompleteLoginWithAccessToken"/>.</returns>
        public async Task<string> CompleteWithRedirect(string redirectUrl)
        {
            Dictionary<string, string> response = Query(redirectUrl);

            // The provider reports refusals here rather than by failing the request: the user
            // cancelled, the redirect was not registered, the policy rejected them.
            if (response.TryGetValue("error", out string error))
            {
                response.TryGetValue("error_description", out string description);

                throw new InvalidOperationException(
                    $"The identity provider refused the login: {error}. {description}");
            }

            if (!response.TryGetValue("state", out string returned) || returned != state)
            {
                throw new InvalidOperationException(
                    "The response does not carry this login's state. It belongs to a different "
                    + "login, or to nobody — either way it is not answered.");
            }

            if (!response.TryGetValue("code", out string code) || string.IsNullOrEmpty(code))
            {
                throw new InvalidOperationException("The response carries no authorization code.");
            }

            return await Exchange(code);
        }

        /// <summary>
        /// The one call that is not a redirect: the code plus the verifier, for a token. Sent as a
        /// form because that is what the token endpoint takes.
        /// </summary>
        private async Task<string> Exchange(string code)
        {
            WWWForm form = new();
            form.AddField("grant_type", "authorization_code");
            form.AddField("client_id", clientId);
            form.AddField("code", code);
            form.AddField("redirect_uri", RedirectUri);
            form.AddField("code_verifier", codeVerifier);
            form.AddField("scope", string.Join(" ", scopes));

            using UnityWebRequest request = UnityWebRequest.Post($"{authority}/oauth2/v2.0/token", form);
            await request.SendWebRequest();

            ApiResponse<IdpToken> response = new(request.responseCode, request.error,
                                                 request.downloadHandler.text);

            if (!response.IsSuccess || string.IsNullOrEmpty(response.Content?.AccessToken))
            {
                throw new InvalidOperationException(
                    $"The identity provider would not exchange the code: {response.StatusCode} "
                    + $"{response.ReasonPhrase}.");
            }

            return response.Content.AccessToken;
        }

        /// <remarks>
        /// B2C addresses a user flow, Entra ID a directory. Both then answer at
        /// <c>/oauth2/v2.0/{authorize,token}</c>, which is why only the authority differs here.
        /// </remarks>
        private static string AuthorityFor(AzureB2CConfig auth)
        {
            if (auth.IsEntraId)
            {
                return $"https://login.microsoftonline.com/{auth.Tenant}";
            }

            if (auth.IsB2C)
            {
                return $"https://{auth.Tenant}.b2clogin.com/{auth.Tenant}.onmicrosoft.com/{auth.Policy}";
            }

            throw new InvalidOperationException(
                $"Unrecognised authentication policy '{auth.Policy}': expected 'EntraID' or a "
                + "value starting with 'B2C_'.");
        }

        /// <remarks>
        /// The same three the editor asks for, and each earns its place: <c>openid</c> to be
        /// issued an identity at all, <c>offline_access</c> for a refresh token — without which
        /// nothing can ever be renewed without showing the user a browser again — and access to
        /// the profile API, which is the one the platform will read this token at.
        /// </remarks>
        private static string[] ScopesFor(AzureB2CConfig auth)
            => new[]
            {
                "openid",
                "offline_access",
                $"https://{auth.Tenant}.onmicrosoft.com/{auth.ProfileApiId}/access",
            };

        /// <summary>
        /// Percent-encoding, not form encoding. <c>HttpHelper.CreateQueryString</c> escapes with
        /// <c>UnityWebRequest.EscapeURL</c>, which writes a space as <c>+</c> — and the scope
        /// parameter is a space-separated list, so a provider reading it literally would be handed
        /// one scope with pluses in its name rather than three scopes.
        /// </summary>
        private static string Encode(Dictionary<string, string> query)
        {
            List<string> pairs = new();

            foreach (KeyValuePair<string, string> pair in query)
            {
                pairs.Add($"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}");
            }

            return string.Join("&", pairs);
        }

        private static Dictionary<string, string> Query(string url)
        {
            Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(url))
            {
                return result;
            }

            int start = url.IndexOf('?');

            if (start < 0 || start == url.Length - 1)
            {
                return result;
            }

            int fragment = url.IndexOf('#', start + 1);
            string query = fragment >= 0
                ? url.Substring(start + 1, fragment - start - 1)
                : url.Substring(start + 1);

            foreach (string pair in query.Split('&'))
            {
                int separator = pair.IndexOf('=');

                if (separator > 0)
                {
                    result[Uri.UnescapeDataString(pair.Substring(0, separator))] =
                        Uri.UnescapeDataString(pair.Substring(separator + 1));
                }
            }

            return result;
        }

        /// <summary>
        /// PKCE's challenge: the verifier's SHA-256, base64url. The provider keeps it, and only
        /// the app that generated the verifier can answer for it later.
        /// </summary>
        private static string Challenge(string verifier)
        {
            using SHA256 sha = SHA256.Create();

            return Base64Url(sha.ComputeHash(Encoding.ASCII.GetBytes(verifier)));
        }

        private static string RandomUrlSafe(int bytes)
        {
            byte[] buffer = new byte[bytes];

            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(buffer);
            }

            return Base64Url(buffer);
        }

        /// <summary>
        /// Base64 as the URL wants it: no padding, and the two characters that mean something else
        /// in a query swapped out.
        /// </summary>
        private static string Base64Url(byte[] value)
            => Convert.ToBase64String(value).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        /// <summary>
        /// What the token endpoint answers with. Only the access token is read: the refresh token
        /// is requested so the provider issues one, but renewing at the provider is the host's
        /// business, and the platform session this token establishes outlives it anyway.
        /// </summary>
        [Serializable, Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
        private class IdpToken
        {
            [SerializeField, Newtonsoft.Json.JsonProperty("access_token")]
            private string accessToken;

            public string AccessToken => accessToken;
        }
    }
}

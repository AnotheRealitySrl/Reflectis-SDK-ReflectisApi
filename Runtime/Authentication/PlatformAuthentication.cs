using System;
using System.Threading.Tasks;

using Virtuademy.SDK.Core.ApiSystem;
using Virtuademy.SDK.Http;
using Virtuademy.SDK.Interface;
using Virtuademy.SDK.TenantConfiguration;

namespace Virtuademy.SDK.ApiData
{
    /// <summary>
    /// Signs a user in from an application the platform did not start — the standalone case, where
    /// there is no launch data and no session to restore.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The code flow, which is the one a headset has always used and the only one that works
    /// without a browser the app can read: the app shows an address and eight characters, the user
    /// opens that address on any other device and types them, and the page answers with four
    /// characters the user types back. Nothing in that round trip requires the app to observe a
    /// redirect, which is why it is the flow that works everywhere.
    /// </para>
    /// <para>
    /// <b>The token completion is declared and refuses.</b> <see cref="CompleteLoginWithAccessToken"/>
    /// belongs to a host that can read the redirect, and nothing in this implementation can —
    /// throwing says so at the call site rather than in a log nobody reads. It is the next piece,
    /// along with an identity provider.
    /// </para>
    /// <para>
    /// The session is not persisted here. Whether an application should keep a platform session
    /// across runs is a policy decision, not a default to inherit, and an app that wants it can
    /// hold the hash itself and hand it to <see cref="RestoreSession"/>.
    /// </para>
    /// <para>
    /// <b>An application is configured from its tenant, exactly as the platform's own is.</b> A
    /// developer receives the app's <c>AppConfig</c> — a credential and the address of the
    /// configuration API — and <see cref="ForTenant"/> fetches the rest: which profile API to talk
    /// to, at which version, and which web address the user opens. Nothing about a tenant is
    /// compiled into an application, which is what lets the same build serve more than one.
    /// </para>
    /// </remarks>
    public sealed class PlatformAuthentication : IPlatformAuthentication
    {
        private readonly ProfileClient profile;
        private readonly string applicationUrl;

        /// <param name="profile">A configured profile client — its connection settles the tenant.</param>
        /// <param name="applicationUrl">
        /// The tenant's web address, which the user opens to complete the login. It comes from the
        /// tenant configuration rather than from this assembly, because an implementation that
        /// composed it would be deciding which tenant the application belongs to.
        /// <see cref="ForTenant"/> is the ordinary way in; this constructor is for a host that has
        /// already resolved both by other means.
        /// </param>
        public PlatformAuthentication(ProfileClient profile, string applicationUrl)
        {
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this.applicationUrl = applicationUrl
                                  ?? throw new ArgumentNullException(nameof(applicationUrl));
        }

        /// <summary>
        /// Builds a login for the tenant a configuration client has already read.
        /// </summary>
        /// <remarks>
        /// The client must have been initialised — this reads the tenant it fetched. The profile
        /// client it builds carries the same credential the configuration one does, because they
        /// are the same application talking to two APIs of the same tenant.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// The configuration client has no tenant yet, so there is nothing to configure from.
        /// </exception>
        public static async Task<PlatformAuthentication> ForTenant(TenantConfigurationClient tenant)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            TenantConfig config = tenant.TenantConfiguration?.Config
                ?? throw new InvalidOperationException(
                    "The configuration client has not read a tenant. Call Init on it first: an "
                    + "application learns which profile API to use from the tenant, not from its "
                    + "own build.");

            ProfileClient profile = new();
            await profile.Init(new AppIdentification(tenant.AppIdentification.Credential,
                                                     config.ProfileApiUrl,
                                                     config.ProfileApiVersion));

            return new PlatformAuthentication(profile, config.ApplicationUrl);
        }

        /// <inheritdoc />
        public bool IsAuthenticated { get; private set; }

        /// <inheritdoc />
        public event Action AuthenticationChanged;

        /// <inheritdoc />
        public async Task<LoginChallenge> BeginLogin()
        {
            ApiResponse<PlatformSession> begun = await profile.BeginSession();

            if (!begun.IsSuccess || begun.Content == null)
            {
                throw new InvalidOperationException(
                    $"Could not begin a login: {begun.StatusCode} {begun.ReasonPhrase}.");
            }

            profile.Session = begun.Content;

            return new LoginChallenge(begun.Content.Identifier, applicationUrl);
        }

        /// <inheritdoc />
        public Task CompleteLoginWithAccessToken(string accessToken)
            => throw new NotSupportedException(
                "This implementation completes a login with the confirmation code, not with an "
                + "access token. The token completion needs a host that can read the login "
                + "redirect — a browser, a web view, a mobile shell — and is not built yet.");

        /// <inheritdoc />
        public async Task CompleteLoginWithCode(string confirmationCode)
        {
            ApiResponse<PlatformSession> enabled = await profile.EnableSession(confirmationCode);

            if (!enabled.IsSuccess || enabled.Content == null)
            {
                throw new InvalidOperationException(
                    $"The confirmation code was refused: {enabled.StatusCode} {enabled.ReasonPhrase}.");
            }

            await Establish(enabled.Content.Hash);
        }

        /// <summary>
        /// Takes up a session the application already has a hash for — one it persisted itself, or
        /// one it was handed. Not part of the contract: an app launched from the catalog receives
        /// its hash in the launch data, and this is the standalone equivalent.
        /// </summary>
        public Task RestoreSession(string sessionHash) => Establish(sessionHash);

        /// <inheritdoc />
        public async Task Logout()
        {
            if (profile.Session != null)
            {
                await profile.KillSession(profile.Session.Id);
            }

            profile.Session = null;
            SetAuthenticated(false);
        }

        /// <summary>
        /// Reads the session back and mints the tokens against it. Both halves matter: the read
        /// says the session is live, and without the tokens the first call to any other API
        /// arrives with nothing to present.
        /// </summary>
        private async Task Establish(string sessionHash)
        {
            ApiResponse<PlatformSession> session = await profile.GetSessionByHash(sessionHash);

            if (!session.IsSuccess || session.Content == null)
            {
                profile.Session = null;
                SetAuthenticated(false);

                throw new InvalidOperationException(
                    $"The session is not usable: {session.StatusCode} {session.ReasonPhrase}.");
            }

            // The read answers with everything but the hash, which is what identified it in the
            // first place — so it is put back rather than lost.
            session.Content.Hash = sessionHash;
            profile.Session = session.Content;

            await profile.GetTokens();

            SetAuthenticated(true);
        }

        private void SetAuthenticated(bool value)
        {
            if (IsAuthenticated == value)
            {
                return;
            }

            IsAuthenticated = value;
            AuthenticationChanged?.Invoke();
        }
    }
}

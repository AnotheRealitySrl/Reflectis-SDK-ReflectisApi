using System;
using System.Threading.Tasks;

namespace Virtuademy.SDK.Interface
{
    /// <summary>
    /// Signs a user in. A sibling of <see cref="IPlatformContext"/> rather than part of it:
    /// <see cref="IPlatformContext.Initialize"/> presupposes an authenticated user, so in an app's
    /// own startup the order is authenticate, then choose a world.
    /// </summary>
    /// <remarks>
    /// <b>One begin, two completions.</b> The user always starts the same way — the app opens
    /// <see cref="LoginChallenge.LoginUrl"/> — and then finishes either by handing back an access
    /// token (a browser or web view that can read the redirect) or by typing a short code shown at
    /// the end of the web flow (a headset, where there is no shared browser to read).
    /// <para>
    /// <b>The catalog path is not here on purpose.</b> An app launched from the catalog receives an
    /// authentication session hash in its launch data, and the implementation restores the session
    /// from it. None of these members are called in that case, which is why the app does not need to
    /// know which way it was started.
    /// </para>
    /// <para>
    /// <b>Restore precedence is part of the contract, not an implementation detail</b>: an incoming
    /// launch hash first, then a persisted one, then nothing. In that order and no other. Preferring
    /// a persisted session over an incoming one is precisely the defect that had an embedded client
    /// running as the *previous* user — it had a stored hash, so it never looked at the one it was
    /// handed.
    /// </para>
    /// </remarks>
    public interface IPlatformAuthentication
    {
        /// <summary>Whether a session is currently established.</summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Starts a login and returns what the user needs in order to complete it.
        /// </summary>
        Task<LoginChallenge> BeginLogin();

        /// <summary>
        /// Completes the login begun by <see cref="BeginLogin"/> with an access token obtained from
        /// the web flow. For hosts that can observe the redirect — a browser, a web view, a mobile
        /// shell.
        /// </summary>
        Task CompleteLoginWithAccessToken(string accessToken);

        /// <summary>
        /// Completes the login begun by <see cref="BeginLogin"/> with the short confirmation code
        /// the web flow shows the user at the end. For hosts where the user cannot hand a token
        /// back — a headset, typically.
        /// </summary>
        Task CompleteLoginWithCode(string confirmationCode);

        /// <summary>Ends the session and clears any persisted one.</summary>
        Task Logout();

        /// <summary>Raised whenever <see cref="IsAuthenticated"/> changes.</summary>
        event Action AuthenticationChanged;
    }
}

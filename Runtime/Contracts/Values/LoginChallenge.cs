namespace Virtuademy.SDK.Interface
{
    /// <summary>
    /// What the user needs in order to finish a login the app has begun.
    /// </summary>
    public sealed class LoginChallenge
    {
        public LoginChallenge(string identifier, string loginUrl)
        {
            Identifier = identifier;
            LoginUrl = loginUrl;
        }

        /// <summary>The pending session's identifier, as the platform knows it.</summary>
        public string Identifier { get; }

        /// <summary>
        /// The page to send the user to, composed by the implementation.
        /// </summary>
        /// <remarks>
        /// Composed there and not by the app on purpose: the host part comes from the tenant
        /// configuration, so an app that built this URL itself would be hardcoding which tenant it
        /// belongs to.
        /// </remarks>
        public string LoginUrl { get; }
    }
}

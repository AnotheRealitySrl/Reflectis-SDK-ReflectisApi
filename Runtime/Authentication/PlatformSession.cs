using System;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    /// <summary>
    /// A login session as the profile API reports it: the code the user types on the web, the code
    /// they type back into the app, and the hash that identifies the session afterwards.
    /// </summary>
    /// <remarks>
    /// The same shape the platform's own authentication system reads, declared again here because
    /// that system lives in a package an external application does not install. Four fields of the
    /// seven the endpoint returns — the rest are timestamps nothing in this flow reads.
    /// </remarks>
    [Serializable, Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class PlatformSession
    {
        [SerializeField] private int id;
        [SerializeField] private string identifier;
        [SerializeField] private string hash;
        [SerializeField] private string checkCode;

        /// <summary>The session's own id, needed to end it.</summary>
        public int Id => id;

        /// <summary>
        /// The eight characters the user types into the web page to say which app is asking.
        /// </summary>
        public string Identifier => identifier;

        /// <summary>
        /// What identifies this session from here on. Sent as the <c>SessionHash</c> header, and
        /// the only part worth persisting.
        /// </summary>
        public string Hash { get => hash; set => hash = value; }

        /// <summary>
        /// The four characters the web page shows at the end, which the user types back into the
        /// app to prove they completed the login.
        /// </summary>
        public string CheckCode => checkCode;
    }
}

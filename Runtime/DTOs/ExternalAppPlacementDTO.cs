using System;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    /// <summary>
    /// One published <c>ExternalApp</c> experience of the calling app, plus the world it lives in.
    /// The answer from <c>GET /external-app/worlds</c>.
    /// </summary>
    /// <remarks>
    /// A projection and not the full world record, deliberately: an external app has no business
    /// receiving a world's configuration. What is here is what a chooser and a session need — which
    /// is also why there is no world lookup per entry. The alternative shape would have cost one
    /// round trip per candidate world just to render a list.
    /// <para>
    /// One entry per <em>experience</em>, not per world: a world holding two publications of the
    /// same app appears twice, told apart by <see cref="ExperienceLabel"/> and nothing else. The
    /// server does not pick a winner, because both are real launch targets.
    /// </para>
    /// <para>
    /// <b>There is no app parameter on the request.</b> The app is read from the token's <c>azp</c>
    /// claim and compared server-side against the <c>appObjectId</c> on each experience's config, so
    /// there is no second copy of an app identity to keep in sync — and an app cannot ask about a
    /// different app. See <c>contracts/openapi/external-app-worlds.yaml</c> in the meta-repo.
    /// </para>
    /// </remarks>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class ExternalAppPlacementDTO
    {
        [SerializeField] private int worldId;
        [SerializeField] private string worldLabel;
        [SerializeField] private string worldDescription;
        [SerializeField] private string worldThumbnailUri;
        [SerializeField] private bool multiplayer;
        [SerializeField] private int experienceId;
        [SerializeField] private string experienceLabel;

        /// <summary>The world to enter.</summary>
        public int WorldId => worldId;

        /// <summary>The world's display name, for a chooser.</summary>
        public string WorldLabel => worldLabel;

        /// <summary>Nullable on the wire.</summary>
        public string WorldDescription => worldDescription;

        /// <summary>Nullable on the wire.</summary>
        public string WorldThumbnailUri => worldThumbnailUri;

        /// <summary>
        /// Whether the world itself is multiplayer. Here because the session the app creates or
        /// joins has to agree with the world about it.
        /// </summary>
        public bool Multiplayer => multiplayer;

        /// <summary>
        /// The experience this app is published as in that world — the same value the catalog passes
        /// when it launches the app itself, which is what makes the standalone and
        /// launched-from-catalog paths converge on one shape.
        /// </summary>
        public int ExperienceId => experienceId;

        /// <summary>The experience's own name, which may differ from the app's.</summary>
        public string ExperienceLabel => experienceLabel;
    }
}

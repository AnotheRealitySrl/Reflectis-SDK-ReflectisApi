using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    /// <summary>
    /// Runtime NPC (chatbot appearance) returned by the Application API
    /// GET worlds/{wid}/npcs and GET worlds/{wid}/npcs/{id}/runtime.
    /// Mirrors the server-side NpcClientData DTO. The download URLs are
    /// short-lived SAS links minted per response — follow them verbatim,
    /// do not cache the URL (re-fetch to re-mint when expired).
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class NpcDTO
    {
        [SerializeField] private int id;
        [SerializeField] private string label;
        [SerializeField] private string description;
        // Canonical animation state (Idle/Thinking/Talk) -> GLB clip name.
        // May be null or partial; fallbacks are the client's job (see ResolveClip).
        [SerializeField] private Dictionary<string, string> clipMap;
        // Slim pose correction; null when the NPC has no correction (apply identity).
        [SerializeField] private NpcTransformDTO transform;
        [SerializeField] private DateTime lastModified;
        [SerializeField] private string thumbnailUrl;
        [SerializeField] private string modelUrl;
        [SerializeField] private bool isWorldDefault;

        public int Id => id;
        public string Label => label;
        public string Description => description;
        public IReadOnlyDictionary<string, string> ClipMap => clipMap;
        public NpcTransformDTO Transform => transform;
        public DateTime LastModified => lastModified;
        public string ThumbnailUrl => thumbnailUrl;
        public string ModelUrl => modelUrl;

        /// <summary>
        /// C2 "world favorite": true for THE single NPC pinned as this world's favorite
        /// (at most one true in the list). A per-world choice, distinct from the tenant-level
        /// auto-seed default (which the runtime list does not carry). Use it to pre-select the
        /// NPC and to pin it first (see <see cref="NpcDtoOrdering.OrderForPicker"/>).
        /// </summary>
        public bool IsWorldDefault => isWorldDefault;

        /// <summary>
        /// Resolves the GLB clip name for a canonical state with the documented
        /// fallbacks: no Thinking -> Idle; no Idle -> null (caller keeps the
        /// model's base pose). Returns null when nothing is mapped.
        /// </summary>
        public string ResolveClip(string state)
        {
            if (clipMap == null)
                return null;
            if (clipMap.TryGetValue(state, out string clip) && !string.IsNullOrEmpty(clip))
                return clip;
            if (state == "Thinking" && clipMap.TryGetValue("Idle", out string idle) && !string.IsNullOrEmpty(idle))
                return idle;
            return null;
        }
    }

    /// <summary>
    /// Default ordering for the NPC appearance picker: the world favorite
    /// (<see cref="NpcDTO.IsWorldDefault"/>) pinned first, then alphabetical by label
    /// (ordinal, case-insensitive) — matching the server's own order. Reusable via the
    /// <see cref="NpcDtoOrdering.OrderForPicker"/> LINQ extension. NPC sibling of
    /// AgentPickerComparer (NPCs have no tier, so there is no tier tie-break).
    /// </summary>
    public sealed class NpcPickerComparer : IComparer<NpcDTO>
    {
        public static readonly NpcPickerComparer Instance = new();

        public int Compare(NpcDTO x, NpcDTO y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return 1;   // nulls last
            if (y is null) return -1;

            // The world favorite is pinned to the very top (it's the most important).
            int byWorldDefault = y.IsWorldDefault.CompareTo(x.IsWorldDefault);   // true before false
            if (byWorldDefault != 0) return byWorldDefault;

            return string.Compare(x.Label, y.Label, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class NpcDtoOrdering
    {
        /// <summary>
        /// Returns the NPCs in the picker's default order (world favorite first, then
        /// alphabetical by label). Non-mutating; e.g. <c>response.Content.OrderForPicker()</c>.
        /// </summary>
        public static IEnumerable<NpcDTO> OrderForPicker(this IEnumerable<NpcDTO> npcs) =>
            npcs.OrderBy(n => n, NpcPickerComparer.Instance);
    }

    /// <summary>
    /// The "transform" node of npc_metadata, already in Unity coordinates
    /// (localRotation is a quaternion). The Backoffice-only "editorData" sibling
    /// is not sent by the runtime endpoints.
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class NpcTransformDTO
    {
        [SerializeField] private Vector3DTO localPosition;
        [SerializeField] private QuaternionDTO localRotation;
        [SerializeField] private Vector3DTO localScale;

        public Vector3 LocalPosition => localPosition?.ToVector3() ?? Vector3.zero;
        public Quaternion LocalRotation => localRotation?.ToQuaternion() ?? Quaternion.identity;
        public Vector3 LocalScale => localScale?.ToVector3() ?? Vector3.one;

        /// <summary>Applies the authoring pose correction to the instantiated GLB root.</summary>
        public void ApplyTo(Transform root)
        {
            root.localPosition = LocalPosition;
            root.localRotation = LocalRotation;
            root.localScale = LocalScale;
        }
    }

    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class Vector3DTO
    {
        [SerializeField] private float x, y, z;
        public Vector3 ToVector3() => new(x, y, z);
    }

    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class QuaternionDTO
    {
        [SerializeField] private float x, y, z, w;
        public Quaternion ToQuaternion() => new(x, y, z, w);
    }
}

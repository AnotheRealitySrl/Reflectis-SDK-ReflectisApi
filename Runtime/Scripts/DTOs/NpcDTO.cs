using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace Reflectis.SDK.ReflectisApi
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

        public int Id => id;
        public string Label => label;
        public string Description => description;
        public IReadOnlyDictionary<string, string> ClipMap => clipMap;
        public NpcTransformDTO Transform => transform;
        public DateTime LastModified => lastModified;
        public string ThumbnailUrl => thumbnailUrl;
        public string ModelUrl => modelUrl;

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

using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    /// <summary>
    /// The interpreted (HybridCLR) assembly an environment needs. Null when the world
    /// carries no scripts — the API answers 204 in that case, which is the common one.
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class EnvironmentDllDTO
    {
        [SerializeField] private int id;
        [SerializeField] private string assemblyName;
        [SerializeField] private string storagePath;
        [SerializeField] private string hash;
        [SerializeField] private long sizeBytes;

        public int Id => id;

        /// <summary>
        /// HotUpdate_&lt;productGUID&gt;_&lt;fingerprint&gt;: the project, then a digest of the source
        /// it was compiled from. One name is one set of bytes, which is what lets a session hold
        /// two versions of the same project at once.
        /// </summary>
        public string AssemblyName => assemblyName;

        /// <summary>
        /// Folder under the storage root, holding <c>manifest.json</c> and one
        /// <c>&lt;Platform&gt;/&lt;AssemblyName&gt;.dll.bytes</c> per supported target.
        /// </summary>
        public string StoragePath => storagePath;

        /// <summary>
        /// sha256 of <c>manifest.json</c>. The only value the client trusts by itself: it
        /// arrives over the authenticated API, and everything downloaded from storage is
        /// verified against it, directly or through the manifest it pins.
        /// </summary>
        public string Hash => hash;

        public long SizeBytes => sizeBytes;
    }

    /// <summary>
    /// Contents of <c>manifest.json</c>: one sha256 per build target. HybridCLR compiles the
    /// assembly once per target, so the four DLLs never share a digest.
    /// </summary>
    [Serializable]
    public class EnvironmentDllManifest
    {
        public string AssemblyName { get; set; }

        /// <summary>Build target (Android, iOS, StandaloneWindows64, WebGL) -> sha256.</summary>
        public System.Collections.Generic.Dictionary<string, string> Dll { get; set; }
    }
}

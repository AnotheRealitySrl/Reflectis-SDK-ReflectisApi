using System;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class FolderDTO
    {
        // Informazioni Base
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private string path;
        [SerializeField] private int level;
        [SerializeField] private int assetCount;

        // Proprietà del Proprietario
        [SerializeField] private string ownerNickname;
        [SerializeField] private int ownerId;

        // Permessi e Stato
        [SerializeField] private bool isPublic;
        [SerializeField] private bool canVisualize;
        [SerializeField] private bool canRead;
        [SerializeField] private bool canSpawn;
        [SerializeField] private bool canManage;
        [SerializeField] private bool canGenerate3D;

        // Metadati Temporali
        [SerializeField] private DateTime createdAt;
        [SerializeField] private DateTime updatedAt;

        // Gerarchia (Sottocartelle)
        [SerializeField] private List<FolderDTO> children;


        // --- Proprietà Pubbliche ---
        public int Id => id;
        public string Name => name;
        public string Path => path;
        public int Level => level;
        public int AssetCount => assetCount;
        public string OwnerNickname => ownerNickname;
        public int OwnerId => ownerId;
        public bool IsPublic => isPublic;
        public bool CanVisualize => canVisualize;
        public bool CanRead => canRead;
        public bool CanSpawn => canSpawn;
        public bool CanManage => canManage;
        public bool CanGenerate3D => canGenerate3D;
        public DateTime CreatedAt => createdAt;
        public DateTime UpdatedAt => updatedAt;
        public List<FolderDTO> Children => children;
    }
}

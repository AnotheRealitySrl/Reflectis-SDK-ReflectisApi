using System;
using UnityEngine;
namespace Virtuademy.SDK.ApiData
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class LeaderboardRecordDTO
    {
        [SerializeField]
        private string leaderboardKey;
        [SerializeField]
        private float data;

        public LeaderboardRecordDTO(string leaderboardKey, float data)
        {
            this.leaderboardKey = leaderboardKey;
            this.data = data;
        }
    }
}
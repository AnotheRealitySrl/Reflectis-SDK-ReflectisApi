using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    public class TagSearchCriteria
    {
        [SerializeField] private string label;
        [SerializeField] private string color;
        [SerializeField] private int worldId;
        [SerializeField] private ESearchMode labelSearchMode;
        [SerializeField] private ESearchMode colorSearchMode;

        public string Label { get => label; set => label = value; }
        public string Color { get => color; set => color = value; }
        public int WorldId { get => worldId; set => worldId = value; }
        public ESearchMode LabelSearchMode { get => labelSearchMode; set => labelSearchMode = value; }
        public ESearchMode ColorSearchMode { get => colorSearchMode; set => colorSearchMode = value; }
    }
}

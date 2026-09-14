using UnityEngine;

#nullable enable
namespace Virtuademy.SDK.ApiData
{
    public class SubCategorySearchCriteria
    {
        [SerializeField] private string? label;
        [SerializeField] private ESearchMode labelSearchMode;
        [SerializeField] private int? userId;

        public string? Label { get => label; set => label = value; }
        public ESearchMode LabelSearchMode { get => labelSearchMode; set => labelSearchMode = value; }
        public int? UserId { get => userId; set => userId = value; }
    }
}

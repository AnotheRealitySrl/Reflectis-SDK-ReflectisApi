namespace Reflectis.SDK.ReflectisApi
{
    public class AssetGenerationStatusDTO
    {
        public AssetGenerationStatus status;
        public string errorMessage;
        public int id;
        public AssetDTO asset;
        public float progress;
    }

    public enum AssetGenerationStatus
    {
        INPROGRESS,
        SUCCESS,
        ERROR
    }
}

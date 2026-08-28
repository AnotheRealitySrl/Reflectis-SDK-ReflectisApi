namespace Virtuademy.SDK.PlatformApi
{
    public class AssetGenerationStatusDTO
    {
        public AssetGenerationStatus status;
        public string errorMessage;
        public int id;
        public AssetDTO asset;
        public float progress;
        public bool IsInProgress => status != AssetGenerationStatus.SUCCESS && status != AssetGenerationStatus.ERROR;
    }


    public enum AssetGenerationStatus
    {
        STARTING,
        IMAGE_GENERATION,
        STARTING_PUBLIC_MODEL_GENERATION,
        MODEL_GENERATION,
        SUCCESS,
        ERROR
    }
}

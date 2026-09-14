namespace Virtuademy.SDK.Interface
{
    /// <summary>
    /// Something the local user may or may not do. Ask through
    /// <see cref="IPlatformContext.IsGranted"/>.
    /// </summary>
    /// <remarks>
    /// <b>The member names are the wire contract.</b> The platform answers the permission endpoints
    /// with a list of identifier <em>strings</em>, and an adapter matches them against these names
    /// case-sensitively — so renaming a member here breaks it silently: the name stops matching, the
    /// permission stops being granted, and it reads as a permissions bug rather than as a rename.
    /// <para>
    /// The numeric values mirror the platform's own facet ids, which is why they are assigned
    /// explicitly and are not contiguous — 4, 5, 6, 8, 9, 10, 11 and 13 do not exist, having been
    /// retired server-side. Nothing matches on them today; they are here so a value seen in a
    /// database row or an API payload can be correlated with a member. Do not renumber them either,
    /// for that reason.
    /// </para>
    /// <para>
    /// Lifted here from a type nested inside a client model in the Worlds package. A permission is
    /// one of the few things an app genuinely has to reason about, and it cannot depend on that
    /// package in order to name one.
    /// </para>
    /// </remarks>
    public enum PlatformPermission
    {
        Unknown = 0,
        MuteOthers = 1,
        KickOthers = 2,
        UseTools = 3,
        SpawnFiles = 7,
        SendGlobalAndShardMessages = 12,
        SendAnnouncementMessages = 14,
        ManageMyExperiencesAndSessions = 15,
        EnableSpeaker = 16,
        UseAuthoringTool = 17,
        InteractWithAllObjects = 18,
        UseVoiceChat = 19,
        ManageMyEvents = 20,
        ManageMySessions = 21,
        ManageMyAuthoredExperiences = 22,
        ShowCatalogTab = 23,

        /// <summary>
        /// Plural, because the platform's identifier is. The member was <c>ShowLeaderboard</c> and
        /// therefore never matched: the seed row is <c>N'ShowLeaderboards'</c> and the live server
        /// sends that, so this one permission was silently dropped by every client that has ever
        /// parsed the list. It is the whole reason the names are the wire contract and not the
        /// numbers.
        /// </summary>
        ShowLeaderboards = 24,
        UseEmotes = 25,
        ShowLogo = 26,
        ShowTutorialButton = 27,
        EnableDoubleUserConnection = 28,
        ShowCompleteName = 29,
        UploadAssetsFolders = 30,
        ReadSharedAssetsFolders = 31,
        ManageSharedAssetsFolders = 32,
        GenerateAI3DAssets = 33,
    }
}

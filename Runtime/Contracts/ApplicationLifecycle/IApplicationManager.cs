using System.Threading.Tasks;

namespace Virtuademy.SDK.Core.ApplicationManagement
{
    public interface IApplicationManager
    {
        static IApplicationManager Instance { get; protected set; }

        /// <summary>
        /// True when Unity runs embedded inside a host application (the Vue
        /// landing) that owns shared transport — most notably the single
        /// Realtime WebSocket (ADR 0008). In embedded mode systems must route
        /// through the host bridge instead of opening their own connections.
        /// False for standalone runtimes (Editor, VR headsets, desktop), where
        /// Unity owns its own transport directly. Time-invariant for a given
        /// run: it reflects whether a host communication system is present, not
        /// a per-frame handshake state, so it is safe for systems to cache.
        /// </summary>
        bool IsEmbedded { get; }

        void QuitApplication();
        void ErasePlayerSessionData();
        Task<bool> CheckInternetConnection();
        string GetCurrentDevice();
    }
}

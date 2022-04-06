using UnityEngine;

namespace GameMain.Logging
{
    /// <summary>
    /// Used to replace log hanlder with Console Color LogHandler
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkHeadlessLogger")]
    public class NetworkHeadlessLogger : MonoBehaviour
    {
#pragma warning disable CS0414 // unused private members
        [SerializeField] bool showExceptionStackTrace = false;
#pragma warning restore CS0414 // unused private members

        void Awake()
        {
#if UNITY_SERVER
            LogFactory.ReplaceLogHandler(new ConsoleColorLogHandler(showExceptionStackTrace));
#endif
        }
    }
}

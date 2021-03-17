
namespace Ollio.Common.Enums
{
    // 1xxx: User error (next: 1004)
    // 2xxx: App error (next: 2003)
    public enum ExitStatus {
        Unknown = 9999,
        CannotLoadPlugin = 2001,
        FirstTimeLaunch = 1002,
        NoPluginsFound = 1001,
        NoConnectionsCreated = 1003
    }
}
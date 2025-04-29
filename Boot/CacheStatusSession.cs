namespace BodySound.Boot;

/// <summary>
/// Manages the session state regarding raid status and hook activation.
/// </summary>
public static class CacheStatusSession
{
    /// <summary>
    /// Indicates whether the player is currently inside a raid.
    /// This should be updated appropriately at the start and end of each raid.
    /// </summary>
    public static bool InRaid;
    /// <summary>
    /// Determines if hooks are allowed to execute based on the configuration.
    /// – If 'EnableAllHooksOnlyInRaid' is true, hooks are allowed only during raids.
    /// – If 'EnableAllHooksOnlyInRaid' is false, hooks are allowed everywhere (hideout, menus, raid).
    /// </summary>
    public static bool AllowHooks =>
        !Plugin.EnableAllHooksOnlyInRaid.Value || InRaid;
}

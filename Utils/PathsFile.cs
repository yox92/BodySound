using System.IO;

namespace BodySound.Utils
{
    public static class PathsFile
    {
        public static readonly string LogFilePath = Path.Combine(
            BepInEx.Paths.PluginPath, "BodySound", "body_log.txt");

        public static readonly string DebugPath = Path.Combine(
            BepInEx.Paths.PluginPath, "BodySound", "debug.cfg");
    }
}
using UnityEditor;

namespace Plugins.VersionTool.Processors
{
    public class VersionUpdateHelper
    {
        [InitializeOnEnterPlayMode]
        public static void Subscribe()
        {
            BuildVersionUpdater.CreateOrFindInProject().RefreshAndSave(false);
        }
    }
}
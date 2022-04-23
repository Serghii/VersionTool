using UnityEditor.Build.Reporting;

namespace Plugins.VersionTool.Processors
{
    public interface IBuildProcess
    {
        void OnPreprocessBuild(BuildReport report);
        void OnPostprocessBuild(BuildReport report, string outputPathOverride);
    }
}
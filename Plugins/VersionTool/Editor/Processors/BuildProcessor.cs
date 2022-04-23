using System;
using System.Collections.Generic;
using System.IO;
using Plugins.VersionTool.MenuItems;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Plugins.VersionTool.Processors
{
	public class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
	{
		private const string FILE_NAME_VERSION = "version.txt";

		public int callbackOrder
		{
			get { return int.MaxValue; }
		}

		private readonly Dictionary<BuildTargetGroup, IBuildProcess> _buildProcess;


		public BuildProcessor()
		{
			_buildProcess = new Dictionary<BuildTargetGroup, IBuildProcess>();
			RegisterProcessors();
		}

		private void RegisterProcessors()
		{
			_buildProcess.Add(BuildTargetGroup.WebGL, new BuildProcessorWebGL());
		}

		public void OnPreprocessBuild(BuildReport report)
		{
			BuildVersionUpdater.CreateOrFindInProject().RefreshAndSave();

			if (_buildProcess.ContainsKey(report.summary.platformGroup))
			{
				_buildProcess[report.summary.platformGroup].OnPreprocessBuild(report);
			}
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			BuildResult result = report.summary.result;
			if (result == BuildResult.Unknown && report.summary.totalErrors == 0)
				result = BuildResult.Succeeded;

			if (result == BuildResult.Succeeded)
			{
				BuildTargetGroup platformGroup = report.summary.platformGroup;

				// Rename build if needed
				string outputPath = EditorPrefs.GetBool(BuildPostProcessMenu.AutoRename, false)
					? RenameBuildWithVersion(report)
					: report.summary.outputPath;
				SetLastBuildPath(platformGroup, outputPath);
				File.WriteAllText(Path.Combine(outputPath, "..", FILE_NAME_VERSION),
					BuildVersionUpdater.CreateOrFindInProject().Version.Full);

				// Get the platform postprocessor
				if (_buildProcess.ContainsKey(report.summary.platformGroup))
				{
					IBuildProcess buildProc = _buildProcess[platformGroup];

					buildProc.OnPostprocessBuild(report, outputPath);
				}
			}

			LogBuildResult(report, result);
		}

		public static string GetLastBuildPath(BuildTargetGroup target)
		{
			return EditorPrefs.GetString(GetBuildPathKey(target), "");
		}

		private static void SetLastBuildPath(BuildTargetGroup target, string outputPath)
		{
			EditorPrefs.SetString(GetBuildPathKey(target), outputPath);
		}

		private static string GetBuildPathKey(BuildTargetGroup target)
		{
			return string.Concat("LastBuildPath_", target.ToString());
		}

		private static string RenameBuildWithVersion(BuildReport report)
		{
			string originalPath = report.summary.outputPath;

			string destPath = originalPath;
			string shortVersion = BuildVersionUpdater.CreateOrFindInProject().RefreshAndSave().Version.Full;

			if (!string.IsNullOrEmpty(shortVersion))
			{
				FileAttributes attr = File.GetAttributes(originalPath);

				if (attr.HasFlag(FileAttributes.Directory))
				{
					destPath += $" {shortVersion}";

					string tmpPath = destPath;
					int n = 1;
					while (Directory.Exists(tmpPath))
					{
						tmpPath = $"{destPath} ({n++})";
					}
					destPath = tmpPath;

					try
					{
						Directory.Move(originalPath, destPath);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
						Debug.LogError("Build version is not appended, the name of the build remains the same");
						return originalPath;
					}
				}
				else
				{
					destPath = Path.Combine(
						Path.GetDirectoryName(originalPath) ?? throw new InvalidOperationException($"Cannot get a directory name for path {originalPath}"),
						Path.GetFileNameWithoutExtension(originalPath) + $" {shortVersion}" + Path.GetExtension(originalPath));

					try
					{
						if (File.Exists(destPath))
							File.Delete(destPath);

						File.Move(originalPath, destPath);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
						Debug.LogError("Build version is not appended, the name of the build remains the same");
						return originalPath;
					}
				}
			}

			return destPath;
		}

		private static void LogBuildResult(BuildReport report, BuildResult result)
		{
			string color;
			switch (result)
			{
				case BuildResult.Succeeded:
					color = "green";
					break;

				case BuildResult.Failed:
					color = "red";
					break;

				default:
					color = "yellow";
					break;
			}

			string totalTime = (DateTime.UtcNow - report.summary.buildStartedAt).ToString(@"hh\:mm\:ss");

			Debug.Log($"<color={color}><b>[{result}]</b></color> Build finished\r\n<b>Total Build Time:</b> {totalTime}; <b>Build Size:</b> {EditorUtility.FormatBytes((long)report.summary.totalSize)}");
		}
	}
}
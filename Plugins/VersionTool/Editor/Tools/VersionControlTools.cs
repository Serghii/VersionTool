using UnityEngine;

namespace Plugins.VersionTool.Tools
{
    public static class VersionControlTools
    {
        public static string GetSVNVersion()
        {
            string version = ExecuteCommand("cd " + Application.dataPath + " && svn info --show-item last-changed-revision");
            version = version.Trim('\r', '\n', ' ');
            return version;
        }

        public static string GetGitVersion()
        {
            string version = ExecuteCommand("cd " + Application.dataPath + " && git log --format=format:\"%h (%ad)\" --date=short -n 1 HEAD");
            version = version.Trim('\r', '\n');
            return version;
        }

        public static bool IsGitRepository()
        {
            string res = ExecuteCommand("cd " + Application.dataPath + " && git rev-parse --is-inside-work-tree");
            res = res.Trim('\r', '\n');
            return res == "true";
        }


        private static string ExecuteCommand(string command)
        {
            try
            {
#if ( UNITY_EDITOR_WIN )
                return ExecuteCommandWin(command);

#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
			return ExecuteCommandUnix(command);
#else
			throw new NotImplementedException("Command prompt not implemented on this platform");
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            return string.Empty;
        }

        private static string ExecuteCommandWin(string command)
        {
            // '/c' tells cmd that we want it to execute the command that follows and then exit.
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

            // Redirect the all output to the streams.
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;

            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;


            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            proc.WaitForExit();
            string result = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError(error);
            }

            return result;
        }

        private static string ExecuteCommandUnix(string command)
        {
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("/bin/bash");
            procStartInfo.WorkingDirectory = "/";
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            proc.StandardInput.WriteLine(command);
            proc.StandardInput.WriteLine("exit"); // if no exit then WaitForExit will lockup your program
            proc.StandardInput.Flush();

            proc.WaitForExit();
            string result = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError(error);
            }

            return result;
        }
    }
}
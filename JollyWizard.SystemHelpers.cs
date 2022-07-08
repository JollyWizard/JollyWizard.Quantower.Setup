using System;

namespace JollyWizard.SystemHelpers;
using System.Diagnostics;

public class ProcessHelpers
{
    public static IEnumerable<Process> ProcessesContainingName(String processName) => Process.GetProcesses().Where(p => p.ProcessName.Contains(processName));

    public static IEnumerable<Process> ProcessesExactName(String processName) => Process.GetProcessesByName(processName);

    /// <summary>
    /// @Todo option for exact.
    /// </summary>
    /// <param name="ProcessName"></param>
    /// <returns></returns>
    public static bool CanDetect(String ProcessName) => ProcessesContainingName(ProcessName).Any();

    /// <summary>
    /// Often need to just cut to business and get the one process we know matches.
    /// </summary>
    /// <returns>The first process whose name matches `processName`. Null if not detected.</returns>
    public static Process? FirstProcessContainingName(String processName)
    {
        var processes = ProcessesContainingName(processName);
        return processes.Any() ? processes.First() : null;
    }

    /// <summary>
    /// *Warning* this can cause security errors.
    /// </summary>
    /// <param name="process"></param>
    /// <returns>The path for the process executable.</returns>
    public static String? ProcessPath(Process? process) => process?.MainModule?.FileName;

    /// <summary>
    /// *Warning* this can cause security errors.
    /// </summary>
    /// <param name="process"></param>
    /// <returns>The directory the process is executing in.</returns>
    public static String? ProcessDirectory(Process? process) => Path.GetDirectoryName(ProcessPath(process));
}

public class FolderBrowser
{
    /// <summary>
    /// If this value is set. Explore requests should be turned ignored.
    /// </summary>
    public static bool SUPRESS_EXPLORE_REQUESTS = false;

    /// <summary>
    /// Open a path in file explorer if it exists.
    /// </summary>
    /// <param name="dirPath">The directory path to browse.</param>
    /// <returns>`true` if the path exists and was opened, else false.</returns>
    public static bool ExploreIfExists(String? dirPath)
    {
        if (SUPRESS_EXPLORE_REQUESTS) return false;

        dirPath = Environment.ExpandEnvironmentVariables(dirPath ?? "");

        if (Directory.Exists(dirPath))
        {
            ProcessStartInfo startInfo = new()
            {
                Arguments = dirPath,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
            return true;
        }
        else
        {
            return false;
        }
    }
}
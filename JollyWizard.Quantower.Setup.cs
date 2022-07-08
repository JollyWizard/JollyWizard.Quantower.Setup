using System.Diagnostics;

using JollyWizard.SystemHelpers;

/// <summary>
/// A collection of methods and values to help detect and interface with the Quantower platform.
/// </summary>
namespace JollyWizard.Quantower.Setup;

/// <summary>
/// Methods to resolve path relationships 
/// </summary>
public class QuantowerPaths
{
    /// <summary>
    /// The relative path that takes
    /// </summary>
    public const String RELATIVE_PATH_PROCESS_TO_ROOT = "..\\..\\";
    public const String RELATIVE_PATH_ROOT_TO_CUSTOM_INDICATORS = ".\\Settings\\Scripts\\Indicators";

    public static String? ConvertProcessPathToRootPath(String? processDir)
    {
        if (processDir == null) return null;

        String rootRelativePath = Path.Combine(processDir, QuantowerPaths.RELATIVE_PATH_PROCESS_TO_ROOT);
        String resolvedPath = Path.GetFullPath(rootRelativePath);
        return resolvedPath;
    }

    public static String? ConvertRootToCustomIndicators(String? QuantowerRoot)
    {
        if (QuantowerRoot == null) return null;

        String indicatorsRelativePath = Path.Combine(QuantowerRoot, QuantowerPaths.RELATIVE_PATH_ROOT_TO_CUSTOM_INDICATORS);
        String resolvedPath = Path.GetFullPath(indicatorsRelativePath);
        return resolvedPath;
    }
}

/// <summary>
/// Methods / Values for reading and setting up Quantower related environment keys.
/// </summary>
public class EnvironmentConfig
    
{
    public static class Keys
    {
        public const String QuantowerRoot = "QuantowerRoot";
    }

    public static class Values
    {
        public static String? QuantowerRoot => Environment.GetEnvironmentVariable(Keys.QuantowerRoot);
    }

    public static class Setup
    {
        public static bool RootPath()
        {
            String? QuantowerRootPath = Utils.DetectQuantowerRootPath();
            if (QuantowerRootPath is not null)
            {
                // This is the session local env value.
                Environment.SetEnvironmentVariable(EnvironmentConfig.Keys.QuantowerRoot, QuantowerRootPath, EnvironmentVariableTarget.Process);

                // This is a system level env value, but stored per user.
                // b/c Quantower doesn't do a global install, this should be kept at the user level engaging with the application.
                // Visual Studio must be rebooted for MSBuild to see change. (@TODO does this change if fire event per `setx`?)
                Environment.SetEnvironmentVariable(EnvironmentConfig.Keys.QuantowerRoot, QuantowerRootPath, EnvironmentVariableTarget.User);

                return true;
            }
            return false;
        }
    }
}

/// <summary>
/// Methods for working with an active Quantower process.
/// </summary>
public class QuantowerProcess
{
    /// <summary>
    /// The name of the Quantower process.
    /// This is used to find the running process info.
    /// </summary>
    public const String PROCESS_NAME = "Starter";

    public static bool CanDetect() => ProcessHelpers.CanDetect(PROCESS_NAME);

    /// <summary>
    /// Get the `Process` for the currently running Quantower instance.
    /// </summary>
    /// /// @TODO Filter for false positives.
    /// <returns>The first process whose name matches the Quantower process name. Null if not detected.</returns>
    public static Process? GetProcess()
    {
        return ProcessHelpers.FirstProcessContainingName(PROCESS_NAME);
    }

    /// <summary>
    /// Gets the path of the active Quantower Executable.
    /// </summary>
    /// <returns>The path of the detected Quantower Executable OR `null` if not detected. </returns>
    public static String? ProcessPath() => ProcessHelpers.ProcessPath(GetProcess());

    /// <summary>
    /// The directory for the Active Quantower Executable.
    /// </summary>
    /// <returns></returns>
    public static String? ProcessDirectory() => ProcessHelpers.ProcessDirectory(GetProcess());
}


/// <summary>
/// Utility methods that end up in the general category.
/// </summary>
public class Utils
{
    /// <summary>
    /// Detect the root path of a currently running Quantower Instance.
    /// @TODO Offline Config.
    /// </summary>
    /// <returns></returns>
    public static String? DetectQuantowerRootPath()
    {
        String? processDir = QuantowerProcess.ProcessDirectory();
        
        // @TODO | Verify when this conversion is appropriate. Don't want to generate bad relative paths and dump files somewhere funny.
        return QuantowerPaths.ConvertProcessPathToRootPath(processDir);
    }

    public static bool ExploreQuantowerRoot() => FolderBrowser.ExploreIfExists(DetectQuantowerRootPath());

    public static bool ExploreCustomIndicators() => FolderBrowser.ExploreIfExists(DetectCustomIndicatorsPath());

    public static String? DetectCustomIndicatorsPath()
    {
        String? rootPath = DetectQuantowerRootPath();
        return QuantowerPaths.ConvertRootToCustomIndicators(rootPath);
    }

}
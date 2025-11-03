namespace ricaun.AppBundleTool.Utils;

/// <summary>
/// Provides utility methods for extracting name and semantic version information from file names.
/// </summary>
public static class NameAndVersionBundleUtils
{
    /// <summary>
    /// The regular expression pattern used to match file names with a specific format.
    /// </summary>
    /// <remarks>
    /// The pattern expects file names in the format: <c>{name}.{semanticVersion}.bundle</c>.
    /// - <c>{name}</c>: Represents the name of the file.
    /// - <c>{semanticVersion}</c>: Represents the semantic version, which includes at least three numeric segments and may include additional alphanumeric identifiers.
    /// </remarks>
    const string pattern = @"^(.*?)\.((?:\.?[0-9]+){3,}(?:[-a-z0-9]+?\.?)*)\.bundle$";

    /// <summary>
    /// Attempts to extract the name and semantic version from a given file name.
    /// </summary>
    /// <param name="fileName">The file name to parse.</param>
    /// <param name="name">The extracted name, if the operation is successful; otherwise, <c>null</c>.</param>
    /// <param name="semanticVersion">The extracted semantic version, if the operation is successful; otherwise, <c>null</c>.</param>
    /// <returns>
    /// <c>true</c> if the name and semantic version were successfully extracted; otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// string fileName = "MyApp.1.0.0-alpha.bundle";
    /// if (NameAndVersionBundleUtils.TryGetNameAndVersionBundle(fileName, out string name, out string version))
    /// {
    ///     Console.WriteLine($"Name: {name}, Version: {version}");
    /// }
    /// </code>
    /// </example>
    public static bool TryGetNameAndVersionBundle(string fileName, out string name, out string semanticVersion)
    {
        name = null;
        semanticVersion = null;

        var match = System.Text.RegularExpressions.Regex.Match(fileName, pattern);
        if (match.Success)
        {
            name = match.Groups[1].Value;
            semanticVersion = match.Groups[2].Value;
            return true;
        }

        return false;
    }
}
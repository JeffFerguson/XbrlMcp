using System.ComponentModel;
using System.Diagnostics;
using JeffFerguson.Gepsio;
using ModelContextProtocol;
using ModelContextProtocol.Server;

/// <summary>
/// Provides tools for working with XBRL document instances, including validation and fragment counting.
/// </summary>
[McpServerToolType]
public static class XbrlTool
{
    private static XbrlDocument? xbrlDoc = null;

    /// <summary>
    /// Checks an XBRL document instance, located by a supplied URL, for validity against the XBRL specification.
    /// </summary>
    /// <param name="progress">A progress reporter for notifications during processing.</param>
    /// <param name="path">The URL or file path to the XBRL document instance.</param>
    /// <returns>A message indicating whether the document is valid, or listing validation errors.</returns>
    [McpServerTool, Description("Checks an XBRL document instance, located by a supplied URL, for validity against the XBRL specification.")]
    public static async Task<string> IsValid(IProgress<ProgressNotificationValue> progress, string path)
    {
        await EnsureXbrlDocumentLoaded(progress, path);
        if (xbrlDoc != null && xbrlDoc.IsValid == true)
        {
            return "According to Gepsio, the XBRL document is valid.";
        }
        var invalidMessage = "According to Gepsio, the XBRL document is invalid. The errors are as follows:\n\n";
        if (xbrlDoc != null)
        {
            foreach (var error in xbrlDoc.ValidationErrors)
            {
                invalidMessage += $"- {error.Message}\n";
            }
        }
        else
        {
            invalidMessage += "- Unable to load XBRL document.\n";
        }
        return invalidMessage;
    }

    /// <summary>
    /// Outputs the number of fragments in an XBRL document instance, located by a supplied URL.
    /// </summary>
    /// <param name="progress">A progress reporter for notifications during processing.</param>
    /// <param name="path">The URL or file path to the XBRL document instance.</param>
    /// <returns>A message indicating the number of fragments in the XBRL document.</returns>
    [McpServerTool, Description("Outputs the number of fragments in an XBRL document instance, located by a supplied URL.")]
    public static async Task<string> FragmentsCount(IProgress<ProgressNotificationValue> progress, string path)
    {
        await EnsureXbrlDocumentLoaded(progress, path);
        if (xbrlDoc == null)
        {
            return "Unable to load XBRL document.";
        }
        if (xbrlDoc.XbrlFragments.Count == 1)
        {
            return "There is one fragment in the XBRL document.";
        }
        return $"There are {xbrlDoc.XbrlFragments.Count} fragments in the XBRL document.";
    }

    /// <summary>
    /// Ensures that the XBRL document is loaded from the specified path, reloading if necessary.
    /// </summary>
    /// <param name="progress">A progress reporter for notifications during processing.</param>
    /// <param name="path">The URL or file path to the XBRL document instance.</param>
    /// <returns>A task representing the asynchronous load operation.</returns>
    private static async Task EnsureXbrlDocumentLoaded(IProgress<ProgressNotificationValue> progress, string path)
    {
        var watch = new Stopwatch();
        watch.Start();
        if (xbrlDoc == null)
        {
            progress.Report(new() { Progress = 0, Message = $"Loading new XBRL document {path}." });
            xbrlDoc = new XbrlDocument();
            await xbrlDoc.LoadAsync(path);
        }
        else if (xbrlDoc.Filename != path)
        {
            progress.Report(new() { Progress = 0, Message = $"Loading different XBRL document {path}." });
            xbrlDoc = new XbrlDocument();
            await xbrlDoc.LoadAsync(path);
        }
        else
        {
            progress.Report(new() { Progress = 100, Message = $"Examining already-loaded XBRL document {path}." });
        }
        watch.Stop();
        progress.Report(new() { Progress = 100, Message = $"XBRL document processing completed in {watch.ElapsedMilliseconds} ms." });
    }
}

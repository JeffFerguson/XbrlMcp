using System.ComponentModel;
using JeffFerguson.Gepsio;
using ModelContextProtocol.Server;

[McpServerToolType]
public static class XbrlTool
{
    private static XbrlDocument? xbrlDoc = null;

    [McpServerTool, Description("Checks an XBRL document instance, located by a supplied URL, for validity against the XBRL specification.")]
    public static async Task<string> IsValid(string path)
    {
        await EnsureXbrlDocumentLoaded(path);
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

    [McpServerTool, Description("Outputs the number of fragments in an XBRL document instance, located by a supplied URL.")]
    public static async Task<string> FragmentsCount(string path)
    {
        await EnsureXbrlDocumentLoaded(path);
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

    private static async Task EnsureXbrlDocumentLoaded(string path)
    {
        // Lazy load the XBRL document only when needed.
        if (xbrlDoc == null)
        {
            xbrlDoc = new XbrlDocument();
            await xbrlDoc.LoadAsync(path);
        }
        else if (xbrlDoc.Filename != path)
        {
            // If the path is different from the currently loaded document, reload it.
            xbrlDoc = new XbrlDocument();
            await xbrlDoc.LoadAsync(path);
        }
    }
}

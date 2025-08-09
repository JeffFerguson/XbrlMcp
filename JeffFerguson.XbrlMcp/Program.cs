using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using JeffFerguson.Gepsio;
using System.Threading.Tasks;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();

[McpServerToolType]
public static class XbrlTool
{
    [McpServerTool, Description("Checks an XBRL document instance, located by a supplied URL, for validity against the XBRL specification.")]
    public static async Task<string> IsValid(string path)
    {
        var xbrlDoc = new XbrlDocument();
        await xbrlDoc.LoadAsync(path);
        if (xbrlDoc.IsValid == true)
        {
            return "According to Gepsio, the XBRL document is valid.";
        }
        var invalidMessage = "According to Gepsio, the XBRL document is invalid. The errors are as follows:\n\n";
        foreach (var error in xbrlDoc.ValidationErrors)
        {
            invalidMessage += $"- {error.Message}\n";
        }
        return invalidMessage;
    }
}
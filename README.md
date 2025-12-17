![PipeDream Logo Banner](assets/logo_banner.png)

![GitHub Release](https://img.shields.io/github/v/release/ryanmichaeljames/pipedream-plugins?style=flat&logo=github&color=33CE57)
![NuGet Version](https://img.shields.io/nuget/v/PipeDream.Plugins?logo=nuget&color=33CE57)
[![Build](https://github.com/ryanmichaeljames/pipedream-plugins/actions/workflows/build.yml/badge.svg)](https://github.com/ryanmichaeljames/pipedream-plugins/actions/workflows/build.yml)
[![CodeQL](https://github.com/ryanmichaeljames/pipedream-plugins/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/ryanmichaeljames/pipedream-plugins/actions/workflows/github-code-scanning/codeql)

> Custom PluginBase, service extensions, and utilities for building robust Dataverse plugins.

## Installation

```powershell
dotnet add package PipeDream.Plugins
```

## Features

- Enhanced PluginBase with error handling and context management
- Structured logging to Plugin Trace Logs and Application Insights
- Context extensions for images, attributes, stages, and messages
- Type-safe constants for messages, stages, modes, and image names
- Access to all IPluginExecutionContext interfaces (1-7)
- Helper properties for Target, PreImage, PostImage

## Usage

### Basic Plugin

```csharp
public class MyPlugin : PluginBase
{
    public MyPlugin(string unsecureConfig, string secureConfig) 
        : base(typeof(MyPlugin))
    {
    }

    protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
    {
        var context = localPluginContext.PluginExecutionContext;
        var service = localPluginContext.OrganizationService;
        
        // Your plugin logic here
    }
}
```

### Context Extensions

**Check stage and message:**
```csharp
if (context.IsPreOperation() && context.IsUpdate())
{
    // Handle pre-update logic
}
```

**Get entity images:**
```csharp
var preImage = context.GetPreImage<Account>();
var postImage = context.GetPostImage<Account>();
```

**Get final attribute values:**
```csharp
// Gets from Target first, falls back to PreImage
var name = context.GetFinalAttributeValue<string>("name");
```

**Check attribute changes:**
```csharp
if (context.HasAttributeChanged("statecode"))
{
    var oldValue = context.GetOldAttributeValue<OptionSetValue>("statecode");
    var newValue = context.GetNewAttributeValue<OptionSetValue>("statecode");
}
```

### Constants

Use built-in constants instead of magic strings:

```csharp
using PipeDream.Plugins.Constants;

// Stage values
if (context.Stage == Stage.PreOperation) { }

// Message names
if (context.MessageName == Message.Update) { }

// Execution modes
if (context.Mode == Mode.Synchronous) { }

// Image names
var image = context.GetPreImage(ImageName.PreImage);
```

### Execution Context Interfaces

The `LocalPluginContext` provides access to all execution context versions:

**IPluginExecutionContext** - Base context with:
- InputParameters, OutputParameters, SharedVariables
- PreEntityImages, PostEntityImages
- Stage, Depth, MessageName, PrimaryEntityName

**IPluginExecutionContext2** - Adds:
- `IsPortalsClientCall` - Detect Power Pages requests
- `PortalsContactId` - Get authenticated portal user
- Azure AD object IDs

**IPluginExecutionContext3** - Adds:
- `AuthenticatedUserId` - For impersonation scenarios

**IPluginExecutionContext4** - Adds:
- `PreEntityImagesCollection`, `PostEntityImagesCollection` - For bulk operations

**IPluginExecutionContext5** - Adds:
- `InitiatingUserAgent` - Browser/client detection

**IPluginExecutionContext6** - Adds:
- `EnvironmentId`, `TenantId` - Multi-tenant scenarios

**IPluginExecutionContext7** - Adds:
- `IsApplicationUser` - Distinguish service principal vs human user

**Usage:**
```csharp
// Check if called from Power Pages
if (localPluginContext.PluginExecutionContext2.IsPortalsClientCall)
{
    var portalContactId = localPluginContext.PluginExecutionContext2.PortalsContactId;
}

// Detect application users (service principals)
if (localPluginContext.PluginExecutionContext7.IsApplicationUser)
{
    // Handle automation differently
}
```

### Logging

The context provides structured logging to both Plugin Trace Logs and Application Insights.

> **Note:** All log methods (`LogInfo`, `LogWarning`, `LogError`) automatically write to both the Plugin Trace Log with time deltas and Application Insights. Use `Trace()` for trace-only logging without Application Insights.

**LogInfo(message)** - Informational logging:
```csharp
localPluginContext.LogInfo("Processing started");
// Trace: [+5ms] - [ExecuteDataversePlugin] - Processing started
// Application Insights: Info level telemetry
```

**LogWarning(message)** - Warning-level logging:
```csharp
localPluginContext.LogWarning("Skipping invalid record");
// Trace: [+20ms] - WARNING: [ExecuteDataversePlugin] - Skipping invalid record
// Application Insights: Warning level telemetry
```

**LogError(message)** - Error-level logging:
```csharp
localPluginContext.LogError("Failed to update");
// Trace: [+35ms] - ERROR: [ExecuteDataversePlugin] - Failed to update
// Application Insights: Error level telemetry
```

**LogError(exception, message)** - Structured exception logging:
```csharp
try
{
    // Your code
}
catch (Exception ex)
{
    localPluginContext.LogError(ex, "Operation failed");
    // Trace: Full exception details with stack trace
    // Application Insights: Structured exception with telemetry
    throw;
}
```

All log methods automatically include the calling method name via `[CallerMemberName]`.

### Custom Logging Implementation

Override the `CreateLocalPluginContext` method to inject your own logging service:

**1. Create an ILogger adapter for your custom logging service:**
```csharp
using Microsoft.Xrm.Sdk.PluginTelemetry;

public class CustomLoggerAdapter : ILogger
{
    private readonly ITelemetryHelper _telemetryHelper;

    public CustomLoggerAdapter(ITelemetryHelper telemetryHelper)
    {
        _telemetryHelper = telemetryHelper;
    }

    public void LogInformation(string message)
    {
        _telemetryHelper.Trace(message);
    }

    public void LogWarning(string message)
    {
        _telemetryHelper.Trace($"WARNING: {message}");
    }

    public void LogError(string message)
    {
        _telemetryHelper.Trace($"ERROR: {message}");
    }

    public void LogError(Exception exception, string message)
    {
        _telemetryHelper.Exception(exception, new Dictionary<string, string> 
        { 
            { "message", message } 
        });
    }
}
```

**2. Create a custom base class that injects your logger:**
```csharp
public abstract class CustomPluginBase : PluginBase
{
    protected CustomPluginBase(Type pluginClassName) : base(pluginClassName)
    {
    }

    protected override ILocalPluginContext CreateLocalPluginContext(IServiceProvider serviceProvider)
    {
        // Initialize your custom logging service
        var envVarService = new EnvironmentVariableService(serviceProvider);
        var telemetryHelper = new TelemetryHelper(envVarService);
        var customLogger = new CustomLoggerAdapter(telemetryHelper);

        // Inject custom logger into context
        return new LocalPluginContext(serviceProvider, customLogger);
    }
}
```

**3. Inherit from your custom base class:**
```csharp
public class MyPlugin : CustomPluginBase
{
    public MyPlugin(string unsecureConfig, string secureConfig) 
        : base(typeof(MyPlugin))
    {
    }

    protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
    {
        // Logging automatically uses your custom implementation
        localPluginContext.LogInfo("Using custom logging");
    }
}
```

**Note:** If Application Insights is not configured in your environment, the default `ILogger` from the service provider will be null and logging will only write to Plugin Trace Logs.

### Tracing and Performance Diagnostics

The `LocalTracingService` automatically prefixes traces with time deltas for performance diagnostics:

```csharp
protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
{
    var tracing = localPluginContext.TracingService;
    
    tracing.Trace("Starting plugin execution");
    // Output: [+0ms] - Starting plugin execution
    
    // Your logic here
    
    tracing.Trace("Completed business logic");
    // Output: [+42ms] - Completed business logic
    
    tracing.Trace("Processing {0} records", recordCount);
    // Output: [+15ms] - Processing 5 records
}
```

Each trace shows milliseconds elapsed since the previous trace, making it easy to identify performance bottlenecks.

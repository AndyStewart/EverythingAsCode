using System.Collections.Generic;
using EverythingAsCode.Infrastructure;
using Pulumi.AzureNative.App;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new Environment();
    var env = new ManagedEnvironment(
        "env",
        new ManagedEnvironmentArgs { ResourceGroupName = resourceGroup.ResourceGroup.Name }
    );

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?> { ["primaryStorageKey"] = "" };
});


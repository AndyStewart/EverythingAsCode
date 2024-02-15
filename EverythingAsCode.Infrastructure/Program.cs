using System.Collections.Generic;
using EverythingAsCode.Infrastructure;
using Pulumi.AzureNative.App;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var environment = new Environment();
    var containerEnv = new ManagedEnvironment(
        "env",
        new ManagedEnvironmentArgs { ResourceGroupName = environment.ResourceGroup.Name }
    );

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?> { ["primaryStorageKey"] = "" };
});

using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using System.Collections.Generic;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new Environment();

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?>
    {
        ["primaryStorageKey"] = primaryStorageKey
    };
});

public class Environment {

    public ResourceGroup ResourceGroup { get; }
    
    public Environment()
    {
        ResourceGroup = new ResourceGroup("resourceGroup");
    }
}
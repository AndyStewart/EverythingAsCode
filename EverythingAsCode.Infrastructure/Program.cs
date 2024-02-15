using System.Collections.Generic;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web.V20230101;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new Environment();
    var kubeEnv = new KubeEnvironment(
        "env",
        new KubeEnvironmentArgs
        {
            ResourceGroupName = resourceGroup.ResourceGroup.Name,
            EnvironmentType = "Managed"
        }
    );

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?> { ["primaryStorageKey"] = "" };
});

public class Environment
{
    public ResourceGroup ResourceGroup { get; }

    public Environment()
    {
        ResourceGroup = new ResourceGroup("resourceGroup");
    }
}


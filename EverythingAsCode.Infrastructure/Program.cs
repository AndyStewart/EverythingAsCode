using System.Collections.Generic;
using EverythingAsCode.Infrastructure;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var environment = new Environment();
    var containerEnv = new ManagedEnvironment(
        "env",
        new ManagedEnvironmentArgs { ResourceGroupName = environment.ResourceGroup.Name }
    );

    // Add a azure container app to deploy a docker container called andy/geoff:latest into the containerEnv
    var containerApp = new ContainerApp(
        "containerApp",
        new ContainerAppArgs
        {
            ResourceGroupName = environment.ResourceGroup.Name,
            Location = environment.ResourceGroup.Location,
            EnvironmentId = containerEnv.Id,
            Template = new TemplateArgs
            {
                Containers = new ContainerArgs[] {
                    new ContainerArgs {
                        Name = "andygeoff",
                        Image = "andy/geoff:latest",
                    }
                }
            }
        }
    );

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?> { ["primaryStorageKey"] = "" };
});

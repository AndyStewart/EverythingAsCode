using System.Collections.Generic;
using EverythingAsCode.Infrastructure;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.AppPlatform;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ManagedIdentity;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var environment = new Environment();
    var containerEnv = new ManagedEnvironment(
        "env",
        new ManagedEnvironmentArgs { ResourceGroupName = environment.ResourceGroup.Name }
    );

    var containerRegistry = ContainerRegistry.Get("andystewartregistry", "andystewartregistry");

    var identity = new UserAssignedIdentity(
        "identity",
        new UserAssignedIdentityArgs { ResourceGroupName = environment.ResourceGroup.Name }
    );

    var acrRole = new RoleAssignment(
        "acrRole",
        new RoleAssignmentArgs
        {
            PrincipalId = identity.PrincipalId,
            RoleAssignmentName = "acrpull",
            Scope = containerRegistry.Id
        },
        new CustomResourceOptions { Parent = containerEnv }
    );

    var containerApp = new ContainerApp(
        "cap-everythingascode",
        new ContainerAppArgs
        {
            ResourceGroupName = environment.ResourceGroup.Name,
            Location = environment.ResourceGroup.Location,
            EnvironmentId = containerEnv.Id,
            Identity = new ManagedServiceIdentityArgs
            {
                Type = ManagedServiceIdentityType.UserAssigned,
                UserAssignedIdentities = { identity.Id },
            },
            Template = new TemplateArgs
            {
                Containers = new ContainerArgs[] {
                    new ContainerArgs {
                        Name = "everythingascode",
                        Image = "andystewartregistry.azurecr.io/my-app:1.0",
                    }
                }
            }
        }
    );

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?> { ["primaryStorageKey"] = "" };
});

using System.Collections.Generic;
using EverythingAsCode.Infrastructure;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ManagedIdentity;
using ManagedServiceIdentityType = Pulumi.AzureNative.App.ManagedServiceIdentityType;

return await Pulumi.Deployment.RunAsync(async () =>
{
    // Create an Azure Resource Group
    var environment = new Environment();
    var containerEnv = new ManagedEnvironment(
        "env",
        new ManagedEnvironmentArgs { ResourceGroupName = environment.ResourceGroup.Name }
    );

    var containerRegistry = await GetRegistry.InvokeAsync(new GetRegistryArgs
    {
        RegistryName = "andystewartregistry",
        ResourceGroupName = "rg-andy-infrastructure"
    });

    var identity = new UserAssignedIdentity(
        "identity",
        new UserAssignedIdentityArgs { ResourceGroupName = environment.ResourceGroup.Name }
    );

    var acrRole = new RoleAssignment(
        "acrRole",
        new RoleAssignmentArgs
        {
            PrincipalId = identity.PrincipalId,
            RoleDefinitionId = "/providers/Microsoft.Authorization/roleDefinitions/7f951dda-4ed3-4680-a7ca-43fe172d538d",
            Scope = containerRegistry.
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

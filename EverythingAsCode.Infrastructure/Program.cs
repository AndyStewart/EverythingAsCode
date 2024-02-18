using System.Collections.Generic;
using EverythingAsCode.Infrastructure;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ManagedIdentity;

return await Deployment.RunAsync(async () =>
{
    var environment = new Environment();
    var containerEnv = new ManagedEnvironment(
        "env",
        new ManagedEnvironmentArgs { ResourceGroupName = environment.ResourceGroup.Name }
    );

    var containerRegistry = await GetRegistry.InvokeAsync(
        new GetRegistryArgs
        {
            RegistryName = "andystewartregistry",
            ResourceGroupName = "rg-andy-infrastructure"
        }
    );

    var identity = new UserAssignedIdentity(
        "identity",
        new UserAssignedIdentityArgs { ResourceGroupName = environment.ResourceGroup.Name }
    );

    var acrPull1 = new AcrPull(identity, containerRegistry);

    var imageName = "andystewartregistry.azurecr.io/my-app:1.0";
    var containerApp = new ContainerApp(
        "cap-everythingascode",
        new ContainerAppArgs
        {
            Configuration = new ConfigurationArgs
            {
                Registries = new[]
                {
                    new RegistryCredentialsArgs
                    {
                        Server = containerRegistry.LoginServer,
                        Identity = identity.Id
                    }
                },
                Ingress = new IngressArgs
                {
                    External = true,
                    AllowInsecure = false,
                    TargetPort = 8080
                },
            },
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
                Containers = new ContainerArgs[]
                {
                    new() { Name = "everythingascode", Image = imageName }
                },
            },
        }
    );

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?> { ["primaryStorageKey"] = "" };
});

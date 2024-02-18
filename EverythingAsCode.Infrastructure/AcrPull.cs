using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ManagedIdentity;

namespace EverythingAsCode.Infrastructure;

internal class AcrPull
{
    private readonly RoleAssignment acrRole;

    public AcrPull(UserAssignedIdentity identity, GetRegistryResult containerEnv) { }
}

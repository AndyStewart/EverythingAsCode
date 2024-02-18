using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ManagedIdentity;

namespace EverythingAsCode.Infrastructure;

internal class AcrPull
{
    private readonly RoleAssignment acrRole;

    public AcrPull(UserAssignedIdentity identity, GetRegistryResult containerEnv)
    {
        acrRole = new RoleAssignment(
            "AcrRoleAssignment",
            new RoleAssignmentArgs
            {
                PrincipalId = identity.PrincipalId,
                PrincipalType = "ServicePrincipal",
                RoleDefinitionId =
                    "/subscriptions/466a09cb-2d6e-4824-9190-47a90985f8b6/providers/Microsoft.Authorization/roleDefinitions/7f951dda-4ed3-4680-a7ca-43fe172d538d",
                Scope = containerEnv.Id
            },
            new() { DeleteBeforeReplace = true }
        );
    }
}

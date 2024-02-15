using Pulumi.AzureNative.Resources;

namespace EverythingAsCode.Infrastructure;

public class Environment
{
    public ResourceGroup ResourceGroup { get; }

    public Environment()
    {
        ResourceGroup = new ResourceGroup("resourceGroup");
    }
}

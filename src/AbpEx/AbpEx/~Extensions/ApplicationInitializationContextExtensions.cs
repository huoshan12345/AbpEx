using Volo.Abp;

namespace AbpEx;

public static class ApplicationInitializationContextExtensions
{
    public static T? GetObject<T>(this ApplicationInitializationContext context)
    {
        return context.ServiceProvider.GetObject<T>();
    }
}
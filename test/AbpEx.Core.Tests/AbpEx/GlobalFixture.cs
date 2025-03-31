using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace AbpEx;

public class GlobalFixture
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ThreadPool.SetMinThreads(100, 100);
#pragma warning disable SYSLIB0014
        ServicePointManager.DefaultConnectionLimit = short.MaxValue;
#pragma warning restore SYSLIB0014
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
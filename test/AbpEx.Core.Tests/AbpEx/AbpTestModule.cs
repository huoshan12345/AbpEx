using Volo.Abp.Modularity;

namespace AbpEx;

[DependsOn(typeof(AbpExModule))]
public class AbpTestModule : AbpModule;
namespace AbpEx;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (Debugger.IsAttached == false)
        //    Debugger.Launch();

        context.RegisterImplementationSourceOutput(context.CompilationProvider, (ctx, value) =>
        {
            const string abpCoreAssemblyName = "Volo.Abp.Core";
            var abpCoreAssembly = value.SourceModule.ReferencedAssemblySymbols.FirstOrDefault(a => a.Name == abpCoreAssemblyName);
            if (abpCoreAssembly is null)
            {
                Report("Cannot find referenced assembly: {0}", abpCoreAssemblyName);
                return;
            }

            var assembly = value.AssemblyName;
            SourceInfo[] codes = assembly switch
            {
                "AbpEx.Core" =>
                [
                    AbpCoreUsingsSource.Generate(abpCoreAssembly),
                ],
                _ => [],
            };

            if (codes.Any(m => m.Success == false))
                return;

            if (codes.Length == 0)
            {
                Report("No code generated for assembly: {0}", assembly);
                return;
            }

            foreach (var (_, file, code) in codes)
            {
                ctx.AddSource(file, code);
            }

            void Report(string messageFormat, params object?[]? args)
            {
                var descriptor = new DiagnosticDescriptor(
                    id: "FclEx",
                    title: nameof(context.RegisterImplementationSourceOutput),
                    messageFormat: messageFormat,
                    category: nameof(SourceGenerator),
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true);
                ctx.ReportDiagnostic(Diagnostic.Create(descriptor, null, messageArgs: args));
            }
        });
    }
}
using System.Reflection;

[assembly: AssemblyCompany("Squared Infinity")]
[assembly: AssemblyCopyright("Copyright © Squared Infinity")]

#pragma warning disable 1699  // warning CS1699: Use command line option '/keyfile' or appropriate project settings instead of 'AssemblyKeyFile'
[assembly: AssemblyKeyFile(@"c:\!\StartSSL Class 2 - Jaroslaw Kardas.snk")]
#pragma warning restore 1699

#if PRIMITIVES
    [assembly: AssemblyVersion("1.6.0.0")]
#elif DISPOSABLES
    [assembly: AssemblyVersion("1.6.0.0")]
#elif EXTENSIONS
    [assembly: AssemblyVersion("1.6.0.0")]
#elif THREADING
    [assembly: AssemblyVersion("1.6.0.0")]
#elif THREADING_LOCKS
    [assembly: AssemblyVersion("1.6.0.0")]
#endif
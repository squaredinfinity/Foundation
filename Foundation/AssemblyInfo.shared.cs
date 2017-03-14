using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: AssemblyCompany("Squared Infinity Limited")]
[assembly: AssemblyCopyright("Copyright © 2014")]

[assembly: AssemblyVersion("1.1.5.8")]
[assembly: AssemblyFileVersion("1.1.5.8")]

[assembly: XmlnsDefinition(@"http://schemas.squaredinfinity.com/foundation/windows", "SquaredInfinity.Presentation.Xaml.Styles.Modern.Windows")]

#if SIGN
#pragma warning disable 1699  // warning CS1699: Use command line option '/keyfile' or appropriate project settings instead of 'AssemblyKeyFile'
[assembly: AssemblyKeyFile(@"c:\!\StartSSL Class 2 - Jaroslaw Kardas.snk")]
#pragma warning restore 1699
#endif

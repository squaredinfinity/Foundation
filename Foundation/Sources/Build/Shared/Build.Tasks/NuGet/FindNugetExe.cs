using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Build.Tasks.NuGet
{
    public class FindNugetExe : NuGetTask
    {
        [Output]
        public new string NuGetExePath { get; set; }

        protected override bool DoExecute()
        {
            var nugetexe_path = string.Empty;

            if(TryFindNuGetExePath(out nugetexe_path))
            {
                NuGetExePath = nugetexe_path;
                return true;
            }

            return false;
        }
    }
}

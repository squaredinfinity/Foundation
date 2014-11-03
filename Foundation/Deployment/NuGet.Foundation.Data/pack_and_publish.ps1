Param 
(
    [string] $nuget_fullPath,
    [string] $output_fullPath,
	[bool] $shouldPublish
)

$global:ExitCode = 1

function Log
{
    [cmdletbinding()]
    Param
    (
        [parameter(ValueFromPipeline=$true)]
        [array] $messages,
        
        [parameter()]
        [string] $level = 'Information',
        
        [parameter()]
        [string] $foreground = 'White'
   )
        begin {}
        
        process
        {
            if($messages.Length -eq 0)
            {
                return
            }
            
            foreach($m in $messages)
            {
                switch($level)
                {
                    'Information'
                    {
                        Write-Host ('{0}' -f , $m) -ForegroundColor $foreground
                    }
                }
            }

        }
        
        end{}
    }
    

function PrepareEnvironment
{
    begin{}    
    process
    {
        # Update NuGet.exe
        Set-ItemProperty $nuget_fullPath -Name IsReadOnly -Value $false
        
        Log "Updating NuGet..."
        
        Log(& $nuget_fullPath "update" "-Self")
        Log "NuGet Updated" -Foreground Green
        
    }    
    end {}
}

function CreatePackages
{
    begin {}
    process
    {
        Log "Executing NuGet Pack" -Foreground 'Yellow'

        $nuget_outputPath = $output_fullPath + "\NuGet"
        $symbols_outputPath = $output_fullPath + "\NuGet-Symbols"
        
        if(!(Test-Path -Path $nuget_outputPath ))
        {
            New-Item -ItemType directory -Path $nuget_outputPath
        }
        
        if(!(Test-Path -Path $symbols_outputPath ))
        {
            New-Item -ItemType directory -Path $symbols_outputPath
        }
        

        Log(& $nuget_fullPath "pack" "Package.nuspec" "-OutputDirectory" $nuget_outputPath -Verbosity Detailed)
        
        Log(& $nuget_fullPath "pack" "Package.symbols.nuspec" "-OutputDirectory" $symbols_outputPath -Symbols -Verbosity Detailed)
    }
    end{}
}

function PublishPackages
{
    begin {}
    process
    {
        Log "Publishing NuGet Packckages" -Foreground 'Yellow'

        $nuget_outputPath = $output_fullPath + "\NuGet"
        $symbols_outputPath = $output_fullPath + "\NuGet-Symbols"
        
        if(!(Test-Path -Path $nuget_outputPath ))
        {
            New-Item -ItemType directory -Path $nuget_outputPath
        }
        
        if(!(Test-Path -Path $symbols_outputPath ))
        {
            New-Item -ItemType directory -Path $symbols_outputPath
        }
        

		# this will publish both NuGet and Symbols Package
        $nuget_packages = Get-ChildItem $symbols_outputPath
        
        foreach($file in $nuget_packages)
        {
            Log( "Pushing package {0}" -f $file.Name)
            Log(& $nuget_fullPath "push" $file.FullName -Verbosity Detailed -Timeout 180)
        }
    }
    end{}
}


function PackOnly
{
    begin {}
    process
    {
        PrepareEnvironment
        CreatePackages
    }
    end {}
}

function PackAndPublish
{
    begin {}
    process
    {
        PrepareEnvironment
        CreatePackages
        PublishPackages
    }
    end {}
}

Log "CREATE NUGET PACKAGE" -Foreground 'Yellow'
Log "  Inputs:"
Log ("    nuget_fullPath {0}" -f, $nuget_fullPath)
Log ("    output_fullPath {0}" -f, $output_fullPath)
Log ("    shouldPublish {0}" -f, $shouldPublish)

if($shouldPublish -eq $True)
{
	PackAndPublish
}
else
{
	PackOnly
}

    
    Exit $global:ExitCode

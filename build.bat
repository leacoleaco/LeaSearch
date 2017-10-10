echo off

echo 删除程序
rem rd /s /q "output/Debug"


echo 开始编译程序

rem path %SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\

rem echo start >output\Build.log
rem %SYSTEMROOT%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe LeaSearch.sln /t:Rebuild /p:Configuration=Debug /p:TargetFrameworkVersion=v4.5 /p:DisableOutOfProcTaskHost=true 


rem %SYSTEMROOT%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe LeaSearch.sln /t:Build /p:Configuration=Release  /p:Platform="Any CPU";TargetFrameworkVersion=v4.5.2


"D:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe" /rebuild "Release|AnyCPU" LeaSearch.sln 

rem msbuild.exe LeaSearch.sln /t:Rebuild /p:Configuration=Release /p:VisualStudioVersion=14.0 /p:DisableOutOfProcTaskHost=true 
rem /l:FileLogger,Microsoft.Build.Engine;logfile=output\Build.log
 
::msbuild  .\src\ElectricManagement.Web\ElectricManagement.Web.csproj /t:ResolveReferences;Compile /t:_WPPCopyWebApplication /p:Configuration=Release /p:VisualStudioVersion=12.0 /p:WebProjectOutputDir=..\..\Release
::/l:FileLogger,Microsoft.Build.Engine;logfile=Build2.log
 
echo 编译结束

echo 打开目录
REM msbuild.exe Plugins\LeaSearch.Plugin.Baidu\LeaSearch.Plugin.Baidu.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.Calculator\LeaSearch.Plugin.Calculator.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.HelloWorld\LeaSearch.Plugin.HelloWorld.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.OpenUrl\LeaSearch.Plugin.OpenUrl.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.Programs\LeaSearch.Plugin.Programs.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.SystemControlPanel\LeaSearch.Plugin.SystemControlPanel.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true



%windir%\explorer.exe output\Release

pause
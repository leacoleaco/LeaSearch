echo off

echo 删除output 文件夹
rd /s /q "output"


echo 开始编译项目

path %SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\

echo start >output\Build.log
msbuild.exe LeaSearch.sln /t:Rebuild /p:Configuration=Release /p:VisualStudioVersion=14.0 /p:DisableOutOfProcTaskHost=true 
rem /l:FileLogger,Microsoft.Build.Engine;logfile=output\Build.log
 
::msbuild  .\src\ElectricManagement.Web\ElectricManagement.Web.csproj /t:ResolveReferences;Compile /t:_WPPCopyWebApplication /p:Configuration=Release /p:VisualStudioVersion=12.0 /p:WebProjectOutputDir=..\..\Release
::/l:FileLogger,Microsoft.Build.Engine;logfile=Build2.log
 
echo 编译当前项目完毕

echo 开始编译插件

REM msbuild.exe Plugins\LeaSearch.Plugin.Baidu\LeaSearch.Plugin.Baidu.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.Calculator\LeaSearch.Plugin.Calculator.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.HelloWorld\LeaSearch.Plugin.HelloWorld.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.OpenUrl\LeaSearch.Plugin.OpenUrl.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.Programs\LeaSearch.Plugin.Programs.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true
REM msbuild.exe Plugins\LeaSearch.Plugin.SystemControlPanel\LeaSearch.Plugin.SystemControlPanel.csproj /t:Build /p:Configuration=Release /p:DisableOutOfProcTaskHost=true



%windir%\explorer.exe output\Release

pause
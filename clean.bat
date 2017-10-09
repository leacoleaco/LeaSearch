echo off

echo 删除bin、obj 文件夹
start for /d /r . %%d in (bin,obj,_ReSharper.*,Backup*) do @if exist "%%d" rd /s /q "%%d"

echo 删除output 文件夹
rd /s /q "output"

exit
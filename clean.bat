echo off

echo ɾ��bin��obj �ļ���
start for /d /r . %%d in (bin,obj,_ReSharper.*,Backup*) do @if exist "%%d" rd /s /q "%%d"

echo ɾ��output �ļ���
rd /s /q "output"

exit
# syenvs

syenvs是一款基于命令行windows环境变量处理小工具。你可以通过在命令行中输入简单的命令查看和修改环境变量。

**特别注意：** 修改后需要重新打开命令行（或重启进程）环境变量才会生效

### 系统要求
[syenvs(.net4x)](https://github.com/zerowsl/syenvs/releases/download/untagged-8ac50300afda1028302e/syenvs-net4x.zip)\
适用winxp+，需要安装 .net framework4.0（win8以上系统自带）

[syenvs(.netcore2.0)](https://github.com/zerowsl/syenvs/releases/download/untagged-8ac50300afda1028302e/syenvs-netcore2.0.zip)\
适用win7+，需要安装dotnet core 2.0运行时

[syenvs(tonative)](https://pan.baidu.com/s/1o8xaxmi#list/path=%2Fgit%2FSysEnvVars%2Fsyenvs%2F0.3.0.0&parentPath=%2Fgit%2FSysEnvVars)\
使用NativeTool把mono打包进syenvs(.net4x)生成的单文件，适用winxp+，无需 .net framework 即可运行\
**关于 NativeTool：** [https://linuxdot.net/](https://linuxdot.net/)

### 使用说明
打开命令行（win7以上系统需要以管理员运行）

**特别注意：** 修改后需要重新打开命令行（或重启进程）环境变量才会生效

sample如下：
```shell
rem 查看help
$ syenvs help

rem 查看版本
$ syenvs -v

rem 查看环境变量，加上 -cu 表示当前用户的环境变量
$ syenvs ls
$ syenvs ls -cu Path

rem 添加环境变量，不会重复添加
$ syenvs add "JAVA_HOME" "C:\Program Files\Java\jdk"
$ syenvs add "CLASSPATH" ".;%%JAVA_HOME%%\lib\dt.jar%%JAVA_HOME%%\lib\tools.jar;"
$ syenvs add "Path" "%%JAVA_HOME%%\bin" "%%JAVA_HOME%%\jre\bin"

rem 删除环境变量，无视不存在的路径
$ syenvs del "Path" -cu "%ProgramFiles%\Microsoft VS Code\bin\\"
$ syenvs del "this_is_not_exists_var"
$ syenvs del ANDROID_HOME
```

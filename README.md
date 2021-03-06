# syenvs

syenvs是一款基于命令行windows环境变量处理小工具。你可以通过在命令行中输入简单的命令查看和修改环境变量。

**特别注意：** 修改后需要重新打开命令行（或重启进程）环境变量才会生效

### 系统要求
[syenvs(.net4x)](https://github.com/zerowsl/syenvs/releases)\
适用winxp+，需要安装 .net framework4.0（win8以上系统自带）

[syenvs(.netcore2.0)](https://github.com/zerowsl/syenvs/releases)\
适用win7+，需要安装dotnet core 2.0运行时 [https://www.microsoft.com/net/download/windows](https://www.microsoft.com/net/download/windows)

[syenvs(tonative)](https://github.com/zerowsl/syenvs/releases)\
使用NativeTool把mono打包进syenvs(.net4x)生成的单文件，适用winxp+，无需 .net framework 即可运行\
**关于 NativeTool：** [https://linuxdot.net/](https://linuxdot.net/)

### 使用说明
打开命令行（win7以上系统需要以管理员运行）

**特别注意：** 修改后需要重新打开命令行（或重启进程）环境变量才会生效

sample如下：

查看help
```cmd
$ syenvs help
```

查看版本
```cmd
$ syenvs -v
```

查看环境变量，加上 -cu 表示当前用户的环境变量
```cmd
$ syenvs ls
$ syenvs ls -cu Path
```

添加环境变量，不会重复添加
```cmd
$ syenvs add "JAVA_HOME" "C:\Program Files\Java\jdk"
$ syenvs add "CLASSPATH" ".;%%JAVA_HOME%%\lib\dt.jar%%JAVA_HOME%%\lib\tools.jar;"
$ syenvs add "Path" "%%JAVA_HOME%%\bin" "%%JAVA_HOME%%\jre\bin"
```

 直接设置环境变量，会覆盖原有变量，所以处理PATH时要小心
 ```cmd
 $ syenvs set "JAVA_HOME" "C:\Program Files\Java\jdk"
 ```

删除环境变量，无视不存在的路径
```cmd
$ syenvs del "Path" -cu "%ProgramFiles%\Microsoft VS Code\bin\\"
$ syenvs del "this_is_not_exists_var"
$ syenvs del ANDROID_HOME
```

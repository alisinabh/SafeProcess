# SafeProcess

A simple .Net process supervisor for windows applications.

## What can SafeProcess do?

It can help you with restarting a process when it crashes and monitoring the exit code!

## Configuration

Just copy SafeProcess.exe and make a file named `safeprocess.conf` beside it.

Config file structure is like:

```
# A commented line
[Process name without .exe]|[Process executable path to re-run on crashes]
# A new comment
[Process2 name without .exe]|[Process2 executable path to re-run on crashes]
```

You can now just run SafeProcess.exe

SafeProcess will not start your process in case if a process with same name already exists. Instead it will Wait for that process crash (exit) then re-run's it.
Means there is no need to start your processes with SafeProcess.

Have fun!

## Logs

SafeProcess will log all events in a directory beside safeprocess.exe named `SafeProcessLogs`

## Windows configuration

**Important: This application will not function correctly if you don't Follow this step.**

Make sure that `Prevent display of user interface for critical errors` from Group policies` is **Enabled**.

You can enable it by running `gpedit.msc` the go to `Computer Configuration > Administrative Templates > Windows Components > Windows Error Reporting`
Then you can double click on ``Prevent display of...``, Enable the option then click OK.

## License

This program is not licensed. Which means do WTF you like to do with it. I don't care...
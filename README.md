# TrayStarter
Minimize application to tray (for programs not supported)

How it works:
1. [Download latest release](https://github.com/AAndyProgram/TrayStarter/releases/latest)
2. Unzip and place this program at any folder you want.
3. Create the shortcut to this program (right mouse click - "Create Shortcut").
4. You can rename this shortcut and place to the desktop or anywhere else you want.
5. Edit this shortcut properties (right mouse click - "Properties").

You should change \<Target\> field by pattern: \<TrayStarter program\> \<Executing program\> \<Process Name\> \<Delay time\>

Arguments:
- ```TrayStarter program``` - path to TrayStarter application;
- ```Executing program``` - path to execution file of application you want to minimize to tray;
- ```Process Name``` - you can found process name of your application at "Windows Task Manager" or "Process Explorer";
- ```Delay time``` - (optional) argument for waiting time for program fully launched (in seconds).

Example.
I want to minimize "Signal" application to tray.
The \<Target\> should be looking like: ```"D:\Programs\TrayStarter.exe" "C:\Users\<YourUserName>\AppData\Local\Programs\signal-desktop\Signal.exe" Signal 30```

1. ```"D:\Programs\TrayStarter.exe"``` - path to TrayStarter program
2. ```"C:\Users\<YourUserName>\AppData\Local\Programs\signal-desktop\Signal.exe"``` - path to execution file of "Signal" application
3. ```Signal``` - name of "Signal" application process
4. ```30``` - 30 seconds for give the program time for fully loaded. Sometimes signal application need more time for load. In that case you should set up <delay time>

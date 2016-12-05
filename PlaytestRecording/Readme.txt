Installation goes as follows:
	- Add this repository to the root of a Unity project (as a submodule or just copy the files).
	- Add the Playtest Services prefab to a scene which is not unloaded. (DontDestroyOnLoad is deprecated)
	- In Playtest Services/Playtest Service set the Application Name to something appropriate. This name is included in the .zip file and will help you distinguish playtest data from different projects.
	- Host an upload handler on a webserver. An example is provided in Scripts/Server
	- Enter the URL of the directory to which you uploaded the .php file (omit the file name itself, Upload.php is assumed)
	- When you make a build, unpack the contents of the Plugins/obs.zip into the build's _Data/obs folder. This way your playtest build comes with the right version of OBS so users don't have to install it themselves. If you forget to do this, the build will not break but it will not record gameplay footage, only gather basic specs in a JSON file.
	
Then you can just run the game and it will record until the application quits.

An easy way to tell if it's recording is to look at the task bar/system tray, but OBS also lowers the volume of media players while recording, so that's also a giveaway. Quitting the application takes a bit longer too because it's uploading the .zip, which may take a while if the recording was quite long.


Thanks for trying out this plug-in. I hope it helps you playtest your game remotely.

Have fun,

Roy Theunissen
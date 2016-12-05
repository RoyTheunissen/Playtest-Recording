<?php
/*
	This is a basic example of a PHP script you can host on a web server.
	This includes no encryption and serves only as a demonstration.
	
	NOTE:	Make sure to enter the URL of the directory to which this file
			was uploaded into the HostingService in the Services prefab.
			
	NOTE:	If this doesn't work for you, make sure the php.ini file on 
			your webserver contains file_uploads = On
*/

$target_dir = "uploads/";
$target_file = $target_dir . basename($_FILES["fileToUpload"]["name"]);
$imageFileType = pathinfo($target_file,PATHINFO_EXTENSION);

// Check if it was intentionally submit.
if (isset($_POST["submit"]))
{
	move_uploaded_file($_FILES["fileToUpload"]["tmp_name"], $target_file);
	
	// Optional: print something to let you know it was accepted.
	echo "Wakarimashita.";
}
?>
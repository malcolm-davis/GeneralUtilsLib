GeneralUtilsLib
========

GeneralUtilsLib is a set of basic utilities classes for C#.

## Features

**ConfigurationUtils** - Set of configuration classes that reduces the work to define and verify configuration files.

- **ApplicationConfiguration class** - Set of access methods to the configuration values. 
The following is an example of accessing the Timeout value in the configuration file as an integer for ImageProcessingService.
```
Config file: ImageProcessingService.Timeout=10
Code:        config.Int(ImageProcessingService.Timeout)
```

- **ConfigurationRulesBuilder class** - ConfigurationRulesBuilder defines the values expected in the configuration file.  This feature allows developers to provide a way for admin personnel to setup and verify a configuration file.
```
ConfigurationRulesBuilder.NewBuilder("Image Processing Service")
    .SetConfigurationFolder(folder.containing.configuration)
    .AddField(ImageProcessingService.SourceFolder, Condition.Required, , ParamType.Folder)
    .AddField(ImageProcessingService.TargetFolder, Condition.Required, , ParamType.Folder)
    .AddField(ImageProcessingService.EmailOnError, Condition.Optional, ParamType.Email)
    .AddField(ImageProcessingService.Timeout, Condition.Optional, ParamType.Number)
    .Build();
```
<br/>

**CloseUtils** - Checks for none null streams prior to closing, and ignores any exceptions.<br/>

- **Dispose(Stream stream)** - Close stream.  Ignore null stream and exceptions.

- **Dispose(IDisposable stream)** - Compares 1 directory to another directory.  Compares folder structure, file name and size, no file content checking.
<br/>

**DirectoryUtils** - Utility for working with folders.  Additional features missing from the present dotnet Dirctory class.<br/>

- **Clean** - Rather than deleting the folder, remove all the contents of the folder and keep the folder intact.

- **CompareDirectories** - Compares 1 directory to another directory.  Compares folder structure, file name and size, no file content checking.

- **CopyDirectory** - Copies a directory to another location.  Folders will be created, and files overwritten if they already exist.

- **DeleteAll** - Deletes the specified directory and any subdirectories and files.

- **DeleteQuietly** - Deletes the specified directory and any subdirectories and files, never throwing an exeception.

- **VerifyFolder** - Verify folder exist, and that the calling application can read & write to folder.

- **CheckReadPermissionOnDir** - Verify that the application has read permissions to the folder.

- **CheckWritePermissionOnDir** - Verify that the application has write permissions to the folder.
<br/>

**MethodUtils** - A set of convenience methods to verify method arguments.  Throws an ArgumentNullException when the rules are violated.

- **NotNull** - A convenience method that ensures that the specified argument is not null.

- **NotNullOrEmpty** - A convenience method that ensures that the specified argument is not null, empty, or white spaces.
<br/>

**StringUtils** - StringUtils provides convenient methods for parsing numbers and booleans represented as a string.  These methods never throw an exception and provide a default value to return.

- **ParseInt** - Returns the int value for a string, never throws an exception, provides a default value option.

- **ParseBool** - Returns the boolean value for a string, and never throws an exception, 
	- Provides a default value option.
	- Parses all forms of true and 1, and all forms of false and 0.

- **IsBool** - Verify the value is a boolean.

- **NotNullOrEmpty** - A convenience method that ensures that the specified argument is not null, empty, or white spaces.
# NLog.Segregate.DP.Target

### Extends functionality of base «FileTarget» module: writes nlog entries to separate files (one log entry per file). Automatically creates zip-archive when log files count reaches value defined in nlog.config.

## Dependencies
+ .NET Standart (>= 2.0)
+ NLog (>= 4.5.9)

## Release history
**v.1.0**
* Initial release

## Usage

## Settings
**LogsDirectory**<br>
Directory for logs<br><br>
**FilesToArchive**<br>
Auto-archiving occurs when files count reach FilesToArchive value

## Example NLog config file:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	<extensions>
		<add assembly="NLog.Segregate.DP.Target"/>
	</extensions>
	
	<targets>
		<target name="segregate" xsi:type="Segregate" 
		        segregateFileName="logs\${longdate}.log" 
		        segregateArchiveFileName="logs\${gdc:item=DeviceID}.${longdate}.zip" 
		        filesToArchive="5" layout="${message}" />
		<target name="logconsole" xsi:type="Console" />
	</targets>

	<rules>
		<logger name="*" minlevel="Debug" writeTo="logconsole,segregate" />
	</rules>
</nlog>
```

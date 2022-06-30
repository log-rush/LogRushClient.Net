# LogRushClient.Net

LogRush Client for C#

## Basic Usage

install the LogRushClient via Nuget

```C#
using LogRushClient.Core;

// create Logger
// you can optionally provide an ID and Key to reuse an old Logger
var logger = new LogRushLogger("MyLogger", "http://my-logrush-server", "id", "key");

// log text to LogRush
logger.log("Hello World!"); 
```

## Log4Net Logger

add the following xml to your Log4Net config:
```xml
<appender name="logrush" type="LogRushClient.Log4Net.LogRushLogger, LogRushClient">
  <Alias value="Zwoo Backend Logger"/>
  <Server value="http://localhost:7000"/>
  <!-- <Id value="your-id"/> -->
  <!-- <Key value="your-key"/> -->
  <layout type="log4net.Layout.PatternLayout">
    <conversionPattern value="[%date] [%logger] [%level] %message%newline" />
  </layout>
</appender>
```

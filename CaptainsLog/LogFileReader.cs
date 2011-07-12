using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;

namespace CaptainsLog
{
  public class LogFileReader
  {
    public ICollection<LogEvent> Read(string fileName)
    {
      string fileContents = "";
      using (StreamReader sr = new StreamReader(File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)))
      {
        fileContents = sr.ReadToEnd();
      }
      var xmlString = "<log xmlns:log4j='http://logging.apache.org/log4j/'>" + fileContents + "</log>";

      var document = new XmlDocument();
      document.LoadXml(xmlString);
      var events = document.GetElementsByTagName("log4j:event");
      var logEvents = new List<LogEvent>();
      foreach (XmlNode eventNode in events)
      {       
        var logger = eventNode.Attributes["logger"].Value;

        var msSince1970 = long.Parse(eventNode.Attributes["timestamp"].Value);
        var timeStamp = new DateTime(1970, 1, 1).AddMilliseconds(msSince1970).ToLocalTime();

        var messageNode = eventNode["log4j:message"];

        var throwable = eventNode["log4j:throwable"];

        var locationNode = eventNode["log4j:locationInfo"];
        int line = 0;
        int.TryParse(locationNode.Attributes["line"].Value, out line);

        var location = new LocationInfo { 
          Class = locationNode.Attributes["class"].Value, 
          Method = locationNode.Attributes["method"].Value,
          File = locationNode.Attributes["file"].Value,
          Line = line
        };

        var logEvent = new LogEvent {
          Logger = logger,
          Timestamp = timeStamp,
          Level = eventNode.Attributes["level"].Value,
          Thread = eventNode.Attributes["thread"].Value,
          Message = messageNode.InnerText,
          Throwable = throwable != null ? throwable.InnerText : null,
          Location = location
        };

        logEvents.Add(logEvent);
      }
      return logEvents;
    }
  }
}

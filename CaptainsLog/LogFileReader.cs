using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

namespace CaptainsLog
{
  public class LogFileReader
  {
    public SortedSet<LogEvent> Read(string fileName)
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
      var logEvents = new SortedSet<LogEvent>(new LogEventComparer());
      foreach (XmlNode eventNode in events)
      {
        var logEvent = new LogEvent();
        logEvent.Logger = eventNode.Attributes["logger"].Value;
        var msSince1970 = long.Parse(eventNode.Attributes["timestamp"].Value);
        logEvent.Timestamp = new DateTime(1970, 1, 1).AddMilliseconds(msSince1970);
        logEvent.Level = eventNode.Attributes["level"].Value;
        logEvent.Thread = eventNode.Attributes["thread"].Value;


        var messageNode = eventNode["log4j:message"];
        logEvent.Message = messageNode.InnerText;

        var location = new LocationInfo();
        var locationNode = eventNode["log4j:locationInfo"];
        location.Class = locationNode.Attributes["class"].Value;
        location.Method = locationNode.Attributes["method"].Value;
        location.File = locationNode.Attributes["file"].Value;

        int line = 0;
        if(int.TryParse(locationNode.Attributes["line"].Value, out line)) {
          location.Line = line;
        }  

        logEvent.Location = location;

        logEvents.Add(logEvent);
      }
      return logEvents;
    }
  }
}

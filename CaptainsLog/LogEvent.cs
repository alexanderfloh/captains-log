﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptainsLog {
  public class LogEvent {
    private string _shortMessage;

    public string Level { get; set; }

    public string Logger { get; set; }
    public DateTime Timestamp { get; set; }
    public string Thread { get; set; }
    public string Message { get; set; }
    public string Throwable { get; set; }
    public LocationInfo Location { get; set; }


    public string ShortMessage {
      get { return _shortMessage ?? (_shortMessage = String.Concat(Message.TakeWhile(c => c != '\n'))); }
    }

    public override string ToString() {
      return "LogEvent[" + Level + ": " + Logger + " @" + Timestamp + " in thread " + Thread + "]";
    }
  }

  public class LogEventComparer : IComparer<LogEvent> {
    public int Compare(LogEvent lhs, LogEvent rhs) {
      if (lhs == null && rhs == null)
        return 0;
      if (lhs == null)
        return -1;
      if (rhs == null)
        return 1;
      return Comparer<DateTime>.Default.Compare(lhs.Timestamp, rhs.Timestamp);
    }
  }
}

﻿namespace CaptainsLog
{
  public class LocationInfo
  {
    public string Class { get; set; }
    public string Method { get; set; }
    public string File { get; set; }
    public int Line { get; set; }

    public override string ToString()
    {
      return Class + "#" + Method;
    }
  }
}

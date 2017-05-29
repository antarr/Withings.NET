using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Withings.NET.Models
{
  public class Measuregrp
  {
    public int attrib { get; set; }
    public int category { get; set; }
    public string date { get; set; }
    public string grpid { get; set; }
    public IList<WithingsMeasure> measures { get; set; }
  }
}
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Withings.NET.Models
{
  public class WithingsBody
  {
    public IList<Measuregrp> measuregrps { get; set; }
    public string timezone { get; set; }
    public string updatetime { get; set; }
  }
}
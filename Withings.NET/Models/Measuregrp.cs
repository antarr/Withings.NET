using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Withings.NET.Models
{
  [DataContract]
  public class Measuregrp
  {
    [DataMember(Name = "attrib")]
    public int Attrib { get; set; }
    [DataMember(Name = "category")]
    public int Category { get; set; }
    [DataMember(Name = "date")]
    public int Date { get; set; }
    [DataMember(Name = "grpid")]
    public int Grpid { get; set; }
    [DataMember(Name = "measures")]
    public IList<WithingsMeasure> Measures { get; set; }
  }
}
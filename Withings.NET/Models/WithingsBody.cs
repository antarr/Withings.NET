using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Withings.NET.Models
{
  [DataContract]
  public class WithingsBody
  {
    [DataMember(Name = "measuregrps")]
    public IList<Measuregrp> Measuregrps { get; set; }
    [DataMember(Name = "timezone")]
    public string Timezone { get; set; }
    [DataMember(Name = "updatetime")]
    public int Updatetime { get; set; }
  }
}
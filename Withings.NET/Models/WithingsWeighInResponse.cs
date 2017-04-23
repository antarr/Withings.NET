using System.Runtime.Serialization;

namespace Withings.NET.Models
{
  [DataContract]
  public class WithingsWeighInResponse
  {
    [DataMember(Name = "body")]
    public WithingsBody Body { get; set; }
    [DataMember(Name = "status")]
    public int Status { get; set; }
  }
}
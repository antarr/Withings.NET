using System.Runtime.Serialization;

namespace Withings.NET.Models
{
  [DataContract]
  public class WithingsMeasure
  {
    public string ReadingType { get; }
    public double ReadingValue { get; }
    [DataMember(Name = "type")]
    public int Type { get; set; }
    [DataMember(Name = "unit")]
    public int Unit { get; set; }
    [DataMember(Name = "value")]
    public int Value { get; set; }
  }
}
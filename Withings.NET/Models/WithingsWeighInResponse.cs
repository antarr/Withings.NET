using System.Runtime.Serialization;

namespace Withings.NET.Models
{
  [DataContract]
  public class WithingsWeighInResponse
  {
    public WithingsBody body { get; set; }
    public int status { get; set; }
  }
}
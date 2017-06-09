namespace Withings.NET.Models
{
  public class WithingsMeasure
  {
    public string ReadingType { get; }
    public double ReadingValue { get; }
    public int type { get; set; }
    public int unit { get; set; }
    public int value { get; set; }
  }
}
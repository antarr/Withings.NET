using System.Collections.Generic;

namespace Withings.NET.Models
{
  public class Data
  {
    public double calories { get; set; }
    public int steps { get; set; }
    public double distance { get; set; }
    public double metcumul { get; set; }
    public int effduration { get; set; }
    public int intensity { get; set; }
    public int hr_average { get; set; }
    public int hr_min { get; set; }
    public int hr_max { get; set; }
    public int hr_zone_0 { get; set; }
    public int hr_zone_1 { get; set; }
    public int hr_zone_2 { get; set; }
    public int hr_zone_3 { get; set; }
    public int wakeupduration { get; set; }
    public int lightsleepduration { get; set; }
    public int deepsleepduration { get; set; }
    public int wakeupcount { get; set; }
    public int durationtosleep { get; set; }
  }

  public class Series
  {
    public int id { get; set; }
    public int userid { get; set; }
    public int category { get; set; }
    public string timezone { get; set; }
    public int model { get; set; }
    public int attrib { get; set; }
    public int startdate { get; set; }
    public int enddate { get; set; }
    public string date { get; set; }
    public Data data { get; set; }
    public int modified { get; set; }
  }

  public class Body
  {
    public List<Series> series { get; set; }
    public bool more { get; set; }
  }

  public class WeighInResponse
  {
    public int status { get; set; }
    public Body body { get; set; }
  }

  public class Result
  {
    public int status { get; set; }
    public Body body { get; set; }
  }

  public class SleepMeasures
  {
    public Result result { get; set; }
    public int id { get; set; }
    public object exception { get; set; }
    public int status { get; set; }
    public bool isCanceled { get; set; }
    public bool isCompleted { get; set; }
    public int creationOptions { get; set; }
    public object asyncState { get; set; }
    public bool isFaulted { get; set; }
  }

  //public class Data
  //{
  //  public int wakeupduration { get; set; }
  //  public int lightsleepduration { get; set; }
  //  public int deepsleepduration { get; set; }
  //  public int wakeupcount { get; set; }
  //  public int durationtosleep { get; set; }
  //}


  public class SleepSummary
  {
    public Result result { get; set; }
    public int id { get; set; }
    public object exception { get; set; }
    public int status { get; set; }
    public bool isCanceled { get; set; }
    public bool isCompleted { get; set; }
    public int creationOptions { get; set; }
    public object asyncState { get; set; }
    public bool isFaulted { get; set; }
  }
}

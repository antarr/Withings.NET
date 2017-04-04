
.NET Client for interacting with Withing OAuth1 Api

[![Build status](https://ci.appveyor.com/api/projects/status/lw9pd7gbdjgck3sq?svg=true)](https://ci.appveyor.com/project/atbyrd/withings-net)
[![Coverage Status](https://coveralls.io/repos/github/atbyrd/Withings.NET/badge.svg?branch=master)](https://coveralls.io/github/atbyrd/Withings.NET?branch=master)
[![codecov](https://codecov.io/gh/atbyrd/Withings.NET/branch/master/graph/badge.svg)](https://codecov.io/gh/atbyrd/Withings.NET)
[![Documentation Status](https://readthedocs.org/projects/withingsnet/badge/?version=latest)](http://withingsnet.readthedocs.io/en/latest/?badge=latest)

[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg?style=plastic)](https://www.nuget.org/packages/Withing.NET)

## USAGE
Due to external dependencies, your callback url should include a username param i.e. http://localhost:49294/api/oauth/callback/{username} 

### All examples will use the Nancy Framework

#### Authorization - Getting user authorization url
```
Get["api/oauth/authorize", true] = async (nothing, ct) => 
{
   var url = await authenticator.UserRequstUrl("nancy_user").ConfigureAwait(true);
   new JsonRespons(url, new DefaultJsonSerializer());
}
```

## CHANGE LOG

Version: 2.1.0 |
Release Date: April 03, 2017 |
New Features |
Get Ability To Get Body Measures

Version: 2.0.0 |
Release Date: April 03, 2017 |
Breaking API Change |
GetActivityMeasures Now Accepts DateTimes Instead of Strings for Dates

Version: 1.1.29 |
Release Date:April 02, 2017 |
New Features |
Add Abiltity To Get Sleep Measures

Version: 1.1.27 |
Release Date:April 02, 2017 |
New Features |
Add Abiltity To Get Workout Data

Version: 1.1.26 |
Release Date:April 02, 2017 |
New Features |
Add Abiltity To Get Sleep Summary Over A Range Of Days

Version: 1.1.23 |
Release Date:April 01, 2017 |
New Features |
Add Abiltity To Get Activity Measures For A Specific Day

Version: 1.1.0 |
Release Date:April 01, 2017 |
New Features |
Add Abiltity To Get Activity Measures For A Date Range

Version: 1.0.0 |
Release Date:March 06, 2017 |
New Features |
Complete Authorization Process

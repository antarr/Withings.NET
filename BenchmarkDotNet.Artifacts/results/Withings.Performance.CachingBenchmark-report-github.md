```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
Intel Xeon Processor 2.30GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-EEGWIO : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

IterationCount=5  LaunchCount=1  WarmupCount=1

```
| Method    | Mean        | Error       | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|---------- |------------:|------------:|----------:|------:|-------:|-------:|----------:|------------:|
| NoCache   | 44,261.4 ns | 3,765.18 ns | 582.67 ns |  1.00 | 0.8545 | 0.3662 |   21209 B |        1.00 |
| WithCache |    469.7 ns |    34.68 ns |   5.37 ns |  0.01 | 0.0124 |      - |     296 B |        0.01 |

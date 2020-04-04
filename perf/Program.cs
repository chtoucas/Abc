// See LICENSE in the project root for license information.

namespace PerfTool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Columns;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Order;
    using BenchmarkDotNet.Running;
    using BenchmarkDotNet.Validators;

    public static class Program
    {
#if BENCHMARK_HARNESS
        public static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args, DefaultConfig.Instance.WithLocalSettings());
        }
#else
        public static void Main()
        {
            var config = GetCustomConfig(shortRunJob: false)
                .WithLocalSettings()
                //.With(new EtwProfiler())
                ;

            BenchmarkRunner.Run<Comparisons.SelectManyPerf>(config);
        }
#endif

        public static IConfig WithLocalSettings(this IConfig config)
        {
            var orderer = new DefaultOrderer(
                SummaryOrderPolicy.FastestToSlowest,
                MethodOrderPolicy.Alphabetical);

            return config
                .With(ExecutionValidator.FailOnError)
                .With(RankColumn.Roman)
                .With(BaselineRatioColumn.RatioMean)
                .With(orderer);
        }

        // No exporter, less verbose logger.
        public static IConfig GetCustomConfig(bool shortRunJob)
        {
            var defaultConfig = DefaultConfig.Instance;

            var config = new ManualConfig();
            config.Add(defaultConfig.GetAnalysers().ToArray());
            config.Add(defaultConfig.GetColumnProviders().ToArray());
            config.Add(defaultConfig.GetDiagnosers().ToArray());
            //config.Add(defaultConfig.GetExporters().ToArray());
            config.Add(defaultConfig.GetFilters().ToArray());
            config.Add(defaultConfig.GetHardwareCounters().ToArray());
            //config.Add(defaultConfig.GetJobs().ToArray());
            config.Add(defaultConfig.GetLogicalGroupRules().ToArray());
            //config.Add(defaultConfig.GetLoggers().ToArray());
            config.Add(defaultConfig.GetValidators().ToArray());

            config.UnionRule = ConfigUnionRule.AlwaysUseGlobal;

            if (shortRunJob)
            {
                config.Add(Job.ShortRun);
            }

            return config.With(new ConsoleLogger_());
        }

        private sealed class ConsoleLogger_ : ILogger
        {
            private const ConsoleColor DefaultColor = ConsoleColor.Gray;

            private static readonly Dictionary<LogKind, ConsoleColor> s_ColorScheme
                = new Dictionary<LogKind, ConsoleColor>
                {
                    { LogKind.Default, ConsoleColor.Gray },
                    { LogKind.Error, ConsoleColor.Red },
                    { LogKind.Header, ConsoleColor.Magenta },
                    { LogKind.Help, ConsoleColor.DarkGreen },
                    { LogKind.Hint, ConsoleColor.DarkCyan },
                    { LogKind.Info, ConsoleColor.DarkYellow },
                    { LogKind.Result, ConsoleColor.DarkCyan },
                    { LogKind.Statistic, ConsoleColor.Cyan },
                };

            private static volatile int s_Counter;

            public void Write(LogKind logKind, string text)
                => Write(logKind, text, Console.Write);

            public void WriteLine()
                => Console.WriteLine();

            public void WriteLine(LogKind logKind, string text)
                => Write(logKind, text, Console.WriteLine);

            public void Flush() { }

            private void Write(LogKind logKind, string text, Action<string> write)
            {
                // Fragile mais au moins ça supprime la plupart des messages que
                // je ne souhaite pas voir.
                if (logKind == LogKind.Default)
                {
                    Spin();
                    return;
                }

                var colorBefore = Console.ForegroundColor;

                try
                {
                    var color = s_ColorScheme.ContainsKey(logKind)
                        ? s_ColorScheme[logKind]
                        : DefaultColor;

                    if (color != colorBefore
                        && color != Console.BackgroundColor)
                    {
                        Console.ForegroundColor = color;
                    }

                    write(text);
                }
                finally
                {
                    if (colorBefore != Console.ForegroundColor
                        && colorBefore != Console.BackgroundColor)
                    {
                        Console.ForegroundColor = colorBefore;
                    }
                }
            }

            public static void Spin()
            {
                s_Counter++;
                switch (s_Counter % 4)
                {
                    case 0: Console.Write("-"); s_Counter = 0; break;
                    case 1: Console.Write("\\"); break;
                    case 2: Console.Write("|"); break;
                    case 3: Console.Write("/"); break;
                }

                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }
    }
}

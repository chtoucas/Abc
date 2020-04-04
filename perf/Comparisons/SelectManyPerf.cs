// See LICENSE in the project root for license information.

namespace PerfTool.Comparisons
{
    using System;

    using Abc;

    using BenchmarkDotNet.Attributes;

    [ShortRunJob]
    [MemoryDiagnoser]
    public partial class SelectManyPerf
    {
        public class MyItem
        {
            public int Id = 0;
            public string Name = String.Empty;
        }

        public class MyInfo
        {
            public int Id { get; set; }
            public string Description { get; set; } = String.Empty;
        }

        public class MyData
        {
            public int Id { get; set; }
            public string Name { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
        }
    }

    public partial class SelectManyPerf
    {
        private static readonly Maybe<MyItem> s_Outer =
            Maybe.SomeOrNone(new MyItem { Id = 1, Name = "Name" });

        private static readonly Maybe<MyInfo> s_Inner =
            Maybe.SomeOrNone(new MyInfo { Id = 1, Description = "Description" });

        [Benchmark(Baseline = true)]
        public static Maybe<MyData> SelectMany() =>
            from x in s_Outer
            from y in s_Inner
            where x.Id == y.Id
            select new MyData
            {
                Id = x.Id,
                Name = x.Name,
                Description = y.Description
            };

        [Benchmark]
        public static Maybe<MyData> Join() =>
            from x in s_Outer
            join y in s_Inner on x.Id equals y.Id
            select new MyData
            {
                Id = x.Id,
                Name = x.Name,
                Description = y.Description
            };
    }
}

using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Microbenchmarks
{
	public class SystemTextBench
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        private const string _jsonStringPascalCase = "{\"MyString\" : \"abc\", \"MyInteger\" : 123, \"MyList\" : [\"abc\", \"123\"]}";
        private const string _jsonStringCamelCase = "{\"myString\" : \"abc\", \"myInteger\" : 123, \"myList\" : [\"abc\", \"123\"]}";

        [Benchmark]
        public MyClass SystemTextCaseSensitive_Pascal()
        {
            return JsonSerializer.Deserialize<MyClass>(_jsonStringPascalCase);
        }

        [Benchmark]
        public MyClass SystemTextCaseInsensitive_Pascal()
        {
            return JsonSerializer.Deserialize<MyClass>(_jsonStringPascalCase, options);
        }

        [Benchmark]
        public MyClass SystemTextCaseSensitive_Camel()
        {
            return JsonSerializer.Deserialize<MyClass>(_jsonStringCamelCase);
        }

        [Benchmark]
        public MyClass SystemTextCaseInsensitive_Camel()
        {
            return JsonSerializer.Deserialize<MyClass>(_jsonStringCamelCase, options);
        }

        private string value;

        public SystemTextBench()
        {
            var name = this.GetType().Assembly
                .GetManifestResourceNames()
                .First(n => n.EndsWith("SystemTextBench_LargeDoc.json"));

            using var reader = new StreamReader(this.GetType().Assembly.GetManifestResourceStream(name));
            value = reader.ReadToEnd();
        }

        [Benchmark]
        public MyClass LargeDoc()
        {
            return JsonSerializer.Deserialize<MyClass>(value);
        }
    }

    public class MyClass
    {
        public int MyInteger { get; set; }

        public string MyString { get; set; }

        public List<string> MyList { get; set; }
    }
}



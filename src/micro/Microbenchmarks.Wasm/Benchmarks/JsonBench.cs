using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if NET5_0
using System.Text.Json;
#endif

namespace Microbenchmarks
{
	public class JsonBench
    {
        private string value;

        public JsonBench()
        {
            var name = this.GetType().Assembly
                .GetManifestResourceNames()
                .First(n => n.EndsWith("JsonBench_LargeDoc.json"));

            using var reader = new StreamReader(this.GetType().Assembly.GetManifestResourceStream(name));
            value = reader.ReadToEnd();
        }

#if NET5_0
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


        [Benchmark]
        public MyClass SystemText_LargeDoc()
        {
            return JsonSerializer.Deserialize<MyClass>(value);
        }
#endif

        [Benchmark]
        public MyClass JsonNet_LargeDoc()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<MyClass>(value);
        }
    }

    public class MyClass
    {
        public int MyInteger { get; set; }

        public string MyString { get; set; }

        public List<string> MyList { get; set; }
    }
}



// Suppress CS8600: Converting null literal or possible null value to non-nullable type
#pragma warning disable CS8600
// Suppress CS8602: Dereference of a possibly null reference
#pragma warning disable CS8602
// Suppress CS8604: Possible null reference argument
#pragma warning disable CS8604
using BnEGames.Cop.Processor;
using BnEGames.Cop.Processor.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Xunit;

namespace BnEGames.Cop.Tests
{
    public class JsonRefResolverTest
    {
        CopProcessor copProcessor = new CopProcessor();
        [Fact]
        public void TestOperationRunner()
        {
            string json = ReadFile("StringRef.json");
            JContainer container = JObject.Parse(json);
            copProcessor.Process(container);
        }

        [Fact]
        public void TestConditionRunner()
        {
            string json = ReadFile("CopPlan.json");
            JContainer container = JObject.Parse(json);
            copProcessor.Process(container);
        }

        [Fact]
        public void TestStringRefsWithinString()
        {
            string json = ReadFile("StringRef.json");
            JContainer container = JObject.Parse(json);
            JsonRefResolver resolver = new JsonRefResolver();
            resolver.Resolve(container);
            Assert.NotNull(container);
            
            JToken puid = container.SelectToken("$.operations[1].parameters.puid");
            Assert.NotNull(puid);
        Assert.Equal("30232", puid);

            JToken puidWithSuffix = container.SelectToken("$.operations[1].parameters.puidWithSuffix");
            Assert.NotNull(puidWithSuffix);
        Assert.Equal("30232suffix", puidWithSuffix);

            JToken puidWithPrefix = container.SelectToken("$.operations[1].parameters.puidWithPrefix");
            Assert.NotNull(puidWithPrefix);
        Assert.Equal("prefix30232", puidWithPrefix);

            JToken puidWithSuffixAndPrefix = container.SelectToken("$.operations[1].parameters.puidWithPrefixAndSuffix");
            Assert.NotNull(puidWithSuffixAndPrefix);
        Assert.Equal("prefix30232suffix", puidWithSuffixAndPrefix); 
            
            JToken entireOp = container.SelectToken("$.operations[1].parameters.entireOperation");
            Assert.NotNull(entireOp);
        Assert.Contains("{\"id\":\"xxx\",\"type\":\"RequestParameters\",\"payload\":{\"cxh-puid\":\"30232\",\"cxh-country\":\"US\"}}", RemoveWhitespace(entireOp.ToString()));
        }
        

        private string RemoveWhitespace(string text)
        {
            return text.Replace("\n", "").Replace("\r", "").Replace(" ", "");
        }
        private string ReadFile(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            var resourceName = resourceNames.Single(str => str.EndsWith(filename));
            if (resourceName == null)
            {
                Assert.True(false, $"Unable to read file {filename}");
            }
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                if(result == null)
                {
                    Assert.True(false, $"Unable to read file {filename}");
                }
                return result ?? "";
            }
        }
    }
}
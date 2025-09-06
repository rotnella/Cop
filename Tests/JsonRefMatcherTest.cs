using BnEGames.Cop.Processor.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace BnEGames.Cop.Tests
{
    public class JsonRefMatcherTest
    {
        [Theory]
        [InlineData("Two refs", "@<xxx::payload.cxh-puid>some string in between@<xyz::payload>", 2, "@<xxx::payload.cxh-puid>", new string[] { "xxx", "payload.cxh-puid" })]
        [InlineData("Single ref", "@<xxx::payload.cxh-puid>", 1, "@<xxx::payload.cxh-puid>", new string[] { "xxx", "payload.cxh-puid" })]
        [InlineData("Ref with suffix", "@<xxx::payload.cxh-puid>suffix", 1, "@<xxx::payload.cxh-puid>", new string[] { "xxx", "payload.cxh-puid" })]
        [InlineData("Ref with prefix", "prefix@<xxx::payload.cxh-puid>", 1, "@<xxx::payload.cxh-puid>", new string[] { "xxx", "payload.cxh-puid" })]
        [InlineData("Ref with prefix and suffix", "prefix@<xxx::payload.cxh-puid>suffix", 1, "@<xxx::payload.cxh-puid>", new string[] { "xxx", "payload.cxh-puid" })]
        [InlineData("Ref full object", "@<xxx::/>", 1, "@<xxx::/>", new string[] { "xxx", "/" })]
        [InlineData("Ref no path", "@<xxx::>", 1, "@<xxx::>", new string[] { "xxx::", "" })] //this will look like a bad ID.
        [InlineData("Ref no path extra delimitor", "@<xxx:::>", 1, "@<xxx:::>", new string[] { "xxx", ":" })] //this will expect id to be :
        [InlineData("Ref path is .", "@<xxx::.>", 1, "@<xxx::.>", new string[] { "xxx", "." })]
        [InlineData("Ref path is .", "@<xxx::?>", 1, "@<xxx::?>", new string[] { "xxx", "?" })]
        [InlineData("Ref no end", "@<xxx::/", 0, null, null)]
        [InlineData("Ref no start", "<xxx::/>", 0, null, null)]
        public void TestRegexMatching(string label, string value, int expectedMatchCount, string expectedMatch, string[] expectedMatchedIdRef)
        {
            JsonRefMatcher refMatcher = new JsonRefMatcher();
            MatchCollection matches = refMatcher.FindReferencesInString(value);
            Assert.NotNull(matches);
            Assert.Equal(expectedMatchCount, matches.Count);
            if (expectedMatchCount > 0)
            {
                Assert.Equal(matches[0].Value, expectedMatch);
                Assert.Equal(matches[0].Groups[3].Value, expectedMatchedIdRef[0]);
                Assert.Equal(matches[0].Groups[4].Value, expectedMatchedIdRef[1]);
            }
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BnEGames.Cop.Processor.Json
{
    public class JsonRefMatcher
    {
        public static string IdPropertyName = "id";
        public static string ReferencePrefix = "@<";
        public static string ReferenceSuffix = ">";
        public static string ReferenceSeparator = "::";
        //public const string FALLBACK_VALUE_SEPARATOR = "?";
        public static string ReferenceGroupName = "reference";
        public static string ReferencePath = "path";

        private readonly Regex _referencePattern;

        public JsonRefMatcher()
        {
            string pattern = string.Format($"({ReferencePrefix}(?<{ReferenceGroupName}>.+?)({ReferenceSeparator}(?<{ReferencePath}>.+?))?{ReferenceSuffix}).*?");
            _referencePattern = new Regex(@pattern);
        }

        public List<JsonRef> GetRefsForValue(string parentId, JValue valueInObjectWithRef)
        {
            MatchCollection matches = FindReferencesInString(valueInObjectWithRef.ToString());
            //there may be mulitple object references in a single object value
            List<JsonRef> refs = new List<JsonRef>(matches.Count);
            for (int matchIndex = 0; matchIndex < matches.Count; matchIndex++)
            {
                string refId = GetRefId(matches[matchIndex]);
                string refPath = GetRefPath(matches[matchIndex]);
                refs.Add(new JsonRef(matches[matchIndex].Value, refId, refPath, parentId, valueInObjectWithRef.Path));
            }
            return refs;
        }

        public MatchCollection FindReferencesInString(string value)
        {
            return _referencePattern.Matches(value);
        }

        public bool IsMatch(string value)
        {
           return _referencePattern.IsMatch(value);
        }
        public string GetRefId(Match match)
        {
            //found @<id::path> reference in value string
            //match.Groups [3] is id, [4] is path
            return match.Groups[ReferenceGroupName].Value;
        }
        public string GetRefPath(Match match)
        {
            //found @<id::path> reference in value string
            //match.Groups [3] is id, [4] is path
            return match.Groups[ReferencePath].Value;
        }
    }
}

using Newtonsoft.Json.Linq;

namespace BnEGames.Cop.Processor.Json
{
    public class JsonRef
    {
        public JsonRef(string refVariable, string refObjectId, string refPath, string sourceId, string sourceValuePath)
        {
            RefVariable = refVariable;
            RefObjectId = refObjectId;
            RefPath = refPath;
            SourceId = sourceId;
            SourceValuePath = sourceValuePath;
        }
        public string RefVariable { get; private set; }
        public string RefObjectId { get; private set; }
        public string RefPath { get; private set; }
        public string SourceId { get; private set; }
        public string SourceValuePath { get; private set; }
    }
}

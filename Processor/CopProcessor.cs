using BnEGames.Cop.Api;
using BnEGames.Cop.Processor.Json;
using BnEGames.Cop.Processor.Operation;
using Newtonsoft.Json.Linq;

namespace BnEGames.Cop.Processor
{
    public class CopProcessor
    {

        public IOperationRegistry Registry { get; set; }
        public JsonRefResolver RefResolver { get; set; }

        public CopProcessor() : this(new OperationRegistry()){}

        public CopProcessor(IOperationRegistry registry) 
        {
            this.Registry = registry;
            this.RefResolver = new JsonRefResolver();
        }

        public void Process(JContainer container)
        {
            RefResolver.Resolve(container);
            while (RefResolver.HasReadyObject())
            {
                JContainer readyOp = RefResolver.TakeReadyObject();
                
                if (readyOp == null)
                {
                    throw new Exception("Ready operation is null");
                }
                // Get the type string from the JContainer
                var typeToken = readyOp["type"];
                if (typeToken == null)
                {
                    throw new Exception("Operation type property missing");
                }
                string typeName = typeToken.Value<string>();
                var opType = Registry.GetOperationType(typeName);
                if (opType == null)
                {
                    throw new Exception($"Unknown operation type: {typeName}");
                }
                var operation = (Api.IOperation)readyOp.ToObject(opType);
                object? output = operation?.Compute();
                if (output != null)
                {
                    readyOp.Add(new JProperty("result", JValue.FromObject(output)));
                }
                RefResolver.ResolveFor(operation?.Id);
            }
        }
    }
}

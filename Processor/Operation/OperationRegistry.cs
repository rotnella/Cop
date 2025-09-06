using System;
using System.Collections.Generic;
using System.Text;
using BnEGames.Cop.Api;

namespace BnEGames.Cop.Processor.Operation
{
    public class OperationRegistry : IOperationRegistry
    {
        private readonly Dictionary<string, Type> registeredTypes = new Dictionary<string, Type>()
        {
            [typeof(EqOperation).FullName] = typeof(EqOperation)
        };

        public Type? GetOperationType(string typeName)
        {
            if (registeredTypes.TryGetValue(typeName, out var type))
            {
                return type;
            }
            return typeof(NoOpOperation);
        }

        public void RegisterOperationType(string key, Type type)
        {
            registeredTypes.Add(key, type);
        }
    }
}

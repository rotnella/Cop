using System;
using System.Collections.Generic;
using System.Text;

namespace BnEGames.Cop.Api
{
    public interface IOperationRegistry
    {
        public Type? GetOperationType(string typeName);
    }
}

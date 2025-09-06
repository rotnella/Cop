using System;

namespace BnEGames.Cop.Processor.Operation
{
    public class NoOpOperation : Api.Operation<object?, object?>
    {
        public override object? Compute()
        {
            return this.Result;
        }
    }
}

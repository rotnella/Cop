using System;
using System.Linq;

namespace BnEGames.Cop.Processor.Operation
{
    public class EqOperation : Api.Operation<EqInput, EqResult>
    {
        public override object? Compute()
        {
            //todo assert
            EqInput payload = Payload;
            if (payload is null || payload.Values == null || payload.Values.Count == 1)
            {
                Result = new EqResult(false);
            }
            else
            {
                bool solution = payload.Values.All(x => x == payload.Values.First());
                Result = new EqResult(solution);
                return Result;
            }
            return null;
        }
    }
    public class EqInput
    {
        public List<string> Values { get; set; } = new List<string>();
    }
    public class EqResult
    {
        public bool IsEqual { get; set; }
        public EqResult(bool isEqual)
        {
            IsEqual = isEqual;
        }
    }
}

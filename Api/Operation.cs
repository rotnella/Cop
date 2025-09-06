using System;
using System.Collections.Generic;

namespace BnEGames.Cop.Api
{
    public abstract class Operation<P, R> : IOperation
    {
        private string? _id;
        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }
        public virtual P? Payload { get; set; }
        public virtual R? Result { get; set; }

        object? IOperation.Payload => Payload;
        object? IOperation.Result => Result;
        public string? Type { get; set; }
        public string? Mode { get; set; }
        public string? Name { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }

        public abstract object? Compute();
    }
}

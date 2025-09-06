using System.Collections.Generic;

namespace BnEGames.Cop.Api
{
    public interface IOperation
    {
        string? Id { get; }
        object? Payload { get; }
        string? Mode { get; }
        string? Name { get; }
        object? Result { get; }
        Dictionary<string, object> Parameters { get; }
        string? Type { get; }

        object? Compute();

    }
}
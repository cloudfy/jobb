using System;

namespace Jobb.Schemas;

public sealed class GenerationProgressEventArgs : EventArgs
{
    internal GenerationProgressEventArgs(int total, int step)
    {
        Total = total;
        Step = step;
    }

    public int Total { get; }
    public int Step { get; }
}

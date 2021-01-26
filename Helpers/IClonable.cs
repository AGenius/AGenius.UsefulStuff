using System;

namespace AGenius.UsefulStuff.Helpers
{
    public interface ICloneable<T>
    {
        T Clone();
    }
}
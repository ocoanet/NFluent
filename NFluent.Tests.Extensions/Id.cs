using System;

namespace NFluent.Tests
{
    public class Id : IEquatable<Id>
    {
        public Id(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public bool Equals(Id other)
        {
            return other != null && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}
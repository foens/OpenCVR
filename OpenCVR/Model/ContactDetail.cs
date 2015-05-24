using System;

namespace OpenCVR.Model
{
    public class ContactDetail
    {
        public DateTime? ValidFrom { get; set; }
        public string Value { get; set; }

        protected bool Equals(ContactDetail other)
        {
            return ValidFrom.Equals(other.ValidFrom) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ContactDetail) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ValidFrom.GetHashCode()*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"ValidFrom: {ValidFrom}, Value: {Value}";
        }
    }
}
using System;

namespace OpenCVR.Update.Parse
{
    public class Industry
    {
        public DateTime ValidFrom { get; set; }
        public int Code { get; set; }
        public string Text { get; set; }

        protected bool Equals(Industry other)
        {
            return Code == other.Code && string.Equals(Text, other.Text) && ValidFrom.Equals(other.ValidFrom);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Industry) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Code;
                hashCode = (hashCode*397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ ValidFrom.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"ValidFrom: {ValidFrom}, Code: {Code}, Text: {Text}";
        }
    }
}
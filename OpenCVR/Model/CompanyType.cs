using System;

namespace OpenCVR.Model
{
    public class CompanyType
    {
        public DateTime? ValidFrom { get; set; }
        public int? Code { get; set; }
        public string Text { get; set; }
        public string ResponsibleDataSupplier { get; set; }

        protected bool Equals(CompanyType other)
        {
            return Code == other.Code && string.Equals(ResponsibleDataSupplier, other.ResponsibleDataSupplier) && string.Equals(Text, other.Text) && ValidFrom.Equals(other.ValidFrom);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CompanyType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Code.GetHashCode();
                hashCode = (hashCode*397) ^ (ResponsibleDataSupplier != null ? ResponsibleDataSupplier.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ ValidFrom.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"ValidFrom: {ValidFrom}, Code: {Code}, Text: {Text}, ResponsibleDataSupplier: {ResponsibleDataSupplier}";
        }
    }
}
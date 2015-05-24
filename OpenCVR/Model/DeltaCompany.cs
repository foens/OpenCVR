namespace OpenCVR.Model
{
    public class DeltaCompany : Company
    {
        public ModificationStatus ModificationStatus { get; set; }

        protected bool Equals(DeltaCompany other)
        {
            return base.Equals(other) && ModificationStatus == other.ModificationStatus;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeltaCompany) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (int) ModificationStatus;
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}, ModificationStatus: {ModificationStatus}";
        }
    }
}
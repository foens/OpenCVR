namespace OpenCVR.Model
{
    public class Interval
    {
        public int Min { get; set; }
        public int Max { get; set; }

        protected bool Equals(Interval other)
        {
            return Max == other.Max && Min == other.Min;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Interval) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Max*397) ^ Min;
            }
        }

        public override string ToString()
        {
            return $"Min: {Min}, Max: {Max}";
        }
    }
}
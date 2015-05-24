using System;

namespace OpenCVR.Update.Parse
{
    public class Address
    {
        public DateTime? ValidFrom { get; set; }
        public string StreetName { get; set; }
        public int? RoadCode { get; set; }
        public int? HouseNumberFrom { get; set; }
        public int? HouseNumberTo { get; set; }
        public string LetterFrom { get; set; }
        public string LetterTo { get; set; }
        public string Floor { get; set; }
        public string SideDoor { get; set; }
        public int? ZipCode { get; set; }
        public string PostalDisrict { get; set; }
        public string CityName { get; set; }
        public int? MunicipalityCode { get; set; }
        public string MunicipalityText { get; set; }
        public int? PostalBox { get; set; }
        public string CoName { get; set; }
        public string AddressFreeText { get; set; }

        protected bool Equals(Address other)
        {
            return string.Equals(AddressFreeText, other.AddressFreeText) && string.Equals(CityName, other.CityName) && string.Equals(CoName, other.CoName) && string.Equals(Floor, other.Floor) && HouseNumberFrom == other.HouseNumberFrom && HouseNumberTo == other.HouseNumberTo && string.Equals(LetterFrom, other.LetterFrom) && string.Equals(LetterTo, other.LetterTo) && MunicipalityCode == other.MunicipalityCode && string.Equals(MunicipalityText, other.MunicipalityText) && PostalBox == other.PostalBox && string.Equals(PostalDisrict, other.PostalDisrict) && RoadCode == other.RoadCode && string.Equals(SideDoor, other.SideDoor) && string.Equals(StreetName, other.StreetName) && ValidFrom.Equals(other.ValidFrom) && ZipCode == other.ZipCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (AddressFreeText != null ? AddressFreeText.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (CityName != null ? CityName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (CoName != null ? CoName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Floor != null ? Floor.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ HouseNumberFrom.GetHashCode();
                hashCode = (hashCode*397) ^ HouseNumberTo.GetHashCode();
                hashCode = (hashCode*397) ^ (LetterFrom != null ? LetterFrom.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LetterTo != null ? LetterTo.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ MunicipalityCode.GetHashCode();
                hashCode = (hashCode*397) ^ (MunicipalityText != null ? MunicipalityText.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ PostalBox.GetHashCode();
                hashCode = (hashCode*397) ^ (PostalDisrict != null ? PostalDisrict.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ RoadCode.GetHashCode();
                hashCode = (hashCode*397) ^ (SideDoor != null ? SideDoor.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (StreetName != null ? StreetName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ ValidFrom.GetHashCode();
                hashCode = (hashCode*397) ^ ZipCode.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"ValidFrom: {ValidFrom}, StreetName: {StreetName}, RoadCode: {RoadCode}, HouseNumberFrom: {HouseNumberFrom}, HouseNumberTo: {HouseNumberTo}, LetterFrom: {LetterFrom}, LetterTo: {LetterTo}, Floor: {Floor}, SideDoor: {SideDoor}, ZipCode: {ZipCode}, PostalDisrict: {PostalDisrict}, CityName: {CityName}, MunicipalityCode: {MunicipalityCode}, MunicipalityText: {MunicipalityText}, PostalBox: {PostalBox}, CoName: {CoName}, AddressFreeText: {AddressFreeText}";
        }
    }
}
using System;
using OpenCVR.Model.Occupation;

namespace OpenCVR.Model
{
    public class Company
    {
        public int VatNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool OptedOutForUnsolicictedAdvertising { get; set; }
        public DateTime? NameValidFrom { get; set; }
        public string Name { get; set; }
        public DateTime? AddressValidFrom { get; set; }
        public Address LocationAddress { get; set; }
        public Address PostalAddress { get; set; }
        public CompanyType CompanyType { get; set; }
        public Industry MainIndustry { get; set; }
        public ContactDetail TelephoneContactDetails { get; set; }
        public ContactDetail FaxContactDetails { get; set; }
        public ContactDetail EmailContactDetails { get; set; }
        public DateTime? CreditDetailsValidFrom { get; set; }
        public string CreditDetails { get; set; }
        public OccupationStatistics OccupationStatistics { get; set; }
        public long[] ProductionUnits { get; set; }
        public long[] Participants { get; set; }

        protected bool Equals(Company other)
        {
            return AddressValidFrom.Equals(other.AddressValidFrom) && Equals(CompanyType, other.CompanyType) && string.Equals(CreditDetails, other.CreditDetails) && CreditDetailsValidFrom.Equals(other.CreditDetailsValidFrom) && Equals(EmailContactDetails, other.EmailContactDetails) && EndDate.Equals(other.EndDate) && Equals(FaxContactDetails, other.FaxContactDetails) && Equals(LocationAddress, other.LocationAddress) && Equals(MainIndustry, other.MainIndustry) && string.Equals(Name, other.Name) && NameValidFrom.Equals(other.NameValidFrom) && Equals(OccupationStatistics, other.OccupationStatistics) && OptedOutForUnsolicictedAdvertising == other.OptedOutForUnsolicictedAdvertising && Equals(Participants, other.Participants) && Equals(PostalAddress, other.PostalAddress) && Equals(ProductionUnits, other.ProductionUnits) && StartDate.Equals(other.StartDate) && Equals(TelephoneContactDetails, other.TelephoneContactDetails) && UpdatedDate.Equals(other.UpdatedDate) && VatNumber == other.VatNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Company) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AddressValidFrom.GetHashCode();
                hashCode = (hashCode*397) ^ (CompanyType != null ? CompanyType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (CreditDetails != null ? CreditDetails.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CreditDetailsValidFrom.GetHashCode();
                hashCode = (hashCode*397) ^ (EmailContactDetails != null ? EmailContactDetails.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ EndDate.GetHashCode();
                hashCode = (hashCode*397) ^ (FaxContactDetails != null ? FaxContactDetails.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LocationAddress != null ? LocationAddress.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MainIndustry != null ? MainIndustry.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ NameValidFrom.GetHashCode();
                hashCode = (hashCode*397) ^ (OccupationStatistics != null ? OccupationStatistics.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ OptedOutForUnsolicictedAdvertising.GetHashCode();
                hashCode = (hashCode*397) ^ (Participants != null ? Participants.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PostalAddress != null ? PostalAddress.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ProductionUnits != null ? ProductionUnits.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ StartDate.GetHashCode();
                hashCode = (hashCode*397) ^ (TelephoneContactDetails != null ? TelephoneContactDetails.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ UpdatedDate.GetHashCode();
                hashCode = (hashCode*397) ^ VatNumber;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"VatNumber: {VatNumber}, StartDate: {StartDate}, EndDate: {EndDate}, UpdatedDate: {UpdatedDate}, OptedOutForUnsolicictedAdvertising: {OptedOutForUnsolicictedAdvertising}, NameValidFrom: {NameValidFrom}, Name: {Name}, AddressValidFrom: {AddressValidFrom}, LocationAddress: {LocationAddress}, PostalAddress: {PostalAddress}, CompanyType: {CompanyType}, MainIndustry: {MainIndustry}, TelephoneContactDetails: {TelephoneContactDetails}, FaxContactDetails: {FaxContactDetails}, EmailContactDetails: {EmailContactDetails}, CreditDetailsValidFrom: {CreditDetailsValidFrom}, CreditDetails: {CreditDetails}, OccupationStatistics: {OccupationStatistics}, ProductionUnits: {ProductionUnits}, Participants: {Participants}";
        }
    }
}
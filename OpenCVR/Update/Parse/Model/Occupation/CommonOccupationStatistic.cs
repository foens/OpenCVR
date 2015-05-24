namespace OpenCVR.Update.Parse.Model.Occupation
{
    public class CommonOccupationStatistic
    {
        public int Year { get; set; }
        public int Employees { get; set; }
        public Interval EmployeeInterval { get; set; }
        public int ManYears { get; set; }
        public Interval ManYearsInterval { get; set; }
    }
}
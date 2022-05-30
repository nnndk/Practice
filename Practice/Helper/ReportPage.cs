namespace Practice.Helper
{
    public class ReportPage
    {
        public string? ReportType { get; set; } = null;
        public int? DepId { get; set; } = null;
        public bool OnlyCurrEmployees { get; set; } = false;

        private DateTime startDate = new DateTime(2000, 1, 1).Date;

        public DateTime StartDate
        {
            get { return startDate.Date; }
            set { startDate = value.Date; }
        }

        private DateTime endDate = DateTime.Now.Date;

        public DateTime EndDate
        {
            get { return endDate.Date; }
            set { endDate = value.Date; }
        }

        public List<Dictionary<string, string>>? Report { get; set; } = null;

        public ReportPage() { }
    }
}

namespace CallCenter.UI.ViewModels
{
    public class IndexFilterViewModel
    {
        public string NameFilter { get; set; } = string.Empty;
        public int GenderFilter { get; set; }
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public int MinDaysAfterLastCall { get; set; }

        public int PageSize { get; set; } = 25;
        public int PageNo { get; set; } = 1;
        public int RecordsCount { get; set; } = 100;
    }
}
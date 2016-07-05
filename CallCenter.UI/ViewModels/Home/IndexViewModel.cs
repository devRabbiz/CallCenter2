namespace CallCenter.UI.ViewModels.Home
{
    public class IndexViewModel
    {
        public string NameFilter { get; set; } = string.Empty;
        public int GenderFilter { get; set; } = 0;
        public int? AgeMin { get; set; } = null;
        public int? AgeMax { get; set; } = null;
        public int MinDaysAfterLastCall { get; set; } = 0;

        public int PageSize { get; set; } = 25;
        public int PageNo { get; set; } = 1;
    }
}
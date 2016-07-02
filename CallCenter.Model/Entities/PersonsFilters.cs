using System;

namespace CallCenter.Model
{    
    public class PersonsFilters
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public Gender Gender { get; set; } = Gender.All;
        public string NameFilter { get; set; } = string.Empty;
        public int? MaxAge { get; set; } = null;
        public int? MinAge { get; set; } = null;
        public int MinDaysAfterLastCall { get; set; } = 0;
        public override string ToString()
        {
            return $"Filter fields: PageNo:{PageNo}; PageSize:{PageSize}; Gender:{Gender}; NameFilter:{NameFilter}; MaxAge:{MaxAge}; MinAge:{MinAge}; MinDaysAfterLastCall:{MinDaysAfterLastCall}";
        }
    }
}
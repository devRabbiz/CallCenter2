using System;

namespace CallCenter.Model.Services.DTO
{
    public class PersonsListItem
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
    }
}

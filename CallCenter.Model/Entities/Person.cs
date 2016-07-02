using System;
using System.Collections.Generic;

namespace CallCenter.Model
    {
    public enum Gender { All, Male, Femaile }
        
    public sealed class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }

        public IList<Call> Calls { get; set; }
    }
}

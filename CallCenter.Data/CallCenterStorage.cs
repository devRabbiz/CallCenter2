using CallCenter.Model;
using CallCenter.Model.Abstract;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace CallCenter.Data
{
    public class CallCenterStorage : DbContext, ICallCenterStorage
    {
        public IDbSet<Call> Calls { get; set; }
        public IDbSet<Person> Persons { get; set; }   
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Person>().Property(p => p.Id).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.PhoneNumber).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.FirstName).HasMaxLength(30);
            modelBuilder.Entity<Person>().Property(p => p.LastName).HasMaxLength(30);
            modelBuilder.Entity<Person>().Property(p => p.Patronymic).HasMaxLength(30);
            modelBuilder.Entity<Person>().Property(p => p.Gender).HasColumnType("INT");

            modelBuilder.Entity<Call>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Call>().Property(c => c.Id).IsRequired();
            modelBuilder.Entity<Call>().Property(c => c.CallDate).IsRequired();
            modelBuilder.Entity<Call>().Property(c => c.CallReport).IsRequired();

            modelBuilder.Entity<Person>().HasMany(p => p.Calls);            

            base.OnModelCreating(modelBuilder);
        }
       
        public CallCenterStorage(string connectionName) : base(string.Format("name={0}", connectionName))
        {
            Configuration.ProxyCreationEnabled = false;
        }           
    }
}

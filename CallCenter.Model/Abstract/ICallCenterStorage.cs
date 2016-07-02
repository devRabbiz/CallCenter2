using CallCenter.Model;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CallCenter.Model.Abstract
{
    public interface ICallCenterStorage : IDisposable
    {
        IDbSet<Call> Calls { get; set; }
        IDbSet<Person> Persons { get; set; }
        Task<int> SaveChangesAsync();
    }
}

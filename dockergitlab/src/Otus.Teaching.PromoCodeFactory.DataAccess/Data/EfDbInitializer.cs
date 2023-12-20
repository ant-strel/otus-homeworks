using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Data
{
    public class EfDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;

        public EfDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public void InitializeDb()
        {
            try
            {
                //_dataContext.Database.EnsureDeleted();
                //_dataContext.Database.EnsureCreated();
                _dataContext.Database.Migrate();
   

                _dataContext.AddRange(FakeDataFactory.Employees);
                _dataContext.SaveChanges();

                _dataContext.AddRange(FakeDataFactory.Preferences);
                _dataContext.SaveChanges();

                _dataContext.AddRange(FakeDataFactory.Customers);
                _dataContext.SaveChanges();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
using MongoDB.Driver;
using Otus.Teaching.Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Otus.Teaching.Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Otus.Teaching.Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<T>(typeof(T).Name);
        }
        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            await _collection.DeleteOneAsync(e => e.Id == entity.Id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
           var result = (await _collection.FindAsync(_ => true)).ToEnumerable();
            return result;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var result = await _collection.FindAsync(e => e.Id == id);
            return result.FirstOrDefault();
        }

        public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
        {
            var result = await _collection.FindAsync<T>(predicate);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        {
            var result = await _collection.FindAsync(e => ids.Contains(e.Id));
            return result.ToEnumerable();
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            var result = await _collection.FindAsync(predicate);
            return result.ToEnumerable();
        }

        public async Task UpdateAsync(T entity)
        {
            await _collection.ReplaceOneAsync(e =>e.Id == entity.Id, entity);
        }
    }
}

using DocSharingAPI.Contexts;
using DocSharingAPI.Interfaces;

namespace DocSharingAPI.Repositories
{
    public  abstract class Repository<K, T> : IRepository<K, T> where T:class
    {
        protected readonly DocumentContext _documentContext;

        public Repository(DocumentContext documentContext)
        {
            _documentContext = documentContext;
        }
        public async Task<T> Add(T item)
        {
            _documentContext.Add(item);
            await _documentContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item != null)
            {
                _documentContext.Remove(item);
                await _documentContext.SaveChangesAsync();
                return item;
            }
            throw new Exception("No such item found for deleting");
        }

        public abstract Task<T> Get(K key);


        public abstract Task<IEnumerable<T>> GetAll();


        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _documentContext.Entry(myItem).CurrentValues.SetValues(item);
                await _documentContext.SaveChangesAsync();
                return item;
            }
            throw new Exception("No such item found for updation");
        }
    }
}

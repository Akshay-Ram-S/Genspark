using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;

namespace FirstAPI.Repositories
{
    public  abstract class Repository<K, T> : IRepository<K, T> where T:class
    {
        protected readonly AuctionContext _auctionContext;

        public Repository(AuctionContext auctionContext)
        {
            _auctionContext = auctionContext;
        }
        public async Task<T> Add(T item)
        {
            _auctionContext.Add(item);
            await _auctionContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item != null)
            {
                _auctionContext.Remove(item);
                await _auctionContext.SaveChangesAsync();
                return item;
            }
            throw new IdNotFoundException("Item not found for deleting");
        }

        public abstract Task<T> Get(K key);


        public abstract Task<IEnumerable<T>> GetAll();


        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _auctionContext.Entry(myItem).CurrentValues.SetValues(item);
                await _auctionContext.SaveChangesAsync();
                return item;
            }
            throw new IdNotFoundException("Item not found for updation");
        }
    }
}

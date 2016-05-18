using System.Data.Entity;


namespace Crucial.Framework.Data.EntityFramework
{
    public interface IContextProvider<out T>
    {
        T DbContext { get; }
    }

    public class ContextProvider<T> : IContextProvider<T> where T : IDbContextAsync, new()
    {
        private T _context;

        public ContextProvider()
        {
            _context = new T();
        }

        public T DbContext
        {
            get
            {
                return _context;
            }
        }
    }
}

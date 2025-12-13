using System.Data.Entity;
using ShoeApp;

namespace ShoeApp.Data
{
    public static class Db
    {
        public static readonly ShoeEntity Context = new ShoeEntity();

        static Db()
        {
            Context.Configuration.LazyLoadingEnabled = false;
            Context.Configuration.ProxyCreationEnabled = false;
        }

        public static void Save() => Context.SaveChanges();
    }
}

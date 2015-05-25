#if (__MonoCS__)
using SQLiteTransaction = Mono.Data.Sqlite.SqliteTransaction;
#else
using System.Data.SQLite;
#endif

namespace OpenCVR.Persistence
{
    internal class PersistenceTransaction : IPersistenceTransaction
    {
        private readonly SQLiteTransaction transaction;

        public PersistenceTransaction(SQLiteTransaction transaction)
        {
            this.transaction = transaction;
        }

        public void Dispose()
        {
            transaction.Dispose();
        }

        public void Commit()
        {
            transaction.Commit();
        }
    }
}
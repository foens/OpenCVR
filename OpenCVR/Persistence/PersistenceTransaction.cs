using System.Data.Common;

namespace OpenCVR.Persistence
{
    internal class PersistenceTransaction : IPersistenceTransaction
    {
        private readonly DbTransaction transaction;

        public PersistenceTransaction(DbTransaction transaction)
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
using NUnit.Framework;
using System.Data.Common;

namespace DbUnitTest
{
    public abstract class DatabaseTestsBase
    {
        protected DbProviderFactory DbProviderFactory { get; private set; }
        protected DbConnection DbConnection { get; private set; }

        [OneTimeSetUp]
        public void Init()
        {
            DbProviderFactory = GetDbProviderFactory();
            DbConnection = DbProviderFactory.CreateConnection();
            OpenConnection(DbConnection);
            using (DbCommand dbCommand = DbConnection.CreateCommand())
            {
                CreateDatabaseTables(dbCommand);
                AddDataToDatabaseTables(dbCommand);
            }
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DbConnection.Dispose();
        }

        protected abstract DbProviderFactory GetDbProviderFactory();

        protected abstract void OpenConnection(DbConnection dbConnection);

        protected abstract void CreateDatabaseTables(DbCommand dbCommand);

        protected abstract void AddDataToDatabaseTables(DbCommand dbCommand);
    }
}

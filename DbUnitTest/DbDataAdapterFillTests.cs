////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace DbUnitTest
{
    [TestFixture]
    [Category("Data Tests - DbDataAdapter.Fill")]
    public sealed class DbDataAdapterFillTests : DatabaseTestsBase
    {
        #region Test Cases

        private static class TestCases
        {
            public static IEnumerable<TestCaseData> TestHappyPath(int recordCount)
            {
                yield return new TestCaseData(0, 0).Returns(recordCount);
                yield return new TestCaseData(1, 0).Returns(recordCount - 1);
                yield return new TestCaseData(2, 0).Returns(recordCount - 2);
                yield return new TestCaseData(3, 0).Returns(recordCount - 3);
                yield return new TestCaseData(0, recordCount).Returns(recordCount);
                yield return new TestCaseData(0, recordCount + 1).Returns(recordCount);
                yield return new TestCaseData(0, recordCount - 1).Returns(recordCount - 1);
                yield return new TestCaseData(1, recordCount - 1).Returns(recordCount - 1);
                yield return new TestCaseData(2, recordCount - 1).Returns(recordCount - 2);
                yield return new TestCaseData(3, recordCount - 1).Returns(recordCount - 3);
            }

            public static IEnumerable<TestCaseData> TestStartRecordAndMaxRecordExceptions()
            {
                yield return new TestCaseData(-1, 0);
                yield return new TestCaseData(0, -1);
            }

            public static IEnumerable<TestCaseData> TestDataTablesExceptions()
            {
                yield return new TestCaseData(0, 0, null);
                yield return new TestCaseData(0, 0, Array.Empty<DataTable>());
            }
        }

        #endregion Test Cases

        #region Fields

        private const int _RECORDCOUNT = 10;

        #endregion Fields

        #region Tests

        // Verify that when the DbDataAdapter.Fill() method is invoked, the results are the expected value
        [TestCaseSource(typeof(TestCases), nameof(TestCases.TestHappyPath), new object[] { _RECORDCOUNT })]
        public int WhenTheStartRecordOrMaxRecordArgumentsAreGreaterOrEqualThanZero_ThenTheResultIsTheExpectedValue(int startRecord, int maxRecords)
        {
            return DbDataAdapterFill(startRecord, maxRecords, (startRec, maxRecs, dbDataAdapter, dataTable) => dbDataAdapter.Fill(startRec, maxRecs, dataTable));
        }

        // Verify that when the startRecord or maxRecords arguments are less than zero, then an ArgumentException is thrown
        [TestCaseSource(typeof(TestCases), nameof(TestCases.TestStartRecordAndMaxRecordExceptions))]
        public void WhenTheStartRecordOrMaxRecordArgumentsAreLessThanZero_ThenAnArgumentExceptionIsThrown(int startRecord, int maxRecords)
        {
            DbDataAdapterFill(startRecord, maxRecords, (startRec, maxRecs, dbDataAdapter, dataTable) =>
            {
                Assert.Throws<ArgumentException>(() => dbDataAdapter.Fill(startRec, maxRecs, dataTable));
                return int.MinValue;    // The delegate argument in DbDataAdapterFill() is a Func therefore
                                        // a return value is needed here.
            });
        }

        // Verify that when the dataTables argument is null or an empty array, then an ArgumentNullException is thrown
        [TestCaseSource(typeof(TestCases), nameof(TestCases.TestDataTablesExceptions))]
        public void WhenTheDataTablesArgumentIsNullOrAnEmptyArray_ThenAnArgumentNullExceptionIsThrown2(int startRecord, int maxRecords, DataTable[] dataTables)
        {
            Assert.Throws<ArgumentNullException>(() => DbDataAdapterFill(startRecord, maxRecords, dataTables,
                    (startRec, maxRecs, dbDataAdapter, dtTables) => dbDataAdapter.Fill(startRec, maxRecs, dtTables)));
        }

        private int DbDataAdapterFill(int startRecord, int maxRecords, Func<int, int, DbDataAdapter, DataTable, int> dbDataAdapterFill)
        {
            int result;
            const string selectText = "SELECT CategoryName FROM Categories";
            using (DbCommand dbCommand = DbConnection.CreateCommand())
                using (DbDataAdapter dbDataAdapter = DbProviderFactory.CreateDataAdapter())
                    using (DataTable dataTable = new DataTable())
                    {
                        dbCommand.CommandText       = selectText;
                        dbDataAdapter.SelectCommand = dbCommand;
                        result                      = dbDataAdapterFill(startRecord, maxRecords, dbDataAdapter, dataTable);
                    }
            return result;
        }

        private void DbDataAdapterFill(int startRecord, int maxRecords, DataTable[] dataTables, Action<int, int, DbDataAdapter, DataTable[]> dbDataAdapterFill)
        {
            const string selectText = "SELECT CategoryName FROM Categories";
            using (DbCommand dbCommand = DbConnection.CreateCommand())
                using (DbDataAdapter dbDataAdapter = DbProviderFactory.CreateDataAdapter())
                {
                    dbCommand.CommandText       = selectText;
                    dbDataAdapter.SelectCommand = dbCommand;
                    dbDataAdapterFill(startRecord, maxRecords, dbDataAdapter, dataTables);
                }
        }

        #endregion Tests

        #region Abstract Class Method Implementations for DatabaseTestsBase class

        protected override DbProviderFactory GetDbProviderFactory() => SQLiteFactory.Instance;

        protected override void OpenConnection(DbConnection dbConnection)
        {
            dbConnection.ConnectionString = "DataSource=:memory:";
            dbConnection.Open();
        }

        protected override void CreateDatabaseTables(DbCommand dbCommand)
        {
            const string createTableCategoriesSqlText =
                @"CREATE TABLE Categories
                    (
                        CategoryId   INTEGER NOT NULL PRIMARY KEY,
                        CategoryName TEXT NOT NULL
                    )";
            dbCommand.CommandText = createTableCategoriesSqlText;
            dbCommand.ExecuteNonQuery();
        }

        protected override void AddDataToDatabaseTables(DbCommand dbCommand)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i <= _RECORDCOUNT; i++)
                stringBuilder.Append($"INSERT INTO Categories (CategoryName) VALUES ('Category {i}');");
            dbCommand.CommandText = stringBuilder.ToString();
            dbCommand.ExecuteNonQuery();
        }

        #endregion Abstract Class Method Implementations for DatabaseTestsBase class
    }
}

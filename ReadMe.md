# DbUnitTest #

This project demonstrates a simple framework for writing unit tests for database code.  I am using the term "framework" in a very loose way here as the framework in question consists of a single abstract class (`DatabaseTestsBase`) that test classes should inherit from if the intent is to create unit tests for code that requires a database.

The abstract methods in `DatabaseTestsBase` are self-explanatory with their intent easily infered from their names:  `OpenConnection()`, `CreateDatabaseTables()`, and `AddDataToDatabaseTables()`.

`DatabaseTestsBase` is [DBMS](https://en.wikipedia.org/wiki/Database#Database_management_system) agnostic, it doesn't care what the underlying DBMS is.  This is accomplished via the `GetDbProviderFactory()` method which descendant classes must also override.  In practical terms however the fact that good unit tests must be fast, repeatable, and isolated, implies that the DBMS should be an [in-memory database](https://en.wikipedia.org/wiki/In-memory_database).  This allows for the database to be quickly created when needed and to be in a consistent state.  The unit tests in this project rely on a [SQLite](https://sqlite.org/index.html) database which has the option to run entirely in memory.

The origin of this project is my finding that the documentation for the [Fill(Int32, Int32, DataTable[])](https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbdataadapter.fill?view=netframework-4.7.2#System_Data_Common_DbDataAdapter_Fill_System_Int32_System_Int32_System_Data_DataTable___) overload of the [DbDataAdapter](https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbdataadapter.fill?view=netframework-4.7.2#System_Data_Common_DbDataAdapter_Fill_System_Int32_System_Int32_System_Data_DataTable___) class was missing some details.  I wanted to have a way to easily document the missing bits and writing unit tests is what made the most sense.  I reported the issue to Microsoft:  [DbDataAdapter.Fill Method Documentation appears to be incomplete](https://developercommunity.visualstudio.com/content/problem/1292017/dbdataadapterfill-method-documentation-appears-to.html).


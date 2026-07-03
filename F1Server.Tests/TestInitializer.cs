using System;

using F1Server.Db.Entity;
using F1Server.Tests.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Initialization of test assembly
/// </summary>
[TestClass]
public static class TestInitializer
{
    #region Methods

    /// <summary>
    /// Test assembly initializer
    /// </summary>
    /// <param name="context">Test context</param>
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        Environment.SetEnvironmentVariable("F1SERVER_DATABASE_TYPE", "99");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            dbFactory.InitDatabase();
        }

        var isPrepared = TestData.PrepareDatabase();

        Assert.IsTrue(isPrepared, "Database preparation failed!");
    }

    #endregion // Methods
}
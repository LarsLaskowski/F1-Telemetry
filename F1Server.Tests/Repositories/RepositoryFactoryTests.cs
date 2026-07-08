using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Tests.Repositories;

/// <summary>
/// Class to test the repository factory
/// </summary>
[TestClass]
public class RepositoryFactoryTests
{
    #region Methods

    /// <summary>
    /// Verifies that a failed operation sets the last error and that the error does not leak
    /// into the next factory instance leased from the context pool
    /// </summary>
    [TestMethod]
    public void RepositoryFactoryLastErrorDoesNotLeakBetweenLeases()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var entity = new NationalityEntity
                         {
                             NationalityGameId = 9101,
                             Name = "LastErrorLeaseTest"
                         };

            var isAdded = repository.Add(entity);

            Assert.IsTrue(isAdded, "Adding the test entity should succeed!");

            var duplicate = new NationalityEntity
                            {
                                Id = entity.Id,
                                NationalityGameId = 9102,
                                Name = "LastErrorLeaseTestDuplicate"
                            };

            var isAddedTwice = repository.Add(duplicate);

            Assert.IsFalse(isAddedTwice, "Adding an entity with an already tracked key should fail!");
            Assert.IsFalse(string.IsNullOrEmpty(dbFactory.LastError), "A failed add should set the last error!");
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            Assert.AreEqual(string.Empty, dbFactory.LastError, "A freshly leased context must not carry over the last error!");
        }
    }

    /// <summary>
    /// Verifies that two factories existing at the same time use independent contexts and
    /// do not share their last error state
    /// </summary>
    [TestMethod]
    public void RepositoryFactoryParallelFactoriesUseIndependentContexts()
    {
        using (var dbFactoryWithError = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactoryWithError.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var entity = new NationalityEntity
                         {
                             NationalityGameId = 9103,
                             Name = "ParallelFactoriesTest"
                         };

            var isAdded = repository.Add(entity);

            Assert.IsTrue(isAdded, "Adding the test entity should succeed!");

            var duplicate = new NationalityEntity
                            {
                                Id = entity.Id,
                                NationalityGameId = 9104,
                                Name = "ParallelFactoriesTestDuplicate"
                            };

            var isAddedTwice = repository.Add(duplicate);

            Assert.IsFalse(isAddedTwice, "Adding an entity with an already tracked key should fail!");

            using (var dbFactoryClean = RepositoryFactory.CreateInstance())
            {
                Assert.AreEqual(string.Empty, dbFactoryClean.LastError, "A factory created in parallel must not see the error of another factory!");
            }

            Assert.IsFalse(string.IsNullOrEmpty(dbFactoryWithError.LastError), "The failing factory should keep its own last error!");
        }
    }

    #endregion // Methods
}
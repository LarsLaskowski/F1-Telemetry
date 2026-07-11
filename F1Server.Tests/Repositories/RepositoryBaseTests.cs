using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Tests.Repositories;

/// <summary>
/// Class to test the repository base class
/// </summary>
[TestClass]
public class RepositoryBaseTests
{
    #region Methods

    /// <summary>
    /// Verifies that UpdateRange returns true after a successful save
    /// </summary>
    [TestMethod]
    public void RepositoryBaseUpdateRangeReturnsTrue()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var entities = new[]
                           {
                               new NationalityEntity
                               {
                                   NationalityGameId = 9001,
                                   Name = "UpdateRangeTestA"
                               },
                               new NationalityEntity
                               {
                                   NationalityGameId = 9002,
                                   Name = "UpdateRangeTestB"
                               }
                           };

            var isAdded = repository.AddRange(entities);

            Assert.IsTrue(isAdded, "Adding the test entities should succeed!");

            foreach (var entity in entities)
            {
                entity.Name += "-Updated";
            }

            var isUpdated = repository.UpdateRange(entities);

            Assert.IsTrue(isUpdated, "UpdateRange should return true after a successful SaveChanges!");

            var query = repository.GetQuery();

            Assert.IsNotNull(query, "Query should be resolvable!");

            var updatedNames = query.Where(n => n.NationalityGameId == 9001 || n.NationalityGameId == 9002)
                                    .Select(n => n.Name)
                                    .ToList();

            Assert.Contains("UpdateRangeTestA-Updated", updatedNames, "Updated name for entity A should be persisted!");
            Assert.Contains("UpdateRangeTestB-Updated", updatedNames, "Updated name for entity B should be persisted!");
        }
    }

    /// <summary>
    /// Verifies that UpdateRangeAsync returns true after a successful save
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task RepositoryBaseUpdateRangeAsyncReturnsTrue()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var entities = new[]
                           {
                               new NationalityEntity
                               {
                                   NationalityGameId = 9003,
                                   Name = "UpdateRangeAsyncTestA"
                               },
                               new NationalityEntity
                               {
                                   NationalityGameId = 9004,
                                   Name = "UpdateRangeAsyncTestB"
                               }
                           };

            var isAdded = repository.AddRange(entities);

            Assert.IsTrue(isAdded, "Adding the test entities should succeed!");

            foreach (var entity in entities)
            {
                entity.Name += "-Updated";
            }

            var isUpdated = await repository.UpdateRangeAsync(entities).ConfigureAwait(false);

            Assert.IsTrue(isUpdated, "UpdateRangeAsync should return true after a successful SaveChangesAsync!");

            var query = repository.GetQuery();

            Assert.IsNotNull(query, "Query should be resolvable!");

            var updatedNames = query.Where(n => n.NationalityGameId == 9003 || n.NationalityGameId == 9004)
                                    .Select(n => n.Name)
                                    .ToList();

            Assert.Contains("UpdateRangeAsyncTestA-Updated", updatedNames, "Updated name for entity A should be persisted!");
            Assert.Contains("UpdateRangeAsyncTestB-Updated", updatedNames, "Updated name for entity B should be persisted!");
        }
    }

    /// <summary>
    /// Verifies that InsertBatchAsync persists all entities of the batch
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task RepositoryBaseInsertBatchAsyncPersistsAllEntities()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var entities = new[]
                           {
                               new NationalityEntity
                               {
                                   NationalityGameId = 9005,
                                   Name = "InsertBatchAsyncTestA"
                               },
                               new NationalityEntity
                               {
                                   NationalityGameId = 9006,
                                   Name = "InsertBatchAsyncTestB"
                               }
                           };

            var isInserted = await repository.InsertBatchAsync(entities).ConfigureAwait(false);

            Assert.IsTrue(isInserted, "InsertBatchAsync should return true after a successful SaveChangesAsync!");

            var query = repository.GetQuery();

            Assert.IsNotNull(query, "Query should be resolvable!");

            var insertedNames = query.Where(n => n.NationalityGameId == 9005 || n.NationalityGameId == 9006)
                                     .Select(n => n.Name)
                                     .ToList();

            Assert.Contains("InsertBatchAsyncTestA", insertedNames, "Inserted name for entity A should be persisted!");
            Assert.Contains("InsertBatchAsyncTestB", insertedNames, "Inserted name for entity B should be persisted!");
        }
    }

    /// <summary>
    /// Verifies that a query ignoring auto-included navigations still returns the stored entities
    /// </summary>
    [TestMethod]
    public void RepositoryBaseGetQueryIgnoreAutoIncludesReturnsEntities()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var lapRepository = dbFactory.GetRepository<LapRepository>();

            Assert.IsNotNull(lapRepository, "Repository should be resolvable!");

            var lapEntity = new LapEntity
                            {
                                LapNumber = 90,
                                ParticipantId = 910001,
                                SessionId = 910001
                            };

            Assert.IsTrue(lapRepository.Add(lapEntity), "Adding the test lap should succeed!");

            var storedLap = lapRepository.GetQuery(ignoreAutoIncludes: true)
                                         ?.FirstOrDefault(l => l.ParticipantId == 910001 && l.LapNumber == 90);

            Assert.IsNotNull(storedLap, "The lap should be found by a query ignoring auto-included navigations!");
            Assert.AreEqual(lapEntity.Id, storedLap.Id, "The queried lap should be the previously added lap!");
        }
    }

    #endregion // Methods
}
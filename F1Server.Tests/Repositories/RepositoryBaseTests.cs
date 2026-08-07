using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;

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
    /// Verifies that RemoveWhere removes all matching entities and returns the removed count
    /// </summary>
    [TestMethod]
    public void RepositoryBaseRemoveWhereRemovesMatchingEntities()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var entities = new[]
                           {
                               new NationalityEntity
                               {
                                   NationalityGameId = 9007,
                                   Name = "RemoveWhereTestA"
                               },
                               new NationalityEntity
                               {
                                   NationalityGameId = 9008,
                                   Name = "RemoveWhereTestB"
                               }
                           };

            var isAdded = repository.AddRange(entities);

            Assert.IsTrue(isAdded, "Adding the test entities should succeed!");

            var removedCount = repository.RemoveWhere(n => n.NationalityGameId == 9007);

            Assert.AreEqual(1, removedCount, "RemoveWhere should remove exactly the matching entity!");

            var query = repository.GetQuery();

            Assert.IsNotNull(query, "Query should be resolvable!");

            var remainingNames = query.Where(n => n.NationalityGameId == 9007 || n.NationalityGameId == 9008)
                                      .Select(n => n.Name)
                                      .ToList();

            Assert.DoesNotContain("RemoveWhereTestA", remainingNames, "The matching entity should be removed!");
            Assert.Contains("RemoveWhereTestB", remainingNames, "The non-matching entity should still be present!");
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

    /// <summary>
    /// Verifies that AddAsync persists the entity
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task RepositoryBaseAddAsyncPersistsEntity()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var isAdded = await repository.AddAsync(new NationalityEntity
                                                    {
                                                        NationalityGameId = 9009,
                                                        Name = "AddAsyncTest"
                                                    })
                                          .ConfigureAwait(false);

            Assert.IsTrue(isAdded, "AddAsync should return true after a successful SaveChangesAsync!");

            var query = repository.GetQuery();

            Assert.IsNotNull(query, "Query should be resolvable!");

            var storedEntity = await query.FirstOrDefaultAsync(n => n.NationalityGameId == 9009)
                                          .ConfigureAwait(false);

            Assert.IsNotNull(storedEntity, "The added entity should be found!");
            Assert.AreEqual("AddAsyncTest", storedEntity.Name, "The name of the added entity should be persisted!");
        }
    }

    /// <summary>
    /// Verifies that RefreshAsync applies the refresh action to the matching entity
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task RepositoryBaseRefreshAsyncUpdatesEntity()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var isAdded = repository.Add(new NationalityEntity
                                         {
                                             NationalityGameId = 9010,
                                             Name = "RefreshAsyncTest"
                                         });

            Assert.IsTrue(isAdded, "Adding the test entity should succeed!");

            var isRefreshed = await repository.RefreshAsync(n => n.NationalityGameId == 9010,
                                                            entity => entity.Name = "RefreshAsyncTest-Updated")
                                              .ConfigureAwait(false);

            Assert.IsTrue(isRefreshed, "RefreshAsync should return true after a successful SaveChangesAsync!");

            var query = repository.GetQuery();

            Assert.IsNotNull(query, "Query should be resolvable!");

            var refreshedEntity = await query.FirstOrDefaultAsync(n => n.NationalityGameId == 9010)
                                             .ConfigureAwait(false);

            Assert.IsNotNull(refreshedEntity, "The refreshed entity should be found!");
            Assert.AreEqual("RefreshAsyncTest-Updated", refreshedEntity.Name, "The refreshed name should be persisted!");
        }
    }

    /// <summary>
    /// Verifies that RemoveAsync removes only the matching entity
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task RepositoryBaseRemoveAsyncRemovesMatchingEntity()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var entities = new[]
                           {
                               new NationalityEntity
                               {
                                   NationalityGameId = 9011,
                                   Name = "RemoveAsyncTestA"
                               },
                               new NationalityEntity
                               {
                                   NationalityGameId = 9012,
                                   Name = "RemoveAsyncTestB"
                               }
                           };

            var isAdded = repository.AddRange(entities);

            Assert.IsTrue(isAdded, "Adding the test entities should succeed!");

            var isRemoved = await repository.RemoveAsync(n => n.NationalityGameId == 9011)
                                            .ConfigureAwait(false);

            Assert.IsTrue(isRemoved, "RemoveAsync should return true after a successful SaveChangesAsync!");

            var query = repository.GetQuery();

            Assert.IsNotNull(query, "Query should be resolvable!");

            var remainingNames = await query.Where(n => n.NationalityGameId == 9011 || n.NationalityGameId == 9012)
                                            .Select(n => n.Name)
                                            .ToListAsync()
                                            .ConfigureAwait(false);

            Assert.DoesNotContain("RemoveAsyncTestA", remainingNames, "The matching entity should be removed!");
            Assert.Contains("RemoveAsyncTestB", remainingNames, "The non-matching entity should still be present!");
        }
    }

    /// <summary>
    /// Verifies that the asynchronous query operators can be used on the queryable returned by GetQuery, so the
    /// controllers can read data without blocking a thread on database I/O
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task RepositoryBaseGetQuerySupportsAsyncQueryOperators()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var repository = dbFactory.GetRepository<NationalityRepository>();

            Assert.IsNotNull(repository, "Repository should be resolvable!");

            var isAdded = repository.Add(new NationalityEntity
                                         {
                                             NationalityGameId = 9013,
                                             Name = "AsyncQueryOperatorsTest"
                                         });

            Assert.IsTrue(isAdded, "Adding the test entity should succeed!");

            var query = repository.GetQuery();

            Assert.IsNotNull(query, "Query should be resolvable!");

            var hasEntity = await query.AnyAsync(n => n.NationalityGameId == 9013)
                                       .ConfigureAwait(false);

            Assert.IsTrue(hasEntity, "AnyAsync should find the added entity!");

            var entityCount = await query.CountAsync(n => n.NationalityGameId == 9013)
                                         .ConfigureAwait(false);

            Assert.AreEqual(1, entityCount, "CountAsync should count exactly the added entity!");

            var names = await query.Where(n => n.NationalityGameId == 9013)
                                   .Select(n => n.Name)
                                   .ToListAsync()
                                   .ConfigureAwait(false);

            Assert.Contains("AsyncQueryOperatorsTest", names, "ToListAsync should return the added entity!");
        }
    }

    #endregion // Methods
}
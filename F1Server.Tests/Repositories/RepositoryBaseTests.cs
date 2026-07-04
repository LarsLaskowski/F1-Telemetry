using System.Linq;
using System.Threading.Tasks;

using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            Assert.IsTrue(updatedNames.Contains("UpdateRangeTestA-Updated"), "Updated name for entity A should be persisted!");
            Assert.IsTrue(updatedNames.Contains("UpdateRangeTestB-Updated"), "Updated name for entity B should be persisted!");
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

            Assert.IsTrue(updatedNames.Contains("UpdateRangeAsyncTestA-Updated"), "Updated name for entity A should be persisted!");
            Assert.IsTrue(updatedNames.Contains("UpdateRangeAsyncTestB-Updated"), "Updated name for entity B should be persisted!");
        }
    }

    #endregion // Methods
}
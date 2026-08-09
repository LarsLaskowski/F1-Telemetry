using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Runtime;

namespace F1Server.Tests.Runtime;

/// <summary>
/// Class to test the session runtime data class
/// </summary>
[TestClass]
public class SessionRuntimeDataTests
{
    #region Methods

    /// <summary>
    /// Verifies that FinishSession marks the session as finished, both on the runtime object and in the database
    /// </summary>
    [TestMethod]
    public void SessionRuntimeDataFinishSessionMarksSessionAsFinished()
    {
        SessionEntity sessionEntity;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            sessionEntity = new SessionEntity
                            {
                                SessionId = 918273UL
                            };

            Assert.IsTrue(dbFactory.GetRepository<SessionRepository>()?.Add(sessionEntity), "Session entity could not be added to the database!");
        }

        var sessionRuntimeData = new SessionRuntimeData(2025, 918273UL, SessionType.Race)
                                 {
                                     CurrentSession = new LiveSessionData
                                                      {
                                                          DbId = sessionEntity.Id
                                                      }
                                 };

        sessionRuntimeData.FinishSession();

        Assert.IsTrue(sessionRuntimeData.CurrentSession.IsFinished, "CurrentSession.IsFinished should be set to true!");
        Assert.IsEmpty(sessionRuntimeData.LastError, "LastError should remain empty on success!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSession = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.Id == sessionEntity.Id);

            Assert.IsNotNull(dbSession, "Session should still exist in the database!");
            Assert.IsTrue(dbSession.IsFinished, "Session should be marked finished in the database!");
        }
    }

    /// <summary>
    /// Verifies that FinishSession is a no-op when there is no current session with a database id
    /// </summary>
    [TestMethod]
    public void SessionRuntimeDataFinishSessionWithoutCurrentSessionDoesNothing()
    {
        var sessionRuntimeData = new SessionRuntimeData(2025, 918274UL, SessionType.Race);

        sessionRuntimeData.FinishSession();

        Assert.IsEmpty(sessionRuntimeData.LastError, "LastError should remain empty when there is no current session!");
    }

    #endregion // Methods
}
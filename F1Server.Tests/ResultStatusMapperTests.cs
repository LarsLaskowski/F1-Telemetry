using F1Server.Core.Enumerations;
using F1Server.Core.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Tests that out-of-range, untrusted UDP result status values fall back to <see cref="ResultStatus.Unknown"/>
/// instead of producing an undefined enum value
/// </summary>
[TestClass]
public class ResultStatusMapperTests
{
    #region Methods

    /// <summary>
    /// Check that F1 2019 mapping falls back to Unknown for a game value outside the documented range
    /// </summary>
    [TestMethod]
    public void ResultStatusMapperMapResultStatus2019WithOutOfRangeValueReturnsUnknown()
    {
        var resultStatus = ResultStatusMapper.MapResultStatus2019(255);

        Assert.AreEqual(ResultStatus.Unknown, resultStatus, "Out-of-range value should map to Unknown!");
    }

    /// <summary>
    /// Check that F1 2019 mapping still maps every documented in-range value correctly
    /// </summary>
    [TestMethod]
    public void ResultStatusMapperMapResultStatus2019WithInRangeValueReturnsMappedValue()
    {
        var resultStatus = ResultStatusMapper.MapResultStatus2019(3);

        Assert.AreEqual(ResultStatus.Finished, resultStatus, "In-range value should be mapped as before!");
    }

    /// <summary>
    /// Check that F1 2021 mapping falls back to Unknown for a game value outside the documented range
    /// </summary>
    [TestMethod]
    public void ResultStatusMapperMapResultStatus2021WithOutOfRangeValueReturnsUnknown()
    {
        var resultStatus = ResultStatusMapper.MapResultStatus2021(200);

        Assert.AreEqual(ResultStatus.Unknown, resultStatus, "Out-of-range value should map to Unknown!");
    }

    /// <summary>
    /// Check that F1 2021 mapping still maps the special-cased "did not finish" game value correctly
    /// </summary>
    [TestMethod]
    public void ResultStatusMapperMapResultStatus2021WithDidNotFinishValueReturnsMappedValue()
    {
        var resultStatus = ResultStatusMapper.MapResultStatus2021(4);

        Assert.AreEqual(ResultStatus.DidNotFinish, resultStatus, "Value 4 should map to DidNotFinish!");
    }

    /// <summary>
    /// Check that F1 2021 mapping still maps every documented in-range value correctly
    /// </summary>
    [TestMethod]
    public void ResultStatusMapperMapResultStatus2021WithInRangeValueReturnsMappedValue()
    {
        var resultStatus = ResultStatusMapper.MapResultStatus2021(7);

        Assert.AreEqual(ResultStatus.Retired, resultStatus, "In-range value should be mapped as before!");
    }

    #endregion // Methods
}
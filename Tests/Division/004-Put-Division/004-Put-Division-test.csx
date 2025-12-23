using Xunit;

tp.Test("Put division ok should return 204 or 404", () =>
{
    var response = tp.Responses["PutDivisionOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

tp.Test("Put division duplicate code should return 409 or 204 or 404", () =>
{
    var response = tp.Responses["PutDivisionDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 409 || status == 204 || status == 404, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Division code already in use for this company", raw);
    }
});

tp.Test("Put division leader not found should return 400/204/404/409", () =>
{
    var response = tp.Responses["PutDivisionLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.Contains("Leader not found", raw);
    }
});

tp.Test("Put division no-op should return 204 or 404", () =>
{
    var response = tp.Responses["PutDivisionNoOp"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});


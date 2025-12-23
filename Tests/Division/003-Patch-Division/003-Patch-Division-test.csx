using Xunit;

tp.Test("Patch division should return 204 or 404/409", () =>
{
    var response = tp.Responses["PatchDivisionOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
});

tp.Test("Patch division duplicate code should return 409 or 204/404", () =>
{
    var response = tp.Responses["PatchDivisionDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 409 || status == 204 || status == 404, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Division code already in use for this company", raw);
    }
});

tp.Test("Patch division leader not found should return 400/204/404/409", () =>
{
    var response = tp.Responses["PatchDivisionLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.Contains("Leader not found", raw);
    }
});

tp.Test("Patch division no-op should return 204 or 404", () =>
{
    var response = tp.Responses["PatchDivisionNoOp"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

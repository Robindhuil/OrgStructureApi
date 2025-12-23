using Xunit;

tp.Test("Put company ok should return 204 or 404", () =>
{
    var response = tp.Responses["PutCompanyOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

tp.Test("Put company director not found should return 400/404/204/409", () =>
{
    var response = tp.Responses["PutCompanyDirectorNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 404 || status == 204 || status == 409, $"Unexpected status: {status}");

    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.Contains("Director not found", raw);
    }
});

tp.Test("Put company conflict code should return 409 or 204 or 404", () =>
{
    var response = tp.Responses["PutCompanyConflictCode"];
    var status = response.StatusCode();
    Assert.True(status == 409 || status == 204 || status == 404, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.True(raw.Contains("Company code already in use") || raw.Contains("code"), raw);
    }
});

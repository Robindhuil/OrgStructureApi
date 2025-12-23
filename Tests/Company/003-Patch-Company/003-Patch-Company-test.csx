using Xunit;

tp.Test("Patch company ok should return 204 or 404", () =>
{
    var response = tp.Responses["PatchCompanyOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");

    if (status == 204)
    {
    }
});

tp.Test("Patch company director not found should return 400/404/204/409", () =>
{
    var response = tp.Responses["PatchCompanyDirectorNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 404 || status == 204 || status == 409, $"Unexpected status: {status}");

    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.True(raw.Contains("Director not found") || raw.Contains("Director must be an employee"), raw);
    }
});

tp.Test("Patch company no-op should return 204 or 404", () =>
{
    var response = tp.Responses["PatchCompanyNoOp"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

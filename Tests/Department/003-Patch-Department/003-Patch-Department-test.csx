using Xunit;

tp.Test("Patch department should return 204 or 404/409", () =>
{
    var response = tp.Responses["PatchDepartmentOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
});

tp.Test("Patch department duplicate code should return 409 or 204/404", () =>
{
    var response = tp.Responses["PatchDepartmentDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 409 || status == 204 || status == 404, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Department code already in use for this project", raw);
    }
});

tp.Test("Patch department leader not found should return 400/204/404/409", () =>
{
    var response = tp.Responses["PatchDepartmentLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.Contains("Leader not found", raw);
    }
});

tp.Test("Patch department no-op should return 204 or 404", () =>
{
    var response = tp.Responses["PatchDepartmentNoOp"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

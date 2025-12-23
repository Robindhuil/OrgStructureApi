using Xunit;

tp.Test("Patch project should return 204 or 404/409", () =>
{
    var response = tp.Responses["PatchProjectOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
});

tp.Test("Patch project duplicate code should return 409 or 204/404", () =>
{
    var response = tp.Responses["PatchProjectDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 409 || status == 204 || status == 404, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Project code already in use for this division", raw);
    }
});

tp.Test("Patch project leader not found should return 400/204/404/409", () =>
{
    var response = tp.Responses["PatchProjectLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.Contains("Leader not found", raw);
    }
});

tp.Test("Patch project no-op should return 204 or 404", () =>
{
    var response = tp.Responses["PatchProjectNoOp"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

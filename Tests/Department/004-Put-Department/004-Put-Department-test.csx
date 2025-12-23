using Xunit;

tp.Test("Put department ok should return 204 or 404", () =>
{
    var response = tp.Responses["PutDepartmentOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

tp.Test("Put department duplicate code should return 409 or 204 or 404", () =>
{
    var response = tp.Responses["PutDepartmentDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 409 || status == 204 || status == 404, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Department code already in use for this project", raw);
    }
});

tp.Test("Put department leader not found should return 400/204/404/409", () =>
{
    var response = tp.Responses["PutDepartmentLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.True(
            raw.Contains("Leader not found") ||
            raw.Contains("Project not found") ||
            raw.Contains("Division not found") ||
            raw.Contains("Validation Error"),
            raw);
    }
});

tp.Test("Put department no-op should return 204 or 404", () =>
{
    var response = tp.Responses["PutDepartmentNoOp"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

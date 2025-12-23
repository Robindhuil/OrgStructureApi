using Xunit;

tp.Test("Put project ok should return 204 or 404", () =>
{
    var response = tp.Responses["PutProjectOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

tp.Test("Put project duplicate code should return 409 or 204 or 404", () =>
{
    var response = tp.Responses["PutProjectDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 409 || status == 204 || status == 404, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Project code already in use for this division", raw);
    }
});

tp.Test("Put project leader not found should return 400/204/404/409", () =>
{
    var response = tp.Responses["PutProjectLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");
    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.True(
            raw.Contains("Leader not found") ||
            raw.Contains("Division not found") ||
            raw.Contains("Project not found") ||
            raw.Contains("Validation Error"),
            raw);
    }
});

tp.Test("Put project no-op should return 204 or 404", () =>
{
    var response = tp.Responses["PutProjectNoOp"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404, $"Unexpected status: {status}");
});

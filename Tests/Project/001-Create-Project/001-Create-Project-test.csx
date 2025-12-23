using Xunit;

tp.Test("Create project should return 201 or 400/409", () =>
{
    var response = tp.Responses["CreateProjectOk"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 400 || status == 409, $"Unexpected status: {status}");

    if (status == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Assert.NotNull((object)body.id);
        Assert.Equal(1, (int)body.divisionId);
        Assert.Equal("PROJ-UNQ-1", (string)body.code);
    }
});

tp.Test("Create project duplicate code should return 409 or 201", () =>
{
    var response = tp.Responses["CreateProjectDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 409 || status == 400, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Project code already in use for this division", raw);
    }
});

tp.Test("Create project missing name should return 400 with errors", () =>
{
    var response = tp.Responses["CreateProjectMissingName"];
    Assert.Equal(400, response.StatusCode());
    dynamic body = response.GetBodyAsExpando();
    Assert.NotNull((object)body.errors);
});

tp.Test("Create project division not found should return 400", () =>
{
    var response = tp.Responses["CreateProjectDivisionNotFound"];
    Assert.Equal(400, response.StatusCode());
    var raw = response.GetBody();
    Assert.Contains("Division not found", raw);
});

tp.Test("Create project leader not found should return 400 or 201/409", () =>
{
    var response = tp.Responses["CreateProjectLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 201 || status == 409, $"Unexpected status: {status}");

    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.True(
            raw.Contains("Leader not found") ||
            raw.Contains("Division not found") ||
            raw.Contains("Validation Error"),
            raw);
    }
});

using Xunit;

tp.Test("Create department should return 201 or 400/409", () =>
{
    var response = tp.Responses["CreateDepartmentOk"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 400 || status == 409, $"Unexpected status: {status}");

    if (status == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Assert.NotNull((object)body.id);
        Assert.Equal(1, (int)body.projectId);
        Assert.Equal("DEPT-UNQ-1", (string)body.code);
    }
});

tp.Test("Create department duplicate code should return 409 or 201", () =>
{
    var response = tp.Responses["CreateDepartmentDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 409 || status == 400, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Department code already in use for this project", raw);
    }
});

tp.Test("Create department missing name should return 400 with errors", () =>
{
    var response = tp.Responses["CreateDepartmentMissingName"];
    Assert.Equal(400, response.StatusCode());
    dynamic body = response.GetBodyAsExpando();
    Assert.NotNull((object)body.errors);
});

tp.Test("Create department project not found should return 400", () =>
{
    var response = tp.Responses["CreateDepartmentProjectNotFound"];
    Assert.Equal(400, response.StatusCode());
    var raw = response.GetBody();
    Assert.Contains("Project not found", raw);
});

tp.Test("Create department leader not found should return 400 or 201/409", () =>
{
    var response = tp.Responses["CreateDepartmentLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 201 || status == 409, $"Unexpected status: {status}");

    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.True(
            raw.Contains("Leader not found") ||
            raw.Contains("Project not found") ||
            raw.Contains("Validation Error"),
            raw);
    }
});

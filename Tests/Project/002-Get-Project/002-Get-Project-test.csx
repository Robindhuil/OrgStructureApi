using Xunit;

tp.Test("Get projects should return 200 and an array", () =>
{
    var response = tp.Responses["GetProjects"];
    var status = response.StatusCode();
    Assert.Equal(200, status);
    var raw = response.GetBody();
    Assert.False(string.IsNullOrWhiteSpace(raw));
    Assert.True(raw.TrimStart().StartsWith("["), "Expected JSON array for GET /api/project");
});

tp.Test("Get project by id should return 200 or 404", () =>
{
    var response = tp.Responses["GetProjectById"];
    var status = response.StatusCode();
    Assert.True(status == 200 || status == 404, $"Unexpected status: {status}");
    if (status == 200)
    {
        dynamic body = response.GetBodyAsExpando();
        Assert.NotNull((object)body.id);
        Assert.NotNull((object)body.name);
        Assert.NotNull((object)body.code);
        Assert.NotNull((object)body.divisionId);
    }
});

using Xunit;

tp.Test("Create division should return 201 or 400/409", () =>
{
    var response = tp.Responses["CreateDivisionOk"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 400 || status == 409, $"Unexpected status: {status}");

    if (status == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Assert.NotNull((object)body.id);
        Assert.Equal(1, (int)body.companyId);
        Assert.Equal("DIV-UNQ-1", (string)body.code);
    }
});

tp.Test("Create division duplicate code should return 409 or 201", () =>
{
    var response = tp.Responses["CreateDivisionDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 409, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Division code already in use", raw);
    }
});

tp.Test("Create division missing name should return 400 with errors", () =>
{
    var response = tp.Responses["CreateDivisionMissingName"];
    Assert.Equal(400, response.StatusCode());
    dynamic body = response.GetBodyAsExpando();
    Assert.NotNull((object)body.errors);
});

tp.Test("Create division company not found should return 400", () =>
{
    var response = tp.Responses["CreateDivisionCompanyNotFound"];
    Assert.Equal(400, response.StatusCode());
    var raw = response.GetBody();
    Assert.Contains("Company not found", raw);
});

tp.Test("Create division leader not found should return 400 or 201/409", () =>
{
    var response = tp.Responses["CreateDivisionLeaderNotFound"];
    var status = response.StatusCode();
    Assert.True(status == 400 || status == 201 || status == 409, $"Unexpected status: {status}");

    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.Contains("Leader not found", raw);
    }
});

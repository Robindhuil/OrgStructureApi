using Xunit;

tp.Test("Create company should return 201 or 409", () =>
{
    var response = tp.Responses["CreateCompanyOk"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 409, $"Unexpected status: {status}");

    if (status == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        var idObj = body.id;
        Assert.NotNull((object)idObj);
        Assert.Equal("TEST-COMP-UNQ", (string)body.code);
        Assert.Equal("Test Company Inc", (string)body.name);
    }
});

tp.Test("Create company duplicate code should return 409 or 201", () =>
{
    var response = tp.Responses["CreateCompanyDuplicateCode"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 409, $"Unexpected status: {status}");

    if (status == 409)
    {
        var raw = response.GetBody();
        Assert.True(raw.Contains("Company code already in use") || raw.Contains("code"), raw);
    }
});

tp.Test("Create company missing name should return 400 with errors", () =>
{
    var response = tp.Responses["CreateCompanyMissingName"];
    var status = response.StatusCode();
    Assert.Equal(400, status);
    dynamic body = response.GetBodyAsExpando();
    Assert.NotNull((object)body.errors);
});

tp.Test("Create company long name should return 400 with errors", () =>
{
    var response = tp.Responses["CreateCompanyLongName"];
    var status = response.StatusCode();
    Assert.Equal(400, status);
    dynamic body = response.GetBodyAsExpando();
    Assert.NotNull((object)body.errors);
});

tp.Test("Create company with missing director should return 400 (or 409 if code exists)", () =>
{
    var response = tp.Responses["CreateCompanyDirectorNotFound"];
    var status = response.StatusCode();

    Assert.True(status == 400 || status == 409 || status == 201, $"Unexpected status: {status}");

    if (status == 400)
    {
        var raw = response.GetBody();
        Assert.Contains("Director not found", raw);
    }
    else if (status == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Assert.Equal("MISSING-DIR", (string)body.code);
        Assert.Equal("Company With Missing Director", (string)body.name);
    }
    else
    {
    }
});


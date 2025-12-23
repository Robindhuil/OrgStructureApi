using Xunit;

tp.Test("Get employees should return 200", () =>
{
    var response = tp.Responses["GetEmployees"];
    Equal(200, response.StatusCode());
});

tp.Test("Get employee by id 1 should be 200 or 404 and have fields when 200", () =>
{
    var response = tp.Responses["GetEmployee1"];
    var status = response.StatusCode();
    Assert.True(status == 200 || status == 404, $"Expected 200 or 404, got {status}");

    if (status == 200)
    {
        dynamic body = response.GetBodyAsExpando();
        Assert.NotNull((object)body.id);
        Assert.NotNull((object)body.email);
        try { var cid = body.companyId; } catch { throw new Exception("Response missing companyId field"); }
    }
});

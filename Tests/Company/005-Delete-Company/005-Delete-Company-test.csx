using Xunit;

tp.Test("Delete company should return 204, 404 or 409", () =>
{
    var response = tp.Responses["DeleteCompanyOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 404 || status == 409, $"Unexpected status: {status}");

    if (status == 204)
    {
        var second = tp.Responses["DeleteCompanyAgain"];
        Assert.Equal(404, second.StatusCode());
    }
    else if (status == 409)
    {
        var raw = response.GetBody();
        Assert.Contains("Company cannot be deleted while employees are assigned", raw);
    }
});

tp.Test("Delete non-existent company should return 404", () =>
{
    var response = tp.Responses["DeleteCompanyNotFound"];
    Assert.Equal(404, response.StatusCode());
});

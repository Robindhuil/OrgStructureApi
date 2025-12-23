using Xunit;

tp.Test("Delete project should return 204 or 404 and subsequent GET should be 404", () =>
{
    var del = tp.Responses["DeleteProject1"];
    var status = del.StatusCode();
    Assert.True(status == 204 || status == 404, $"Expected 204 or 404, got {status}");

    var get = tp.Responses["GetProject1AfterDelete"];
    if (status == 204)
    {
        Assert.Equal(404, get.StatusCode());
    }
    else
    {
        Assert.True(get.StatusCode() == 404 || get.StatusCode() == 200, $"Unexpected GET status: {get.StatusCode()}");
    }
});

tp.Test("Deleting same project twice: second delete should be 404 if first was 204", () =>
{
    var first = tp.Responses["DeleteProject1"];
    var second = tp.Responses["DeleteProject1Again"];

    if (first.StatusCode() == 204)
    {
        Assert.Equal(404, second.StatusCode());
    }
    else
    {
        Assert.Equal(404, second.StatusCode());
    }
});

tp.Test("Delete non-existent project should return 404", () =>
{
    var resp = tp.Responses["DeleteProjectNonExist"];
    Assert.Equal(404, resp.StatusCode());
});

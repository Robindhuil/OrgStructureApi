using Xunit;

tp.Test("Delete department should return 204 or 404 and subsequent GET should be 404", () =>
{
    var del = tp.Responses["DeleteDepartment1"];
    var status = del.StatusCode();
    Assert.True(status == 204 || status == 404, $"Expected 204 or 404, got {status}");

    var get = tp.Responses["GetDepartment1AfterDelete"];
    if (status == 204)
    {
        Assert.Equal(404, get.StatusCode());
    }
    else
    {
        Assert.True(get.StatusCode() == 404 || get.StatusCode() == 200, $"Unexpected GET status: {get.StatusCode()}");
    }
});

tp.Test("Deleting same department twice: second delete should be 404 if first was 204", () =>
{
    var first = tp.Responses["DeleteDepartment1"];
    var second = tp.Responses["DeleteDepartment1Again"];

    if (first.StatusCode() == 204)
    {
        Assert.Equal(404, second.StatusCode());
    }
    else
    {
        Assert.Equal(404, second.StatusCode());
    }
});

tp.Test("Delete non-existent department should return 404", () =>
{
    var resp = tp.Responses["DeleteDepartmentNonExist"];
    Assert.Equal(404, resp.StatusCode());
});

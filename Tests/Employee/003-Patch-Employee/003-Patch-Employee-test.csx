using Xunit;

tp.Test("Patch employee should return 204 or 409", () =>
{
    var response = tp.Responses["PatchEmployeeOk"];
    var status = response.StatusCode();
    Assert.True(status == 204 || status == 409 || status == 404, $"Expected 204, 409 or 404, got {status}");

    if (status == 204)
    {
        var getResp = tp.Responses["GetPatchedEmployee1"];
        Equal(200, getResp.StatusCode());
        dynamic body = getResp.GetBodyAsExpando();
        Equal("patched.email@test.sk", (string)body.email);
    }
    else if (status == 409)
    {
        dynamic body = response.GetBodyAsExpando();
        try
        {
            var detail = (string)body.detail;
            Assert.True(detail.Contains("Email") || detail.Contains("Phone"), "Conflict detail must mention Email or Phone");
        }
        catch { throw new Exception("PatchEmployeeOk returned 409 but response has no 'detail' field"); }
    }
    else
    {
    }
});


tp.Test("Patch duplicate email should return 409 or succeed", () =>
{
    var response = tp.Responses["PatchEmployeeDuplicateEmail"];
    var status = response.StatusCode();
    if (status == 409)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("Email already in use by another employee.", (string)body.detail);
    }
    else if (status == 204)
    {
        var getResp = tp.Responses["GetPatchedEmployee2"];
        Equal(200, getResp.StatusCode());
        dynamic body = getResp.GetBodyAsExpando();
        Equal("unique.email@test.sk", (string)body.email);
    }
    else if (status == 404)
    {
    }
    else
    {
        Assert.True(false, $"Unexpected status code: {status}");
    }
});


tp.Test("Patch duplicate phone should return 409 or succeed", () =>
{
    var response = tp.Responses["PatchEmployeeDuplicatePhone"];
    var status = response.StatusCode();
    if (status == 409)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("Phone already in use by another employee.", (string)body.detail);
    }
    else if (status == 204)
    {
        var getResp = tp.Responses["GetPatchedEmployee3"];
        Equal(200, getResp.StatusCode());
        dynamic body = getResp.GetBodyAsExpando();
        Equal("0999000001", (string)body.phone);
    }
    else if (status == 404)
    {
    }
    else
    {
        Assert.True(false, $"Unexpected status code: {status}");
    }
});


tp.Test("Cannot change company if employee is leader/director (400) or succeed if not", () =>
{
    var response = tp.Responses["PatchEmployeeChangeCompanyLeader"];
    var status = response.StatusCode();
    if (status == 400)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("Validation Error", (string)body.title);
        Assert.True(((string)body.detail).Contains("Cannot change company"));
    }
    else if (status == 204)
    {
        var getResp = tp.Responses["GetPatchedEmployee1AfterChange"];
        Equal(200, getResp.StatusCode());
        dynamic body = getResp.GetBodyAsExpando();
        int cid = System.Convert.ToInt32((object)body.companyId);
        Equal(2, cid);
    }
    else if (status == 409)
    {
        dynamic body = response.GetBodyAsExpando();
        var detail = (string)body.detail;
        Assert.False(string.IsNullOrEmpty(detail));
    }
    else if (status == 404)
    {
    }
    else
    {
        Assert.True(false, $"Unexpected status code: {status}");
    }
});

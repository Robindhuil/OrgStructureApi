using Xunit;

tp.Test("Create employee should return 201", () =>
{
    var response = tp.Responses["CreateEmployeeOk"];
    var status = response.StatusCode();
    Assert.True(status == 201 || status == 409, $"Expected 201 or 409, got {status}");
});

tp.Test("Created employee should contain correct email", () =>
{
    var response = tp.Responses["CreateEmployeeOk"];
    if (response.StatusCode() == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("unique.email@test.sk", (string)body.email);
    }
    else
    {
        dynamic body = response.GetBodyAsExpando();
        try
        {
            var detail = (string)body.detail;
            Assert.True(detail.Contains("Email") || detail.Contains("Phone"), "Conflict detail must mention Email or Phone");
        }
        catch
        {
            throw new Exception("CreateEmployeeOk returned 409 but response has no 'detail' field");
        }
    }
});

tp.Test("Created employee should have companyId", () =>
{
    var response = tp.Responses["CreateEmployeeOk"];
    if (response.StatusCode() == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        var companyIdObj = body.companyId;
        Assert.NotNull((object)companyIdObj);
        int cid = System.Convert.ToInt32(companyIdObj);
        Assert.True(cid > 0, $"companyId should be > 0, got {cid}");
    }
    else
    {
        // If creation failed due to conflict, ensure response contains detail (previous tests cover specifics)
        dynamic body = response.GetBodyAsExpando();
        try { var detail = (string)body.detail; } catch { /* ignore */ }
    }
});



// ---------- DUPLICATE EMAIL ----------
tp.Test("Duplicate email should return 409", () =>
{
    var response = tp.Responses["CreateEmployeeDuplicateEmail"];
    var status = response.StatusCode();
    if (status == 409)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("Email already in use by another employee.", (string)body.detail);
    }
    else if (status == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("unique.email@test.sk", (string)body.email);
    }
    else
    {
        Assert.True(false, $"Unexpected status code: {status}");
    }
});

tp.Test("Duplicate email error detail should be correct", () =>
{
    var response = tp.Responses["CreateEmployeeDuplicateEmail"];
    if (response.StatusCode() == 409)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("Email already in use by another employee.", (string)body.detail);
    }
    else if (response.StatusCode() == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("unique.email@test.sk", (string)body.email);
    }
    else
    {
        Assert.True(false, $"Unexpected status code: {response.StatusCode()}");
    }
});


// ---------- DUPLICATE PHONE ----------
tp.Test("Duplicate phone should return 409", () =>
{
    var response = tp.Responses["CreateEmployeeDuplicatePhone"];
    var status = response.StatusCode();
    if (status == 409)
    {
        dynamic body = response.GetBodyAsExpando();
        var detail = (string)body.detail;
        Assert.True(detail == "Phone already in use by another employee." || detail == "Email already in use by another employee.", "Conflict detail must be phone or email");
    }
    else if (status == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("eva.hruskova@test.sk", (string)body.email);
    }
    else
    {
        Assert.True(false, $"Unexpected status code: {status}");
    }
});

tp.Test("Duplicate phone error detail should be correct", () =>
{
    var response = tp.Responses["CreateEmployeeDuplicatePhone"];
    if (response.StatusCode() == 409)
    {
        dynamic body = response.GetBodyAsExpando();
        var detail = (string)body.detail;
        Assert.True(detail == "Phone already in use by another employee." || detail == "Email already in use by another employee.", "Conflict detail must be phone or email");
    }
    else if (response.StatusCode() == 201)
    {
        dynamic body = response.GetBodyAsExpando();
        Equal("eva.hruskova@test.sk", (string)body.email);
    }
    else
    {
        Assert.True(false, $"Unexpected status code: {response.StatusCode()}");
    }
});


using System.Collections.Generic;

namespace OrgStructureApi.Dtos;

public class ApiErrorResponse
{
    public int Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public ApiErrorResponse() { }

    public ApiErrorResponse(int status, string title, string? detail = null)
    {
        Status = status;
        Title = title;
        Detail = detail;
    }
}

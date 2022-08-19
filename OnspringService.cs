using Onspring.API.SDK;
using Onspring.API.SDK.Models;
using Onspring.API.SDK.Enums;

public class OnspringService
{
    private readonly OnspringClient _client;

    public OnspringService(string apiKey)
    {
        var baseUrl = "https://api.onspring.com/";
        _client = new OnspringClient(baseUrl, apiKey);
    }

    public async Task<List<int>> GetFilesForApp(int appId)
    {
        var fileFields = await GetAppsFileFields(appId);
        return fileFields;
    }

    private async Task<List<int>> GetAppsFileFields(int appId)
    {
        Console.WriteLine($"Getting file fields for app {appId}...");

        var pagingRequest = new PagingRequest(1, 50);

        var response = await _client.GetFieldsForAppAsync(appId, pagingRequest);

        if (response.IsSuccessful is false) return new List<int>();

        var fields = response.Value.Items;

        if (response.Value.TotalPages > 1)
        {
            var currentPage = response.Value.PageNumber;
            var totalPages = response.Value.TotalPages;

            while (currentPage != totalPages)
            {
                pagingRequest.PageNumber++;
                response = await _client.GetFieldsForAppAsync(appId, pagingRequest);

                foreach (var field in response.Value.Items)
                {
                    fields.Add(field);
                }

                currentPage = response.Value.PageNumber;
                totalPages = response.Value.TotalPages;
            }
        }

        return fields
        .Where(field => field.Type == FieldType.Attachment || field.Type == FieldType.Image)
        .Select(field => field.Id)
        .ToList();
    }
}
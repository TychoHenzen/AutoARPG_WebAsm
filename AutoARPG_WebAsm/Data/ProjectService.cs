using System.Net.Http.Json;

public class ProjectService
{
    private readonly HttpClient _httpClient;

    public ProjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProjectInfo>> GetProjectsAsync()
    {
        try
        {
            var projects = await _httpClient.GetFromJsonAsync<List<ProjectInfo>>("projects/projects.json");
            return projects ?? new List<ProjectInfo>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading projects: {ex.Message}");
            return new List<ProjectInfo>();
        }
    }
}
public class ProjectInfo
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string FullDescription { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public string VideoUrl { get; set; } = "";
    public string SourceUrl { get; set; } = "";
    public List<string> Tags { get; set; } = new();
}
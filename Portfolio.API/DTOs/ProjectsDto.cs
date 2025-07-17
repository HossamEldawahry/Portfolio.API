namespace Portfolio.API.DTOs
{
    public class ProjectsDto
    {
        public int? Id { get; set; }
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public string? GitHubUrl { get; set; }
        public string? DemoUrl { get; set; }
    }
}

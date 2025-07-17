namespace Portfolio.API.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? DemoUrl { get; set; }

    }
}

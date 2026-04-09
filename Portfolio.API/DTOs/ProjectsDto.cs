namespace Portfolio.API.DTOs
{
    public class ProjectsDto
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(8000)]
        public string Description { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        [MaxLength(500)]
        public string? GitHubUrl { get; set; }

        [MaxLength(500)]
        public string? DemoUrl { get; set; }

        public bool IsFeatured { get; set; }
    }
}

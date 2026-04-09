namespace Portfolio.API.DTOs
{
    public class ProfileDto
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        public string Bio { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        [MaxLength(500)]
        public string? LinkedInUrl { get; set; }

        [MaxLength(500)]
        public string? GitHubUrl { get; set; }
    }
}

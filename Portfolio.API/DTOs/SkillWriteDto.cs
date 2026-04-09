namespace Portfolio.API.DTOs;

public class SkillWriteDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 5)]
    public int Level { get; set; }
}

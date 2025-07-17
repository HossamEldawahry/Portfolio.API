namespace Portfolio.API.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; } = 0; // Level 0 => Starter, 1 => Beginner, 2 => Intermediate, 3 => Advanced, 4 => Expert , 5 => Master 
    }
}

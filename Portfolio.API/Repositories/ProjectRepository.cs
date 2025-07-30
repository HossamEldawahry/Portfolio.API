using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Portfolio.API.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProjectRepository(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folderPath = Path.Combine(_env.WebRootPath, "Images", "projects");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream).ConfigureAwait(false);
            }

            return $"/Images/projects/{fileName}";
        }

        public async Task AddAsync(ProjectsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            string? imagePath = await SaveImageAsync(dto.Image).ConfigureAwait(false);

            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                GitHubUrl = dto.GitHubUrl,
                DemoUrl = dto.DemoUrl,
                ImageUrl = imagePath
            };

            await _context.Projects.AddAsync(project).ConfigureAwait(false);

        }
        public async Task<int> CountAsync() => await _context.Projects.CountAsync().ConfigureAwait(true);

        public void Delete(Project data) => 
            _context.Projects.Remove(data);

        public async Task UpdateAsync(int id,ProjectsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var project = await GetByIdAsync(id).ConfigureAwait(false);
            // حذف الصورة القديمة إذا تم رفع صورة جديدة
            if (dto.Image != null && dto.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(project!.ImageUrl))
                {
                    string oldImagePath = Path.Combine(_env.WebRootPath, project.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                // حفظ الصورة الجديدة
                project.ImageUrl = await SaveImageAsync(dto.Image).ConfigureAwait(false);
            }

            // تحديث باقي البيانات
            project!.Title = dto.Title;
            project.Description = dto.Description;
            project.GitHubUrl = dto.GitHubUrl;
            project.DemoUrl = dto.DemoUrl;

            _context.Projects.Update(project);

        }

        public async Task<IEnumerable<Project>> GetAllAsync() => await _context.Projects.ToListAsync().ConfigureAwait(true);
        public async Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize) => 
            await _context.Projects
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);
        public async Task<Project?> GetByIdAsync(int id) => 
            await _context.Projects
                .SingleOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(true);
        public async Task<Project?> GetFirstAsync() => 
            await _context.Projects
                .FirstOrDefaultAsync()
                .ConfigureAwait(true);
    }
}

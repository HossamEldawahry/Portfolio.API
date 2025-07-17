
namespace Portfolio.API.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProfileRepository(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task AddAsync(ProfileDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            string? imagePath = await SaveImageAsync(dto.Image).ConfigureAwait(false);

            var profile = new Profile
            {
                Name = dto.Name,
                Title = dto.Title,
                Email = dto.Email,
                Bio = dto.Bio,
                LinkedInUrl = dto.LinkedInUrl,
                GitHubUrl = dto.GitHubUrl,
                ImageUrl = imagePath
            };
            await _context.Profiles.AddAsync(profile).ConfigureAwait(false);
        }
        public async Task<Profile?> GetByIdAsync(int id) =>
            await _context.Profiles.FindAsync(id).ConfigureAwait(false);
        public async Task<Profile?> GetFirstAsync() => 
            await _context.Profiles.FirstOrDefaultAsync().ConfigureAwait(false);
        public async Task UpdateAsync(int id, ProfileDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var profile = await GetByIdAsync(id).ConfigureAwait(false);
            // حذف الصورة القديمة إذا تم رفع صورة جديدة
            if (dto.Image != null && dto.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(profile!.ImageUrl))
                {
                    string oldImagePath = Path.Combine(_env.WebRootPath, profile.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                // حفظ الصورة الجديدة
                profile.ImageUrl = await SaveImageAsync(dto.Image).ConfigureAwait(false);
            }

            // تحديث باقي البيانات
            profile!.Name = dto.Name;
            profile!.Title = dto.Title;
            profile.Email = dto.Email;
            profile.Bio = dto.Bio;
            profile.LinkedInUrl = dto.LinkedInUrl;
            profile.GitHubUrl = dto.GitHubUrl;

            _context.Profiles.Update(profile);
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

    }
}

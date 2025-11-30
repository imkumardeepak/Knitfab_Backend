using Microsoft.EntityFrameworkCore;
using AvyyanBackend.Data;
using AvyyanBackend.Models;
using AvyyanBackend.Interfaces;

namespace AvyyanBackend.Repositories
{
    public class SlitLineRepository : ISlitLineRepository
    {
        private readonly ApplicationDbContext _context;

        public SlitLineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SlitLineMaster>> GetAllAsync()
        {
            return await _context.SlitLineMasters
                .Where(sl => sl.IsActive)
                .OrderBy(sl => sl.SlitLine)
                .ToListAsync();
        }

        public async Task<SlitLineMaster?> GetByIdAsync(int id)
        {
            return await _context.SlitLineMasters
                .FirstOrDefaultAsync(sl => sl.Id == id);
        }

        public async Task<SlitLineMaster> CreateAsync(SlitLineMaster slitLine)
        {
            _context.SlitLineMasters.Add(slitLine);
            await _context.SaveChangesAsync();
            return slitLine;
        }

        public async Task<SlitLineMaster?> UpdateAsync(SlitLineMaster slitLine)
        {
            var existingSlitLine = await _context.SlitLineMasters.FindAsync(slitLine.Id);
            if (existingSlitLine == null)
                return null;

            existingSlitLine.SlitLine = slitLine.SlitLine;
            existingSlitLine.SlitLineCode = slitLine.SlitLineCode;
            existingSlitLine.IsActive = slitLine.IsActive;
            existingSlitLine.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingSlitLine;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var slitLine = await _context.SlitLineMasters.FindAsync(id);
            if (slitLine == null)
                return false;

            // Soft delete
            slitLine.IsActive = false;
            slitLine.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SlitLineMaster>> SearchAsync(string? slitLine, char? slitLineCode, bool? isActive)
        {
            var query = _context.SlitLineMasters.AsQueryable();

            if (!string.IsNullOrEmpty(slitLine))
                query = query.Where(sl => sl.SlitLine.Contains(slitLine));

            if (slitLineCode.HasValue)
                query = query.Where(sl => sl.SlitLineCode == slitLineCode.Value);

            if (isActive.HasValue)
                query = query.Where(sl => sl.IsActive == isActive.Value);

            return await query
                .OrderBy(sl => sl.SlitLine)
                .ToListAsync();
        }
    }
}
using Abacus.API.Data;
using Abacus.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Abacus.API.Repositories.Impl
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly WorkflowDbContext _context;

        public TemplateRepository(WorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowTemplate>> GetAllAsync()
        {
            return await _context.WorkflowTemplates
                .Include(t => t.Tasks)
                .Include(t => t.Transitions)
                .ToListAsync();
        }

        public async Task<WorkflowTemplate> GetByIdAsync(int id)
        {
            return await _context.WorkflowTemplates.FindAsync(id);
        }

        public async Task<WorkflowTemplate> GetByIdWithDetailsAsync(int id)
        {
            return await _context.WorkflowTemplates
                .Include(t => t.Tasks)
                .Include(t => t.Transitions)
                    .ThenInclude(tr => tr.FromTask)
                .Include(t => t.Transitions)
                    .ThenInclude(tr => tr.ToTask)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<WorkflowTemplate> CreateAsync(WorkflowTemplate template)
        {
            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<WorkflowTemplate> UpdateAsync(WorkflowTemplate template)
        {
            template.UpdatedAt = DateTime.UtcNow;

            _context.Entry(template).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task DeleteAsync(int id)
        {
            var template = await _context.WorkflowTemplates.FindAsync(id);
            if (template != null)
            {
                _context.WorkflowTemplates.Remove(template);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.WorkflowTemplates.AnyAsync(e => e.Id == id);
        }
    }
}
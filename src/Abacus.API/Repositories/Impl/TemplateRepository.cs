using System.Linq.Expressions;
using Abacus.API.Data;
using Abacus.Core;
using Abacus.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Abacus.API.Repositories.Impl
{
    public class TemplateRepository : IDataProvider<WorkflowTemplate>
    {
        private readonly WorkflowDbContext _context;

        public TemplateRepository(WorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowTemplate>> GetAll(Expression<Func<WorkflowTemplate, bool>> expression = null)
        {
            return await _context.WorkflowTemplates
                .Include(t => t.Tasks)
                .Include(t => t.Transitions)
                .Where(expression ?? (t => true))
                .ToListAsync();
        }

        public async Task<WorkflowTemplate> GetById(int id)
        {
            return await _context.WorkflowTemplates
                .Include(t => t.Tasks)
                .Include(t => t.Transitions)
                    .ThenInclude(tr => tr.FromTask)
                .Include(t => t.Transitions)
                    .ThenInclude(tr => tr.ToTask)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<WorkflowTemplate> Create(WorkflowTemplate template)
        {
            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<WorkflowTemplate> Update(WorkflowTemplate template)
        {
            template.UpdatedAt = DateTime.UtcNow;

            _context.Entry(template).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task Delete(int id)
        {
            var template = await _context.WorkflowTemplates.FindAsync(id);
            if (template != null)
            {
                _context.WorkflowTemplates.Remove(template);
                await _context.SaveChangesAsync();
            }
        }
    }
}
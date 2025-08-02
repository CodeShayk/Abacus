using System.Linq.Expressions;
using Abacus.API.Data;
using Abacus.Core;
using Abacus.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Abacus.API.Repositories.Impl
{
    public class InstanceRepository : IDataProvider<WorkflowInstance>
    {
        private readonly WorkflowDbContext _context;

        public InstanceRepository(WorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowInstance>> GetAll(Expression<Func<WorkflowInstance, bool>> expression = null)
        {
            return await _context.WorkflowInstances
                .Include(i => i.WorkflowTemplate)
                .Include(i => i.TaskInstances)
                    .ThenInclude(ti => ti.WorkflowTask)
                .Where(expression ?? (i => true))
                .OrderByDescending(i => i.StartedAt)
                .ToListAsync();
        }

        public async Task<WorkflowInstance> GetById(int id)
        {
            return await _context.WorkflowInstances
                .Include(i => i.WorkflowTemplate)
                    .ThenInclude(t => t.Tasks)
                .Include(i => i.WorkflowTemplate)
                    .ThenInclude(t => t.Transitions)
                .Include(i => i.TaskInstances)
                    .ThenInclude(ti => ti.WorkflowTask)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<WorkflowInstance> Create(WorkflowInstance instance)
        {
            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();
            return instance;
        }

        public async Task<WorkflowInstance> Update(WorkflowInstance instance)
        {
            _context.Entry(instance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return instance;
        }

        public async Task Delete(int id)
        {
            var instance = await _context.WorkflowInstances.FindAsync(id);
            if (instance != null)
            {
                _context.WorkflowInstances.Remove(instance);
                await _context.SaveChangesAsync();
            }
        }
    }
}
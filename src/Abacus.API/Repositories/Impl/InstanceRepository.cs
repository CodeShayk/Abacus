using Abacus.API.Data;
using Abacus.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Abacus.API.Repositories.Impl
{
    public class InstanceRepository : IInstanceRepository
    {
        private readonly WorkflowDbContext _context;

        public InstanceRepository(WorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowInstance>> GetAllAsync()
        {
            return await _context.WorkflowInstances
                .Include(i => i.WorkflowTemplate)
                .Include(i => i.TaskInstances)
                    .ThenInclude(ti => ti.WorkflowTask)
                .OrderByDescending(i => i.StartedAt)
                .ToListAsync();
        }

        public async Task<WorkflowInstance> GetByIdAsync(int id)
        {
            return await _context.WorkflowInstances.FindAsync(id);
        }

        public async Task<WorkflowInstance> GetByIdWithDetailsAsync(int id)
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

        public async Task<IEnumerable<WorkflowInstance>> GetByEntityAsync(string entityId, string entityType)
        {
            return await _context.WorkflowInstances
                .Include(i => i.WorkflowTemplate)
                .Include(i => i.TaskInstances)
                    .ThenInclude(ti => ti.WorkflowTask)
                .Where(i => i.EntityId == entityId && i.EntityType == entityType)
                .OrderByDescending(i => i.StartedAt)
                .ToListAsync();
        }

        public async Task<WorkflowInstance> CreateAsync(WorkflowInstance instance)
        {
            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();
            return instance;
        }

        public async Task<WorkflowInstance> UpdateAsync(WorkflowInstance instance)
        {
            _context.Entry(instance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return instance;
        }

        public async Task DeleteAsync(int id)
        {
            var instance = await _context.WorkflowInstances.FindAsync(id);
            if (instance != null)
            {
                _context.WorkflowInstances.Remove(instance);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<WorkflowInstance>> GetRunningInstancesAsync()
        {
            return await _context.WorkflowInstances
                .Include(i => i.WorkflowTemplate)
                    .ThenInclude(t => t.Tasks)
                .Include(i => i.WorkflowTemplate)
                    .ThenInclude(t => t.Transitions)
                .Include(i => i.TaskInstances)
                    .ThenInclude(ti => ti.WorkflowTask)
                .Where(i => i.Status == WorkflowStatus.Running)
                .ToListAsync();
        }
    }
}
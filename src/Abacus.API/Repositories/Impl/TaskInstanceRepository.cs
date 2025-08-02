using System.Linq.Expressions;
using Abacus.API.Data;
using Abacus.Core;
using Abacus.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Abacus.API.Repositories.Impl
{
    public class TaskInstanceRepository : IDataProvider<TaskInstance>
    {
        private readonly WorkflowDbContext _context;

        public TaskInstanceRepository(WorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<TaskInstance> GetById(int id)
        {
            return await _context.TaskInstances
                .Include(ti => ti.WorkflowTask)
                .Include(ti => ti.WorkflowInstance)
                    .ThenInclude(wi => wi.WorkflowTemplate)
                .FirstOrDefaultAsync(ti => ti.Id == id);
        }

        public async Task<TaskInstance> Create(TaskInstance taskInstance)
        {
            _context.TaskInstances.Add(taskInstance);
            await _context.SaveChangesAsync();
            return taskInstance;
        }

        public async Task<TaskInstance> Update(TaskInstance taskInstance)
        {
            _context.Entry(taskInstance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return taskInstance;
        }

        public async Task<IEnumerable<TaskInstance>> GetAll(Expression<Func<TaskInstance, bool>> expression = null)
        {
            return await _context.TaskInstances
                .Include(ti => ti.WorkflowTask)
                .Include(ti => ti.WorkflowInstance)
                    .ThenInclude(wi => wi.WorkflowTemplate)
                .Where(expression ?? (ti => true))
                .OrderBy(ti => ti.CreatedAt)
                .ToListAsync();
        }

        public async Task Delete(int id)
        {
            var task = await _context.TaskInstances.FindAsync(id);
            if (task != null)
            {
                _context.TaskInstances.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
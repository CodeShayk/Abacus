using Abacus.API.Data;
using Abacus.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Abacus.API.Repositories.Impl
{
    public class TaskInstanceRepository : ITaskInstanceRepository
    {
        private readonly WorkflowDbContext _context;

        public TaskInstanceRepository(WorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<TaskInstance> GetByIdAsync(int id)
        {
            return await _context.TaskInstances
                .Include(ti => ti.WorkflowTask)
                .Include(ti => ti.WorkflowInstance)
                    .ThenInclude(wi => wi.WorkflowTemplate)
                .FirstOrDefaultAsync(ti => ti.Id == id);
        }

        public async Task<TaskInstance> CreateAsync(TaskInstance taskInstance)
        {
            _context.TaskInstances.Add(taskInstance);
            await _context.SaveChangesAsync();
            return taskInstance;
        }

        public async Task<TaskInstance> UpdateAsync(TaskInstance taskInstance)
        {
            _context.Entry(taskInstance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return taskInstance;
        }

        public async Task<IEnumerable<TaskInstance>> GetPendingTasksAsync()
        {
            return await _context.TaskInstances
                .Include(ti => ti.WorkflowTask)
                .Include(ti => ti.WorkflowInstance)
                    .ThenInclude(wi => wi.WorkflowTemplate)
                .Where(ti => ti.Status == TaskInstanceStatus.Pending || ti.Status == TaskInstanceStatus.Running)
                .OrderBy(ti => ti.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskInstance>> GetTasksByWorkflowInstanceAsync(int workflowInstanceId)
        {
            return await _context.TaskInstances
                .Include(ti => ti.WorkflowTask)
                .Where(ti => ti.WorkflowInstanceId == workflowInstanceId)
                .OrderBy(ti => ti.CreatedAt)
                .ToListAsync();
        }
    }
}
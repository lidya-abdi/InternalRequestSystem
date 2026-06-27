using InternalRequestSystem.Data;
using InternalRequestSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalRequestSystem.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _context;

        //هاي Dependency Injection.
        public RequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Request> GetAll()
        {
            return _context.Requests.Include(r => r.Department);
        }

        public Request? GetById(int id)
        {
            return _context.Requests.FirstOrDefault(r => r.Id == id);
        }

        public Request? GetByIdWithDepartment(int id)
        {
            return _context.Requests
                .Include(r => r.Department)
                .FirstOrDefault(r => r.Id == id);
        }

        public Request? GetByIdWithApprovals(int id)
        {
            return _context.Requests
                .Include(r => r.Approvals)
                .FirstOrDefault(r => r.Id == id);
        }

        //يعني نقلنا الوصول لقاعدة البيانات من الـ Controller إلى الـ Repository.
        public void Add(Request request)
        {
            _context.Requests.Add(request);
        }

        public void Update(Request request)
        {
            _context.Requests.Update(request);
        }

        public void Delete(Request request)
        {
            _context.Requests.Remove(request);
        }

        public void AddApproval(Approval approval)
        {
            _context.Approvals.Add(approval);
        }

        public void RemoveApprovals(IEnumerable<Approval> approvals)
        {
            _context.Approvals.RemoveRange(approvals);
        }

        public IQueryable<RequestLog> GetLogs()
        {
            return _context.RequestLogs;
        }

        public void AddLog(RequestLog log)
        {
            _context.RequestLogs.Add(log);
        }

        public List<Department> GetDepartments()
        {
            return _context.Departments.ToList();
        }

        public int CountPendingRequests()
        {
            return _context.Requests.Count(r => r.Status == "Pending");
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
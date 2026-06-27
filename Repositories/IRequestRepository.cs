/*
 هذا اسمه Interface.
أي Repository للطلبات لازم يعرف يجيب طلبات، يضيف طلب، يحذف طلب، يضيف Log، ويحفظ.
 */

using InternalRequestSystem.Models;

namespace InternalRequestSystem.Repositories
{
    public interface IRequestRepository
    {
        IQueryable<Request> GetAll();
        Request? GetById(int id);
        Request? GetByIdWithDepartment(int id);
        Request? GetByIdWithApprovals(int id);

        void Add(Request request);
        void Update(Request request);
        void Delete(Request request);

        void AddApproval(Approval approval);
        void RemoveApprovals(IEnumerable<Approval> approvals);

        IQueryable<RequestLog> GetLogs();
        void AddLog(RequestLog log);

        List<Department> GetDepartments();
        int CountPendingRequests();

        void Save();
    }
}
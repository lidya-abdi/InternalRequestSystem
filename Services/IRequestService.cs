using InternalRequestSystem.Models;

namespace InternalRequestSystem.Services
{
    public interface IRequestService
    {
        IQueryable<Request> GetRequestsForUser(
            string? role,
            string? email,
            string? searchText,
            string? statusFilter,
            int? departmentFilter);

        Request? GetById(int id);
        Request? GetDetails(int id);
        Request? GetForDelete(int id);

        void CreateRequest(Request request, string fullName, string email);
        void UpdateRequest(Request updatedRequest, string performedBy);
        void DeleteRequest(int id, string performedBy);

        void ApproveRequest(int id, string performedBy);
        void RejectRequest(int id, string performedBy);
        void MarkRequestInReview(int id, string performedBy);

        List<RequestLog> GetLogs();
        List<RequestLog> GetLogsByRequestId(int requestId);

        List<Department> GetDepartments();
        int CountPendingRequests();
    }
}
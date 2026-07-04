using InternalRequestSystem.Models;
using InternalRequestSystem.Repositories;

namespace InternalRequestSystem.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;

        public RequestService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public IQueryable<Request> GetRequestsForUser(
            string? role,
            string? email,
            string? searchText,
            string? statusFilter,
            int? departmentFilter)
        {
            var requests = _requestRepository.GetAll();

            if (role == "Employee")
            {
                requests = requests.Where(r => r.SubmittedByEmail == email);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                requests = requests.Where(r =>
                    r.Title!.Contains(searchText) ||
                    r.Description!.Contains(searchText) ||
                    r.RequestType!.Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                requests = requests.Where(r => r.Status == statusFilter);
            }

            if (departmentFilter.HasValue)
            {
                requests = requests.Where(r => r.DepartmentId == departmentFilter.Value);
            }

            return requests;
        }

        public Request? GetById(int id)
        {
            return _requestRepository.GetById(id);
        }

        public Request? GetDetails(int id)
        {
            return _requestRepository.GetByIdWithDepartment(id);
        }

        public Request? GetForDelete(int id)
        {
            return _requestRepository.GetByIdWithApprovals(id);
        }

        public void CreateRequest(Request request, string fullName, string email)
        {
            request.Status = "Pending";
            request.CreatedDate = DateTime.Now;
            request.SubmittedByName = fullName;
            request.SubmittedByEmail = email;

            _requestRepository.Add(request);
            _requestRepository.Save();

            AddRequestLog(request.Id, "Created", $"Request '{request.Title}' was created.");
            _requestRepository.Save();
        }

        public void UpdateRequest(Request updatedRequest)
        {
            var request = _requestRepository.GetById(updatedRequest.Id);

            if (request == null)
            {
                return;
            }

            request.Title = updatedRequest.Title;
            request.Description = updatedRequest.Description;
            request.RequestType = updatedRequest.RequestType;

            AddRequestLog(request.Id, "Updated", $"Request '{request.Title}' was updated.");

            _requestRepository.Save();
        }

        public void DeleteRequest(int id)
        {
            var request = _requestRepository.GetByIdWithApprovals(id);

            if (request == null)
            {
                return;
            }

            AddRequestLog(request.Id, "Deleted", $"Request '{request.Title}' was deleted.");

            if (request.Approvals != null && request.Approvals.Any())
            {
                _requestRepository.RemoveApprovals(request.Approvals);
            }

            _requestRepository.Delete(request);
            _requestRepository.Save();
        }

        public void ApproveRequest(int id)
        {
            var request = _requestRepository.GetById(id);

            if (request == null)
            {
                return;
            }

            request.Status = "Approved";

            _requestRepository.AddApproval(new Approval
            {
                RequestId = request.Id,
                Decision = "Approved",
                ApprovedByUserId = null,
                Comment = "Request approved successfully."
            });

            AddRequestLog(request.Id, "Approved", $"Request '{request.Title}' was approved.");

            _requestRepository.Save();
        }

        public void RejectRequest(int id)
        {
            var request = _requestRepository.GetById(id);

            if (request == null)
            {
                return;
            }

            request.Status = "Rejected";

            _requestRepository.AddApproval(new Approval
            {
                RequestId = request.Id,
                Decision = "Rejected",
                ApprovedByUserId = null,
                Comment = "Request rejected."
            });

            AddRequestLog(request.Id, "Rejected", $"Request '{request.Title}' was rejected.");

            _requestRepository.Save();
        }

        public void MarkRequestInReview(int id)
        {
            var request = _requestRepository.GetById(id);

            if (request == null)
            {
                return;
            }

            request.Status = "In Review";

            AddRequestLog(request.Id, "In Review", $"Request '{request.Title}' moved to review stage.");

            _requestRepository.Save();
        }

        public List<RequestLog> GetLogs()
        {
            return _requestRepository.GetLogs()
                .OrderByDescending(l => l.ActionDate)
                .ToList();
        }

        public List<RequestLog> GetLogsByRequestId(int requestId)
        {
            return _requestRepository.GetLogs()
                .Where(l => l.RequestId == requestId)
                .OrderByDescending(l => l.ActionDate)
                .ToList();
        }

        public List<Department> GetDepartments()
        {
            return _requestRepository.GetDepartments();
        }

        public int CountPendingRequests()
        {
            return _requestRepository.CountPendingRequests();
        }

        private void AddRequestLog(int? requestId, string action, string description)
        {
            var log = new RequestLog
            {
                RequestId = requestId,
                Action = action,
                Description = description,
                PerformedBy = "System",
                ActionDate = DateTime.Now
            };

            _requestRepository.AddLog(log);
        }
    }
}
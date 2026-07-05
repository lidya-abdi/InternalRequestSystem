using InternalRequestSystem.Models;
using InternalRequestSystem.Repositories;
using AutoMapper;


namespace InternalRequestSystem.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;

        public RequestService(IRequestRepository requestRepository, IMapper mapper)
        {
            _requestRepository = requestRepository;
            _mapper = mapper;
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

            AddRequestLog(request.Id, "Created", $"Request '{request.Title}' was created.", fullName);

            _requestRepository.Save();
        }

        public void UpdateRequest(Request updatedRequest, string performedBy)
        {
            var request = _requestRepository.GetById(updatedRequest.Id);

            if (request == null)
            {
                return;
            }
            // Use AutoMapper to map the updatedRequest properties to the existing request entity
            _mapper.Map(updatedRequest, request);

            //Before using AutoMapper, you can manually update the properties like this:
            /*
            request.Title = updatedRequest.Title;
            request.Description = updatedRequest.Description;
            request.RequestType = updatedRequest.RequestType;
            */

            AddRequestLog(request.Id, "Updated", $"Request '{request.Title}' was updated.", performedBy);

            _requestRepository.Save();
        }

        public void DeleteRequest(int id, string performedBy)
        {
            var request = _requestRepository.GetByIdWithApprovals(id);

            if (request == null)
            {
                return;
            }

            AddRequestLog(request.Id, "Deleted", $"Request '{request.Title}' was deleted.", performedBy);

            if (request.Approvals != null && request.Approvals.Any())
            {
                _requestRepository.RemoveApprovals(request.Approvals);
            }

            _requestRepository.Delete(request);
            _requestRepository.Save();
        }

        public void ApproveRequest(int id, string performedBy)
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

            AddRequestLog(request.Id, "Approved", $"Request '{request.Title}' was approved.", performedBy);

            _requestRepository.Save();
        }

        public void RejectRequest(int id, string performedBy)
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

            AddRequestLog(request.Id, "Rejected", $"Request '{request.Title}' was rejected.", performedBy);

            _requestRepository.Save();
        }

        public void MarkRequestInReview(int id, string performedBy)
        {
            var request = _requestRepository.GetById(id);

            if (request == null)
            {
                return;
            }

            request.Status = "In Review";

            AddRequestLog(request.Id, "In Review", $"Request '{request.Title}' moved to review stage.", performedBy);

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

        private void AddRequestLog(int? requestId, string action, string description, string performedBy)
        {
            var log = new RequestLog
            {
                RequestId = requestId,
                Action = action,
                Description = description,
                PerformedBy = performedBy,
                ActionDate = DateTime.Now
            };

            _requestRepository.AddLog(log);
        }
    }
}
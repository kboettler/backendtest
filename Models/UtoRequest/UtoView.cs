using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public class UtoView : IListener
    {
        private readonly IDictionary<int, (UtoRequest request, RequestStatus status)> _requests 
            = new Dictionary<int, (UtoRequest, RequestStatus)>();

        public UtoRequest GetRequest(int id)
        {
            if(!_requests.ContainsKey(id))
            {
                throw new ArgumentException("The specified request did not exist");
            }
            
            return _requests[id].request;
        }

        public IEnumerable<UtoRequest> GetRequests(Employee employee)
        {
            return _requests.Values
                .Select(t => t.request)
                .Where(t => t.Requester.Equals(employee));
        }

        public bool RequestExists(int id)
        {
            return _requests.ContainsKey(id);
        }

        public RequestStatus GetStatus(UtoRequest request)
        {
            if(!_requests.ContainsKey(request.Id))
            {
                throw new ArgumentException("The specified request did not exist");
            }

            return _requests[request.Id].status;
        }

        public void RecordEvent(StoredEvent stored)
        {
            switch (stored.EventName)
            {
                case nameof(RequestCreated):
                    {
                        var created = stored.ToEvent<RequestCreated>();
                        _requests.Add(created.Value.Id, (created.Value, RequestStatus.Pending));
                        break;
                    }
                case nameof(RequestRemoved):
                    {
                        var removed = stored.ToEvent<RequestRemoved>();
                        _requests.Remove(removed.Id);
                        break;
                    }
                case nameof(RequestDenied):
                    {
                        var denied = stored.ToEvent<RequestDenied>();
                        var request = _requests[denied.Id].request;

                        _requests[denied.Id] = (request, RequestStatus.Denied);
                        break;
                    }
                case nameof(RequestApproved):
                    {
                        var approved = stored.ToEvent<RequestApproved>();
                        var request = _requests[approved.Id].request;

                        _requests[approved.Id] = (request, RequestStatus.Approved);
                        break;
                    }
            }
        }
    }
}

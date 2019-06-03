using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Model.Services;

namespace Backend.Model
{
    public class UtoRequest
    {
        private const uint WorkingHoursPerDay = 8;

        public static IEnumerable<UtoRequest> Create(int id, Employee employee, DateTime startDay, DateTime endDay)
        {
            if (startDay < DateTime.Today)
            {
                throw new ArgumentException("Requested start day was before today");
            }
            else if (endDay < startDay)
            {
                throw new ArgumentException("Requested end date was before requested start date");
            }

            var dateRange =
                Enumerable.Range(0, endDay.Subtract(startDay).Days)
                .Select(offset => startDay.AddDays(offset));
            //todo increment id?
            return dateRange.Select(d => new UtoRequest(id, employee, d, WorkingHoursPerDay));
        }

        public int Id { get; }
        public Employee Requester { get; }
        public DateTime Day { get; }
        public uint Hours { get; }

        public UtoRequest(int id, Employee requester, DateTime day, uint hours)
        {
            Id = id;
            Requester = requester;
            Day = day;
            Hours = hours;
        }
    }

    public class UtoRequestMetadata
    {
        public int EmployeeId { get; }
        public DateTime Day { get; }
        public uint Hours { get; }

        public UtoRequestMetadata(int employeeId, DateTime day, uint hours)
        {
            EmployeeId = employeeId;
            Day = day;
            Hours = hours;
        }
    }

    public enum RequestStatus
    {
        Pending,
        Approved,
        Denied
    }

    public class RequestCreated : IEvent
    {
        public UtoRequest Value { get; }

        public RequestCreated(UtoRequest value)
        {
            Value = value;
        }
    }

    public class RequestRemoved : IEvent
    {
        public int Id { get; }

        public RequestRemoved(int id)
        {
            Id = id;
        }
    }

    public class RequestApproved : IEvent
    {
        public int Id { get; }

        public RequestApproved(int id)
        {
            Id = id;
        }
    }

    public class RequestDenied : IEvent
    {
        public int Id { get; }

        public RequestDenied(int id)
        {
            Id = id;
        }
    }
}
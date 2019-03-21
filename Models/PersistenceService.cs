using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Backend.Model.Services
{
    public class EventListenerCollection : IEventService
    {
        private readonly ImmutableList<IEventService> _listeners;

        public EventListenerCollection(IEnumerable<IEventService> listeners)
        {
            _listeners = ImmutableList<IEventService>.Empty.AddRange(listeners);
        }

        public void RecordEvent(IEvent e)
        {
            foreach (var listener in _listeners)
            {
                listener.RecordEvent(e);
            }
        }
    }

    public class CommandListenerCollection : ICommandService
    {
        private readonly ImmutableList<ICommandService> _listeners;

        public CommandListenerCollection(IEnumerable<ICommandService> listeners)
        {
            _listeners = ImmutableList<ICommandService>.Empty.AddRange(listeners);
        }

        public void IssueCommand(ICommand command)
        {
            foreach (var listener in _listeners)
            {
                listener.IssueCommand(command);
            }
        }
    }
}
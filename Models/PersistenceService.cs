using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Model.Services
{
    public class CommandListenerCollection : ICommandService
    {
        private readonly ImmutableList<ICommandService> _listeners;

        public CommandListenerCollection(IEnumerable<ICommandService> listeners)
        {
            _listeners = ImmutableList<ICommandService>.Empty.AddRange(listeners);
        }

        public async Task IssueCommand(ICommand command)
        {
            foreach (var listener in _listeners.AsParallel())
            {
                await listener.IssueCommand(command);
            }
        }
    }
}
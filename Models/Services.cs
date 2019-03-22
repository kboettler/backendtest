
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Backend.Model.Services
{
    public interface IViewService
    {
        void RecordEvent(ResolvedEvent resolved);
    }

    public interface ICommandService
    {
        Task IssueCommand(ICommand command);
    }

    public interface ICommand
    {

    }

    public interface IEvent
    {
    }

    public static class ServiceHelpers
    {
        // private static readonly Lazy<ImmutableDictionary<string, Type>> _eventTypes = 
        //     new Lazy<ImmutableDictionary<string, Type>>(() =>
        //         {
        //             return Assembly.GetExecutingAssembly()
        //                 .GetExportedTypes()
        //                 .Where(t => typeof(IEvent).IsAssignableFrom(t) && 
        //                     t.IsAbstract == false)
        //                     .ToImmutableDictionary(t => t.Name);
        //         });

        public static EventData GenerateData(this IEvent e)
        {
            var data = JsonConvert.SerializeObject(e);
            return new EventData(Guid.NewGuid(), e.Type(), true, Encoding.UTF8.GetBytes(data), new byte[]{});
        }

        //public static 

        public static string Type(this IEvent e)
        {
            return e.GetType().Name;
        }
    }
}
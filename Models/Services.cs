
namespace Backend.Model.Services
{
    public interface IEventService
    {
        void RecordEvent(IEvent e);
    }

    public interface ICommandService
    {
        void IssueCommand(ICommand command);
    }

    public interface ICommand
    {

    }

    public interface IEvent
    {

    }
}
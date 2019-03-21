using System;
using Backend.Model.Services;
using TestingDb;
using Microsoft.Extensions.DependencyInjection;
using Backend.Model;

namespace Microsoft.Extensions.DependencyInjection{
    public static class Initialization{
        public static void InitializeStudents(this IServiceCollection services)
        {
            services.AddTransient<InMemoryStudentDb, InMemoryStudentDb>(p => StudentDbHelper.InitialDb);
            services.AddSingleton<StudentReader, StudentReader>();
            services.AddSingleton<IEventService, EventListenerCollection>(CreateEventListeners);

            services.AddSingleton<StudentWriter, StudentWriter>();
            services.AddTransient<ICommandService, CommandListenerCollection>(CreateCommandListeners);
        }

        private static EventListenerCollection CreateEventListeners(IServiceProvider provider)
        {
            var studentReader = provider.GetService<StudentReader>();

            var collection = new EventListenerCollection(new[]{studentReader});
            Hydrate(provider, collection);

            return collection;
        }

        private static CommandListenerCollection CreateCommandListeners(IServiceProvider provider)
        {
            var studentWriter = provider.GetService<StudentWriter>();

            var collection = new CommandListenerCollection(new[]{studentWriter});
            return collection;
        }

        private static void Hydrate(IServiceProvider provider, EventListenerCollection collection)
        {
            var db = provider.GetService<InMemoryStudentDb>();
            foreach(var student in db.GetAllStudents())
            {
                collection.RecordEvent(new StudentCreated(student));
            }
        }
    }
}
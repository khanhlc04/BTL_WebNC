using BTLChatDemo.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BTLChatDemo.Extensions
{
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Base Repository
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Specific Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IRoomChatRepository, RoomChatRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            return services;
        }
    }
}

using BTL_WebNC.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BTL_WebNC.Extensions
{
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Specific Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IRoomChatRepository, RoomChatRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ITeacherSubjectRepository, TeacherSubjectRepository>();

            return services;
        }
    }
}

namespace EchoProject.Infrastructure.Repositories
{
    using EchoProject.Domain.ProjectAggregate;
    using EchoProject.Domain.Repositories;
    using EchoProject.Infrastructure.Database;
    using Microsoft.EntityFrameworkCore;

    public class BlogPostRepository(EchoDbContext context) : EfRepository<ProjectBlogPost>(context), IBlogPostRepository;
}
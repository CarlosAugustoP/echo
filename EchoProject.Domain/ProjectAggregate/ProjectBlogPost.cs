using System.Globalization;
using EchoProject.Domain.Common;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Domain.ProjectAggregate
{
    public class ProjectBlogPost : Entity
    {
        public ImageUrl? HeaderImage { get; set; }
        private readonly List<ImageUrl> _images = []; 
        public IReadOnlyCollection<ImageUrl> Images => _images.AsReadOnly();
        public string Content { get; set; } = string.Empty;
        public Guid ProjectId { get; private set; }
        public virtual Project Project { get; private set; } = null!;

        private ProjectBlogPost() { } // EF Core

        public ProjectBlogPost(ImageUrl? headerImage, string content, Project project, List<ImageUrl>? images = null)
        {
            HeaderImage = headerImage;
            Content = content;
            Project = project;
            ProjectId = project.Id;
            if (images != null)
            {
                _images.AddRange(images);
            }   
        }

        public void AddImage(ImageUrl image)
        {
            if (!_images.Contains(image))
                _images.Add(image);
        }
    }
}
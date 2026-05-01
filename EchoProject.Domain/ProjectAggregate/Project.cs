using EchoProject.Domain.Common;
using EchoProject.Domain.Exception.EchoProject.Domain.Common;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Domain.ProjectAggregate
{
    public class Project : Entity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public Guid ManagerId { get; private set; }
        public virtual User Manager { get; private set; } = null!;
        public SmartContractAddress SmartContractAddress { get; private set; }
        public ImageUrl? MainImage { get; private set; } = null;
        private readonly List<ImageUrl> _images = [];
        public IReadOnlyCollection<ImageUrl> Images => _images.AsReadOnly();
        private readonly List<Goal> _goals = [];
        private readonly List<ProjectBlogPost> _blogPosts = [];
        public IReadOnlyCollection<Goal> Goals => _goals.AsReadOnly();
        public IReadOnlyCollection<ProjectBlogPost> BlogPosts => _blogPosts.AsReadOnly();
        private Project() { } // EF Core
        public Project(string title, string description, Guid managerId)
        {
            Id = Guid.NewGuid();
            Title = title.Length < 100 && title.Length > 0 
                ? title 
                : throw new ArgumentException("O título deve ter entre 1 e 100 caracteres.");
            Description = (description.Length < 2000 && description.Length > 0)  
                ? description
                : throw new ArgumentException("A descrição deve ter entre 1 e 2000 caracteres.");
            ManagerId = managerId;
            SmartContractAddress = new SmartContractAddress("TemporaryAddress");
        }

        public void SetSmartContractAddress(string address)
        {
            SmartContractAddress = new SmartContractAddress(address);
        }
    public decimal GetProgress()
        {
            var g = Goals.Where(x => x.GoalType.Name != PresetName.Money).ToDictionary(g => g.Id, g => g.CurrentAmount / g.TargetAmount);
            return g.Count > 0 ? g.Values.Average() * 100 : 0;
        }

        public Goal AddGoal(string title, decimal target, GoalType goalType, decimal? costPerUnit, string? description = null)
        {
            var goal = new Goal(Id, title, target, goalType, costPerUnit, description);
            _goals.Add(goal);
            return goal;
        }

        public ProjectBlogPost AddBlogPost(ProjectBlogPost pbp)
        {
            _blogPosts.Add(pbp);
            return pbp;
        }

        public Goal RemoveGoal(Guid goalId)
        {
            var goal = _goals.FirstOrDefault(g => g.Id == goalId);
            if (goal is not null)
            { 
                _goals.Remove(goal);
                return goal;
            }
            throw new ArgumentException("Meta não encontrada.");
        }

        public void AddOrUpdateMainImage(ImageUrl mainImage)
        {
            MainImage = mainImage;
        }
        
        public void RemoveMainImage()
        {
            MainImage = null;
        }

        public void AddImage(ImageUrl image)
        {
            if (_images.Count < 10) 
            {
                _images.Add(image);
            }
            else throw new DomainException("Não é possível adicionar mais de 10 imagens a um projeto.");
        }

        public void RemoveImage(ImageUrl image)
        {
            _images.Remove(image);
        }

        public void UpdateDetails(string title, string description)
        {
            Title = title.Length < 100 && title.Length > 0 
                ? title 
                : throw new ArgumentException("O título deve ter entre 1 e 100 caracteres.");
            Description = (description.Length < 500 && description.Length > 0)  
                ? description
                : throw new ArgumentException("A descrição deve ter entre 1 e 500 caracteres.");
        }
    }
}

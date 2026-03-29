using EchoProject.Domain.Common;

namespace EchoProject.Domain.ValueObjects
{
    public class ImageUrl : ValueObject
    {
        public string Url { get; private set; }

        public ImageUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL cannot be empty.");

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException("Image URL must be a valid absolute URL.");

            Url = url;
        }

        protected override IEnumerable<string> GetEqualityComponents()
        {
            yield return Url;
        }

        public static implicit operator string(ImageUrl imageUrl) => imageUrl.Url;
        public static implicit operator ImageUrl(string url) => new(url);
    }
}
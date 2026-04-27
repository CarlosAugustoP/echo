using EchoProject.Domain.Common;

namespace EchoProject.Domain.ValueObjects
{
    public class ImageUrl : ValueObject
    {
        public string Url { get; private set; }

        public ImageUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("A URL da imagem não pode estar vazia.");

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException("A URL da imagem deve ser uma URL absoluta válida.");

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

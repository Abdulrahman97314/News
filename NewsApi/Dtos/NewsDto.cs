using System.ComponentModel.DataAnnotations;

namespace News.Dtos
{
    public class NewsDto : IValidatableObject
    {
       public int Id { get; set; }
       public string Title { get; set; }
       public AuthorDto? Author { get; set; }
       public int AuthorId { get; set; }
       public string NewsContent { get; set; }
       public string? ImageUrl { get; set; }
       public DateTime PublicationDate { get; set; }
       public IFormFile Image { get; set; }
       public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           var results = new List<ValidationResult>();

           if (PublicationDate < DateTime.Now || PublicationDate > DateTime.Today.AddDays(7))
           {
             results.Add(new ValidationResult("Publication date must be between today and a week from today", new[] { "PublicationDate" }));
           }

            return results;
        }
    }
}

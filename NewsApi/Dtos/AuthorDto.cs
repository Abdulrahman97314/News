using System.ComponentModel.DataAnnotations;

namespace News.Dtos
{
    public class AuthorDto
    {
        public int Id { get; set; }
        [MaxLength(20, ErrorMessage = "The name must be no more than {1} characters long.")]
        [MinLength(3, ErrorMessage = "The name must be at least {1} characters long.")]
        public string Name { get; set; }

    }
}

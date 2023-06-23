using System.ComponentModel.DataAnnotations;

namespace News.Core.Entities
{
    public class News :BaseEntity
    {
        public string Title { get; set; }
        public virtual Author Author { get; set; }
        public int AuthorId { get; set; }
        public string NewsContent { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}

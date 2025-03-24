using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecuritySWD62A2025.Models.DatabaseModels
{
    public class Article
    {

        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public string AuthorFK { get; set; } //ForeignKey
 
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string Digest { get; set; }

    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecuritySWD62A2025.Models.DatabaseModels
{
    public class Permission
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]  
        public int Id { get; set; }

        [ForeignKey("Article")]
        public Guid ArticleFK { get; set; }

        public virtual Article Article { get; set; }
       
        public string UsernameFK { get; set; }

    }
}

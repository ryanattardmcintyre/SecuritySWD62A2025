using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecuritySWD62A2025.Models.DatabaseModels
{
    public class AsymmetricKeys
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}

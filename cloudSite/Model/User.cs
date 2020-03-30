using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace cloudSite.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        
        [JsonIgnore]
        public string Password { get; set; }
    }
}
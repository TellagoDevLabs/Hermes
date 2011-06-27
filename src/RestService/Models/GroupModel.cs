using System.ComponentModel.DataAnnotations;

namespace TellagoStudios.Hermes.RestService.Models
{
    public class GroupModel
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentId { get; set; }
    }
}
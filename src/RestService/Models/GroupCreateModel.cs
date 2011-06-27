using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TellagoStudios.Hermes.RestService.Models
{
    public class GroupCreateModel
    {

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentId { get; set; }

        public IEnumerable<GroupModel> GroupList { get; set; }
    }
}
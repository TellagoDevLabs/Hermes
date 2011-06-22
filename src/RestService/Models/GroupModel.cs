using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Models
{
    public class GroupModel
    {
        public GroupModel(Group group)
        {
            Id = group.Id.ToString();
            Name = group.Name;
            Description = group.Description;
            ParentId = group.ParentId.ToString();
        }

        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentId { get; set; }
    }
}
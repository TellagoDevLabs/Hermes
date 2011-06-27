using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Models
{
    public class EditTopicModel
    {
        public EditTopicModel(
            Topic topic, IEnumerable<Group> groups)
        {
            Name = topic.Name;
            Description = topic.Description;
            Groups = groups;
            Group = Groups.First(g => g.Id == topic.GroupId);
        }

        public IEnumerable<Group> Groups { get; private set; }
        
        [Required(ErrorMessage = "Name is requierd")]
        public string Name { get; set; }

        public string Description { get; set; }
        
        [Required(ErrorMessage = "Group is requierd")]
        public Group Group { get; set; }
    }
}
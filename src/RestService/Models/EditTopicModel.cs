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
            : this(groups)
        {
            Name = topic.Name;
            Description = topic.Description;
            
            Group = (string) topic.GroupId;
            TopicId = (string) topic.Id.Value;
        }

        public EditTopicModel()
        {}

        public EditTopicModel(IEnumerable<Group> groups)
        {
            Groups = groups.Select(g => new GroupViewModel((string)g.Id.Value, g.Name)).ToArray();
        }

        [Key]
        [Required(ErrorMessage = "Id is required")]
        public string TopicId { get; set; }

        public IEnumerable<GroupViewModel> Groups { get; private set; }
        
        [Required(ErrorMessage = "Name is requierd")]
        public string Name { get; set; }

        public string Description { get; set; }
        
        [Required(ErrorMessage = "Group is requierd")]
        public string Group { get; set; }

        public class GroupViewModel
        {
            public GroupViewModel(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public string Id { get; set; }
            public string Name { get; set; }
        }
    }

    
}
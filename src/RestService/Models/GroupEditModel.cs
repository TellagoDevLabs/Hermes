using System.Collections.Generic;

namespace TellagoStudios.Hermes.RestService.Models
{
    public class GroupEditModel
    {
        public GroupModel Group { get; set; }
        public IEnumerable<GroupModel> GroupList { get; set; }
    }
}
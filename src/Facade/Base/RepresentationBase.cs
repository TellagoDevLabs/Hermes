using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TellagoStudios.Hermes.Facade.Base
{
    [XmlRoot(ElementName = "Topic", Namespace = XmlNamespaces.Default)]
    public class RepresentationBase
    {
        public RepresentationBase()
        {
            Links = new List<Link>();
        }

        [XmlArray(ElementName = "links", Order = 50), XmlArrayItem(ElementName = "link")]
        public List<Link> Links { get; set; }
        public string GetLinkForRelation(string relation)
        {
            Link link = Links.FirstOrDefault(l => l.Relation == relation);
            if(link == null)
            {
                throw new KeyNotFoundException(string.Format("Can't find the relation {0}", relation));
            }
            return link.Uri;
        }
    }

    
}
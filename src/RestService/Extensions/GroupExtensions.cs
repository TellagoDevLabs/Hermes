using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Facade;
using TellagoStudios.Hermes.RestService.Resources;
using Group = TellagoStudios.Hermes.Business.Model.Group;
using Identity = TellagoStudios.Hermes.Business.Model.Identity;

namespace TellagoStudios.Hermes.RestService.Extensions
{
    public static class GroupExtensions
    {
        static public Group ToModel(this Facade.GroupPost from)
        {
            if (from == null) return null;

            return new Group
            {
                Name = from.Name,
                Description = from.Description,
                ParentId = from.ParentId.ToModel()
            };
        }

        static public Group ToModel(this Facade.GroupPut from)
        {
            if (from == null) return null;

            return new Group
            {
                Id = from.Id.ToModel(),
                Name = from.Name,
                Description = from.Description,
                ParentId = from.ParentId.ToModel()
            };
        }

        static public Facade.Link ToLink(this Identity? id, string rel)
        {
            if (id.HasValue)
            {
                return id.Value.ToLink(rel);
            }
            return null;
        }

        static public Facade.Link ToLink(this Identity id, string rel)
        {
            return new Facade.Link(ResourceLocation.OfGroup(id).ToString(), rel);
        }

        static public Facade.Group ToFacade(this Group from)
        {
            if (from == null) return null;

            return new Facade.Group
            {
                Id = from.Id.Value.ToFacade(),
                Name = from.Name,
                Description = from.Description,
                Links = GetLinks(from).ToList()
            };
        }

        private static IEnumerable<Link> GetLinks(Group group)
        {
            if(group.ParentId != null)
            {
                yield return group.ParentId.ToLink(Constants.Relationships.Parent);
            }
            yield return new Link(ResourceLocation.OfTopics(), "Create Topics");
        }
    }
}

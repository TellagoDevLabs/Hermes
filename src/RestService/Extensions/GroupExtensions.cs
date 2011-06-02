using System;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Resources;

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
            return new Facade.Link
            {
                rel = rel,
                href = Resources.ResourceLocation.OfGroup(id)
            };
        }

        static public Facade.Group ToFacade(this Group from)
        {
            if (from == null) return null;

            return new Facade.Group
            {
                Id = from.Id.Value.ToFacade(),
                Name = from.Name,
                Description = from.Description,
                Parent = from.ParentId.ToLink(Constants.Relationships.Parent) 
            };
        }
    }
}

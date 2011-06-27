using System;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Resources;

namespace TellagoStudios.Hermes.RestService.Extensions
{
    public static class SubscriptionExtensions
    {
        static public Subscription ToModel(this Facade.SubscriptionPost from)
        {
            if (from == null) return null;

            var kind = from.TopicId.HasValue ? TargetKind.Topic : (from.GroupId.HasValue ? TargetKind.Group : TargetKind.None);
            var id = from.TopicId.HasValue ? from.TopicId.Value.ToModel() : (from.GroupId.HasValue ? from.GroupId.Value.ToModel() : new Identity?());

            return new Subscription
            {
                Callback = from.Callback.ToModel(),
                TargetId = id,
                TargetKind = kind
            };
        }

        static public Subscription ToModel(this Facade.SubscriptionPut from)
        {
            if (from == null) return null;

            return new Subscription
            {
                Callback = from.Callback.ToModel(),
                Id = from.Id.ToModel()
            };
        }

        static public Callback ToModel(this Facade.Callback callback)
        {
            if (callback == null) return null;

            try
            {
                if (string.IsNullOrWhiteSpace(callback.Url)) return null;

                var result = new Callback
                {
                    Url = new Uri(callback.Url)
                };

                switch (callback.Kind)
                {
                    case Facade.CallbackKind.Message:
                        result.Kind = CallbackKind.Message;
                        break;
                    case Facade.CallbackKind.Key:
                        result.Kind = CallbackKind.Key;
                        break;
                    default:
                        throw new InvalidOperationException(string.Format(Messages.CallbackKindUnknown, callback.Kind));

                }
                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException(Messages.InvalidCallbackFormat, "callback", e);
            }
        }

        static public Facade.Subscription ToFacade(this Subscription from)
        {
            if (from == null) return null;

            Guard.Instance
                .ArgumentNotNull(() => from.Id, from.Id)
                .ArgumentNotNull(() => from.TargetId, from.TargetId);

            var subscription  = new Facade.Subscription
            { 
                Callback = from.Callback.ToFacade(),
// ReSharper disable PossibleInvalidOperationException
                Id = from.Id.Value.ToFacade(),
// ReSharper restore PossibleInvalidOperationException
            };

            switch(from.TargetKind)
            {
                case TargetKind.Topic:
                    subscription.Target = new Facade.Link
                                              {
                                                  Rel = Constants.Relationships.Topic, 
// ReSharper disable PossibleInvalidOperationException
                                                  HRef = Resources.ResourceLocation.OfTopic(from.TargetId.Value)
// ReSharper restore PossibleInvalidOperationException
                                              };
                    break;
                case TargetKind.Group:
                    subscription.Target = new Facade.Link
                                              {
                                                  Rel = Constants.Relationships.Group, 
// ReSharper disable PossibleInvalidOperationException
                                                  HRef = Resources.ResourceLocation.OfGroup(from.TargetId.Value)
// ReSharper restore PossibleInvalidOperationException
                                              };
                    break;
                default:
                    throw new InvalidOperationException(string.Format(Messages.TargetKindUnknown, from.TargetKind));
            }

            return subscription;
        }



        static public Facade.Callback ToFacade(this Callback callback)
        {
            if (callback == null || callback.Url==null) return null;

            try
            {

                var result = new Facade.Callback
                {
                    Url = callback.Url.ToString()
                };

                switch (callback.Kind)
                {
                    case CallbackKind.Message:
                        result.Kind = Facade.CallbackKind.Message;
                        break;
                    case CallbackKind.Key :
                        result.Kind = Facade.CallbackKind.Key;
                        break;
                    default:
                        throw new InvalidOperationException(string.Format(Messages.CallbackKindUnknown, callback.Kind));

                }
                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException(Messages.InvalidCallbackFormat, "callback", e);
            }
        }


        //static public Filter ToModel(this Facade.Filter from)
        //{
        //    if (from == null) return null;

        //    return new Filter
        //    {
        //        Name = from.Name,
        //        Value = from.Value
        //    };
        //}

        //static public Facade.Filter ToFacade(this Filter from)
        //{
        //    if (from == null) return null;

        //    return new Facade.Filter
        //    {
        //        Name = from.Name,
        //        Value = from.Value
        //    };
        //}
    }
}

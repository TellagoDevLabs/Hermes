using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace TellagoStudios.Hermes.Client
{
    public class SubscriptionToTopic<T> : IObservable<Message<T>>
        where T : class, new()
    {
        readonly private RestClient restClient;
        readonly private Topic topic;

        private Uri last;
        private readonly ISubject<Message<T>> subject = new Subject<Message<T>>();
        private readonly object sync = new object();

        public SubscriptionToTopic(Topic topic, RestClient restClient, TimeSpan interval, Uri last = null, IScheduler scheduler = null)
        {
            this.restClient = restClient;
            this.last = last;
            this.topic = topic;

            Observable.Timer(interval, scheduler ?? Scheduler.TaskPool)
                .Repeat()
                .Subscribe(u => GetMessages());
        }

        public void GetMessages()
        {
            lock (sync)
            {
                foreach (var url in topic.PollMessages(last))
                {
                    last = url;
                    var data = restClient.Get<T>(url);
                    subject.OnNext(new Message<T> { Url = url, Data = data });
                }
            }
        }

        public IDisposable Subscribe(IObserver<Message<T>> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
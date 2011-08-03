///<reference path="../jquery.json2xml.js" />
///<reference path="../Rx.js" />
///<reference path="../rx.jQuery.js" />
///<reference path="../RestClient.js" />
///<reference path="../Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

$(document).ready(function () {

    module('Feed Tests');

    var serviceUrl = 'http://localhost:6156';

    test("Always passes", function () {
        ok(true);
    });

    test('GetAllMessages returns an Obserable', function () {
        var groupName = 'GetAllMessages returns an Obserable';
        var topicName = groupName;
        var interval = 1; // second
        usingTopicWithMessages(groupName, topicName, function (topic) {
            start();
            var actual = topic.GetAllMessages();
            ok('Subscribe' in actual);
        });
    });

    test('GetAllMessages Observable has messages', function () {
        var groupName = 'GetAllMessages Observable has messages';
        var topicName = groupName;
        var interval = 1; // second
        usingTopicWithMessages(groupName, topicName, function (topic) {
            var items = [];
            var observable = topic.GetAllMessages();
            var observer = Rx.Observer.Create(
                function (next) {
                    items.push(next);
                },
                function (err) {
                    console.log('Error');
                    start();
                    ok(false, 'Observer got an error.');
                },
                function () {
                    start();
                    console.log(items);
                    ok(items.length > 10);
                }
            );
            observable.Subscribe(observer);
        });
    });

    test('PollFeed has messages', function () {
        var groupName = 'PollFeed has messages';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var items = [];
            var observable = topic.PollFeed(500);
            var observer = Rx.Observer.Create(
                function (next) {
                    items.push(next);
                },
                function (err) {
                    start();
                    ok(false, 'Observer got an error.');
                },
                function () {
                    start();
                    console.log(items);
                    ok(false, 'A polling subscription never completes.');
                }
            );
            var subscription = observable.Subscribe(observer);

            setTimeout(function () {
                topic.PostStringMessage('1');
            }, 1000);

            setTimeout(function () {
                topic.PostStringMessage('2');
            }, 2000);

            setTimeout(function () {
                topic.PostStringMessage('3');
            }, 3000);

            setTimeout(function () {
                subscription.Dispose();
                start();
                ok(items.length == 3);
            }, 4000);

        });
    });

    test('topic.PollFeed can manage multiple subscriptions', function () {
        var groupName = 'topicProxy.PollFeed can manage multiple subscriptions';
        var topicName = groupName;
        var message0 = 'Message 0.';
        var message1 = 'Message 1.';
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var observable = topic.PollFeed(500);

            var fail = function () {
                console.log('Test failed');
                start();
                ok(false);
                subscription0.Dispose();
            };

            var pass = function () {
                console.log('Test passed');
                start();
                ok(true);
                subscription0.Dispose();
            };

            // Should receive both messages
            var subscription0 = observable.Subscribe(
                function (next) {
                    console.log('Subscription 0 got message ' + next);
                    if (next == message1)
                        pass();
                },
                fail,
                fail);

            // Should receive message0, but not message1
            var subscription1 = observable.Subscribe(
                function (next) {
                    console.log('Subscription 1 got message ' + next);
                    if (next == message1)
                        fail();
                },
                fail,
                fail);

            setTimeout(function() {
                topic.PostStringMessage(message0);
            }, 100);

            setTimeout(function () {
                subscription1.Dispose();
            }, 1500);

            setTimeout(function () {
                topic.PostStringMessage(message1);
            }, 2500);

        });

    });


});
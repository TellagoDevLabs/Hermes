///<reference path="../jquery.json2xml.js" />
///<reference path="../Rx.js" />
///<reference path="../RestClient.js" />
///<reference path="../Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

$(document).ready(function () {

    module('Feed Tests');

    var serviceUrl = 'http://localhost:6156/';

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

});
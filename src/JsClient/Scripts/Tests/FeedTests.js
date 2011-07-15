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

    test('Get feed', function () {
        var groupName = 'Get feed';
        var topicName = groupName;
        usingTopicWithMessages(groupName, topicName, function (topic) {
            topic.GetFeed()
                .done(function (feed) {
                    start();
                    console.log(feed);
                    ok(true);
                })
                .fail(function () {
                    start();
                    ok(false, 'GetFeed failed.');
                });
        });
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
                    ok(items.length > 10);
                }
            );
            observable.Subscribe(observer);
        });
    })

});
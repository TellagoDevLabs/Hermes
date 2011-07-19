///<reference path="../jquery.json2xml.js" />
///<reference path="../Rx.js" />
///<reference path="../rx.jQuery.js" />
///<reference path="../RestClient.js" />
///<reference path="../Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

$(document).ready(function () {

    module('Message Tests');

    var serviceUrl = 'http://localhost:6156/';

    test("Always passes", function () {
        ok(true);
    });

    test('Post a string message', function () {
        var groupName = 'Post a string message';
        var topicName = groupName;
        var message = 'The crow flies at midnight.';
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            topic.PostStringMessage(message)
                .done(function (location) {
                    start();
                    notEqual(location, null);
                    notEqual(location, '');
                })
                .fail(function () {
                    start();
                    ok(false, 'PostStringMessage failed.');
                });
        });

    });

    test('Post an xml message', function () {
        var groupName = 'Post an xml message';
        var topicName = groupName;
        var message = '<message>The crow flies at midnight.</message>';
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            topic.PostMessage(message, 'application/xml')
                .done(function (location) {
                    start();
                    notEqual(location, null);
                    notEqual(location, '');
                })
                .fail(function () {
                    start();
                    ok(false, 'PostMessage failed.');
                });
        });

    });

    test('Post a json message', function () {
        var groupName = 'Post an xml message';
        var topicName = groupName;
        var msgObject = {
            message: 'The crow flies at midnight.',
            timestamp: new Date()
        };
        var message = JSON.stringify(msgObject);

        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            topic.PostMessage(message, 'application/json')
                .done(function (location) {
                    start();
                    notEqual(location, null);
                    notEqual(location, '');
                })
                .fail(function () {
                    start();
                    ok(false, 'PostMessage failed.');
                });
        });

    });
    
});
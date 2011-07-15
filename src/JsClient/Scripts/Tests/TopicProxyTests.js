///<reference path="../jquery.json2xml.js" />
///<reference path="../Rx.js" />
///<reference path="../rx.jQuery.js" />
///<reference path="../RestClient.js" />
///<reference path="../Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

$(document).ready(function () {

    module('Promise as Topic proxy');

    var serviceUrl = 'http://localhost:6156/';

    test("Always passes", function () {
        ok(true);
    });

    var assertIsTopicProxy = function (proxy) {
        start();
        ok('Delete' in proxy, 'proxy should implement Delete()');
        ok('PostMessage' in proxy, 'proxy should implement PostMessage(data, contentType)');
        ok('PostStringMessage' in proxy, 'proxy should implement PostStringMessage(message)');
    };

    test('group.CreateTopic returns Topic proxy', function () {
        var groupName = 'group.CreateTopic returns Topic proxy';
        var topicName = groupName;
        usingGroupWithoutTopics(groupName, function (group) {
            var actual = group.CreateTopic(topicName, '');
            assertIsTopicProxy(actual);
        });
    });

    test('group.GetTopicByName returns Topic proxy', function () {
        var groupName = 'group.GetTopicByName returns Topic proxy';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var actual = group.GetTopicByName(topicName);
            assertIsTopicProxy(actual);
        });
    });

    test('group.TryCreateTopic returns Topic proxy', function () {
        var groupName = 'group.TryCreateTopic returns Topic proxy';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var actual = group.TryCreateTopic(topicName, '');
            assertIsTopicProxy(actual);
        });
    });

    test('groupProxy.CreateTopic returns Topic proxy', function () {
        var groupName = 'groupProxy.CreateTopic returns Topic proxy';
        var topicName = groupName;
        usingGroupWithoutTopics(groupName, function () {
            var client = new HermesClient(serviceUrl);
            var group = client.TryCreateGroup(groupName, '');
            var actual = group.CreateTopic(topicName, '');
            assertIsTopicProxy(actual);
        });
    });

    test('groupProxy.GetTopicByName returns Topic proxy', function () {
        var groupName = 'groupProxy.GetTopicByName returns Topic proxy';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function () {
            var client = new HermesClient(serviceUrl);
            var group = client.TryCreateGroup(groupName, '');
            var actual = group.GetTopicByName(topicName);
            assertIsTopicProxy(actual);
        });
    });

    test('groupProxy.TryCreateTopic returns Topic proxy', function () {
        var groupName = 'groupProxy.TryCreateTopic returns Topic proxy';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function () {
            var client = new HermesClient(serviceUrl);
            var group = client.TryCreateGroup(groupName, '');
            var actual = group.TryCreateTopic(topicName, '');
            assertIsTopicProxy(actual);
        });
    });

});
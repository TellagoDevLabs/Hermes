///<reference path="../jquery.json2xml.js" />
///<reference path="../Rx.js" />
///<reference path="../rx.jQuery.js" />
///<reference path="../RestClient.js" />
///<reference path="../Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

$(document).ready(function () {

    module('Promise as Group proxy');

    var serviceUrl = 'http://localhost:6156/';

    test("Always passes", function () {
        ok(true);
    });

    var assertIsGroupProxy = function (proxy) {
        start();
        ok('Delete' in proxy, 'proxy should implement Delete()');
        ok('GetTopics' in proxy, 'proxy should implement GetTopics()');
        ok('CreateTopic' in proxy, 'proxy should implement CreateTopic(topicName, topicDescription)');
        ok('GetTopicByName' in proxy, 'proxy should implement GetTopicByName(topicName)');
        ok('TryCreateTopic' in proxy, 'proxy should implement TryCreateTopic(topicName, topicDescription)');
    };

    test("CreateGroup(name, description) returns Group proxy", function () {
        var groupName = 'CreateGroup(name, description) returns Group proxy';
        removeGroup(groupName, function () {
            var client = new HermesClient(serviceUrl);
            var actual = client.CreateGroup(groupName, '');
            assertIsGroupProxy(actual);
        });
    });

    test("GetGroupByName(name) returns Group proxy", function () {
        var groupName = 'GetGroupByName(name) returns Group proxy';
        usingGroup(groupName, function (group) {
            var client = new HermesClient(serviceUrl);
            var actual = client.GetGroupByName(groupName);
            assertIsGroupProxy(actual);
        });
    });

    test("GetGroup(id) returns Group proxy", function () {
        var groupName = 'GetGroup(id) returns Group proxy';
        usingGroup(groupName, function (group) {
            var client = new HermesClient(serviceUrl);
            var actual = client.GetGroup(group.getId());
            assertIsGroupProxy(actual);
        });
    });

    test("TryCreateGroup(name, description) returns Group proxy", function() {
        stop();
        var groupName = 'TryCreateGroup(name, description) returns Group proxy';
        var client = new HermesClient(serviceUrl);
        var actual = client.TryCreateGroup(groupName, '');
        assertIsGroupProxy(actual);
    });

});
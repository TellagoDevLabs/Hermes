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
        ok('Delete' in proxy && $.isFunction(proxy.Delete), 'proxy should implement Delete()');
        ok('GetTopics' in proxy && $.isFunction(proxy.GetTopic), 'proxy should implement GetTopics()');
        ok('CreateTopic' in proxy && $.isFunction(proxy.CreateTopic), 'proxy should implement CreateTopic(topicName, topicDescription)');
        ok('GetTopicByName' in proxy && $.isFunction(proxy.GetTopicByName), 'proxy should implement GetTopicByName(topicName)');
        ok('GetTopic' in proxy && $.isFunction(proxy.GetTopic), 'proxy should implement GetTopic(id)');
        ok('TryCreateTopic' in proxy && $.isFunction(proxy.TryCreateTopic), 'proxy should implement TryCreateTopic(topicName, topicDescription)');
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

    test("TryCreateGroup(name, description) returns Group proxy", function () {
        stop();
        var groupName = 'TryCreateGroup(name, description) returns Group proxy';
        var client = new HermesClient(serviceUrl);
        var actual = client.TryCreateGroup(groupName, '');
        assertIsGroupProxy(actual);
    });

    test("Group proxy Delete works", function () {
        var groupName = 'Group proxy Delete works';
        usingGroup(groupName, function (group) {
            var groupId = group.getId();
            var client = new HermesClient(serviceUrl);
            var proxy = client.TryCreateGroup(groupName);
            proxy.Delete()
                .done(function () {
                    client.GetGroup(groupId)
                        .done(function (grp) {
                            start();
                            ok(false, 'GetGroup should fail if the group was deleted.');
                        })
                        .fail(function () {
                            start();
                            ok(true, 'GetGroup failed, meaning the group is deleted.');
                        });
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateGroup or Delete failed.');
                });
        });
    });

    test('Group proxy GetTopics works', function () {
        var groupName = 'Group proxy GetTopics works';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function () {
            var client = new HermesClient(serviceUrl);
            var proxy = client.TryCreateGroup(groupName, '');
            proxy.GetTopics()
                .done(function (topics) {
                    start();
                    ok(topics.length > 0);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateGroup or GetTopics failed.');
                });
        });
    });

    test("Group proxy CreateTopic works", function () {
        var groupName = 'Group proxy CreateTopic works';
        usingGroupWithoutTopics(groupName, function (group) {
            var client = new HermesClient(serviceUrl);
            var proxy = client.TryCreateGroup(groupName);
            proxy.CreateTopic(groupName, 'My Topic Description')
                .done(function (topic) {
                    start();
                    ok(topic instanceof Topic);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateGroup or CreateTopic failed');
                });
        });
    });

    test("Group proxy GetTopicByName works", function () {
        var groupName = 'Group proxy GetTopicByName works';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group) {
            var client = new HermesClient(serviceUrl);
            var proxy = client.TryCreateGroup(groupName);
            proxy.GetTopicByName(topicName)
                .done(function (topic) {
                    start();
                    ok(topic instanceof Topic);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateGroup or TryCreateTopic failed');
                });
        });
    });

    test("Group proxy GetTopicByName works", function () {
        var groupName = 'Group proxy GetTopicByName works';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var topicId = topic.getId();
            var client = new HermesClient(serviceUrl);
            var proxy = client.TryCreateGroup(groupName);
            proxy.GetTopic(topicId)
                .done(function (topic) {
                    start();
                    ok(topic instanceof Topic);
                    equal(topic.getId(), topicId);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateGroup or TryCreateTopic failed');
                });
        });
    });


    test("Group proxy TryCreateTopic works", function () {
        var groupName = 'Group proxy TryCreateTopic works';
        usingGroupWithoutTopics(groupName, function (group) {
            var client = new HermesClient(serviceUrl);
            var proxy = client.TryCreateGroup(groupName);
            proxy.TryCreateTopic(groupName, 'My Topic Description')
                .done(function (topic) {
                    start();
                    ok(topic instanceof Topic);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateGroup or TryCreateTopic failed');
                });
        });
    });

});
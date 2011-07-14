///<reference path="../jquery.json2xml.js" />
///<reference path="../RestClient.js" />
///<reference path="../Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

$(document).ready(function () {

    module('Group Tests');

    var serviceUrl = 'http://localhost:6156/';

    module('group.Delete');

    test('Delete returns promise', function () {
        whenCreateGroupCompletes('Delete returns promise', '', function (group) {
            start();
            var actual = group.Delete();
            ok('done' in actual && 'fail' in actual);
        });
    });

    test('Delete completes successfully', function () {
        whenCreateGroupCompletes('Delete returns promise', '', function (group) {
            group.Delete()
                .done(function () {
                    start();
                    ok(true);
                })
                .fail(function () {
                    start();
                    ok(false, 'Delete failed.');
                });
        });
    });

    module('group.SaveChanges()');

    test('SaveChanges completes', function () {
        whenCreateGroupCompletes('SaveChanges completes', '', function (group) {
            group.Description = 'This is a new description';
            group.SaveChanges().done(function () {
                start();
                ok(true);
            })
                .fail(function () {
                    start();
                    ok(false, 'SaveChanges failed.');
                });
        });
    });

    test('SaveChanges updates description', function () {
        whenCreateGroupCompletes('SaveChanges updates description', 'old description', function (group) {
            group.Description = 'new description';
            group.SaveChanges().done(function () {
                var client = new HermesClient(serviceUrl);
                client.GetGroupByName(group.Name)
                    .done(function (refreshedGroup) {
                        start();
                        equal(refreshedGroup.Description, group.Description);
                    })
                    .fail(function () {
                        start();
                        ok(false, 'GetGroupByName failed.');
                    });
            })
                .fail(function () {
                    start();
                    ok(false, 'SaveChanges failed.');
                });
        });
    });

    module('group.CreateTopic');



    test('group.CreateTopic returns a promise', function () {
        var groupName = 'group.CreateTopic returns a promise';
        usingGroup(groupName, function (group) {
            start();
            var actual = group.CreateTopic('group.CreateTopic returns a promise', '');
            ok('done' in actual && 'fail' in actual);
        });
    });

    test('when group.CreateTopic completes, it returns a topic', function () {
        var groupName = 'when group.CreateTopic completes, it returns a topic';
        var topicName = groupName;
        usingGroupWithoutTopics(groupName, function (group) {
            group.CreateTopic(topicName, '')
                .done(function (topic) {
                    start();
                    ok(topic instanceof Topic);
                })
                .fail(function () {
                    start();
                    ok(false, 'CreateTopic failed.');
                });
        });
    });

    module('group.GetTopics');

    test("group.GetTopics returns a promise", function () {
        var groupName = "group.GetTopics returns a promise";
        usingGroup(groupName, function (group) {
            start();
            var actual = group.GetTopics();
            ok('done' in actual && 'fail' in actual);
        });
    });

    test("when group.GetTopics completes, it returns topics", function () {
        var groupName = "when group.GetTopics completes, it returns topics";
        whenGetTopicsCompletes(groupName, function (topics) {
            start();
            ok(topics.length > 0, 'topics array should not be empty');
            for (var i = 0; i < topics.length; i++)
                ok(topics[i] instanceof Topic);
        });
    });

    module('group.GetTopicByName');

    test('group.GetTopicByName returns promise', function () {
        var groupName = 'group.GetTopicByName returns promise';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var actual = group.GetTopicByName(topicName);
            start();
            ok('done' in actual && 'fail' in actual);
        });
    });

    test('group.GetTopicByName returns the topic', function () {
        var groupName = 'group.GetTopicByName returns the topic';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            group.GetTopicByName(topicName)
                .done(function (actual) {
                    start();
                    notEqual(actual, null);
                    equal(actual.getId(), topic.getId());
                })
                .fail(function () {
                    start();
                    ok(false, 'GetTopicByName failed.');
                });
        });
    });

    module('group.TryCreateTopic');

    test('TryCreateTopic returns a newly created topic', function () {
        var groupName = 'TryCreateTopic returns a newly created topic';
        var topicName = groupName;
        usingGroupWithoutTopics(groupName, function (group) {
            group.TryCreateTopic(topicName)
                .done(function (topic) {
                    start();
                    equal(topic.Name, topicName);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateTopic failed.');
                });
        });
    });

    test('TryCreateTopic returns an existing topic', function () {
        var groupName = 'TryCreateTopic returns an existing topic';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            group.TryCreateTopic(topicName)
                .done(function (topic) {
                    start();
                    equal(topic.Name, topicName);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateTopic failed.');
                });
        });
    });


    module('Deserializing topics');

    test("new topics should have id", function () {
        var groupName = 'new topics should have id';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            var id = topic.getId();
            notEqual(id, null);
            notEqual(id, '');
        });
    });

    test("new topics should have name", function () {
        var groupName = 'new topics should have name';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            equal(topic.Name, topicName);
        });
    });

    test("new topics should have description", function () {
        var groupName = 'new topics should have description';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            var description = topic.Description;
            notEqual(description, null);
        });
    });

    test("new topics should have a group", function () {
        var groupName = 'new topics should have a group';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            equal(topic.getGroup().getId(), group.getId());
        });
    });

    test("new topics should have link to Group", function () {
        var groupName = 'new topics should have link to Group';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            var links = topic.getLinks();
            ok('Group' in links);
            notEqual(links['Group'], null);
            notEqual(links['Group'], '');
        });
    });

    test("new topics should have link to Delete", function () {
        var groupName = 'new topics should have link to Delete';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            var links = topic.getLinks();
            ok('Delete' in links);
            notEqual(links['Delete'], null);
            notEqual(links['Delete'], '');
        });
    });

    test("new topics should have link to Update", function () {
        var groupName = 'new topics should have link to Update';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            var links = topic.getLinks();
            ok('Update' in links);
            notEqual(links['Update'], null);
            notEqual(links['Update'], '');
        });
    });

    test("new topics should have link to Post Message", function () {
        var groupName = 'new topics should have link to Post Message';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            var links = topic.getLinks();
            ok('Post Message' in links);
            notEqual(links['Post Message'], null);
            notEqual(links['Post Message'], '');
        });
    });

    test("new topics should have link to Current Feed", function () {
        var groupName = 'new topics should have link to Current Feed';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            start();
            var links = topic.getLinks();
            ok('Current Feed' in links);
            notEqual(links['Current Feed'], null);
            notEqual(links['Current Feed'], '');
        });
    });


});
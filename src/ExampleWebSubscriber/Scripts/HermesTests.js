///<reference path="jquery.json2xml.js" />
///<reference path="RestClient.js" />
///<reference path="Hermes.js" />
$(document).ready(function () {

    var serviceUrl = 'http://localhost:6156/';

    test("Always passes", function () {
        ok(true);
    });

    module("HermesClient");

    test("Call HermesClient constructor without new", function () {
        var hermes = HermesClient(serviceUrl);
        ok(hermes instanceof HermesClient);
    });

    test("Call HermesClient constructor without serviceUrl", function () {
        raises(function () { HermesClient(); });
    });

    test("Call HermesClient constructor with null serviceUrl", function () {
        raises(function () { HermesClient(null); });
    });

    test("Call HermesClient constructor with empty serviceUrl", function () {
        raises(function () { HermesClient(''); });
    });

    module("HermesClient.GetGroups");

    test("Call hermes.GetGroups returns a promise", function () {
        var client = new HermesClient(serviceUrl);
        var actual = client.GetGroups();
        ok(actual != null && 'done' in actual && 'fail' in actual);
    });

    var ensureAtLeastOneGroupExists = function (action) {
        // Yes, atempt to create this group before each test.
        var client = new HermesClient(serviceUrl);
        client.CreateGroup('junk', '').done(action).fail(action);
        stop();
    };

    var whenGetGroupsCompletes = function (action) {
        ensureAtLeastOneGroupExists(function () {
            var client = new HermesClient(serviceUrl);
            var promise = client.GetGroups();

            promise.done(function (groups) {
                action(groups);
            })
                .fail(function () {
                    start();
                    ok(false, 'call to GetGroups failed.');
                });
        });
    };


    test("when hermes.GetGroups completes, it returns groups", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            ok(groups.length > 0, 'groups array was empty');
            for (var i = 0; i < groups.length; i++)
                ok(groups[i] instanceof Group);
        });
    });

    module('Deserializing groups');

    test("new groups should have id", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var id = groups[0].getId();
            notEqual(id, null);
            notEqual(id, '');
        });
    });

    test("new groups should have name", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var name = groups[0].Name;
            notEqual(name, null);
            notEqual(name, '');
        });
    });

    test("new groups should have description", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var description = groups[0].Description;
            notEqual(description, null);
        });
    });

    test("new groups should have link to create topic", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var links = groups[0].getLinks();
            ok('Create Topic' in links);
            notEqual(links['Create Topic'], null);
            notEqual(links['Create Topic'], '');
        });
    });

    test("new groups should have link to all topics", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var links = groups[0].getLinks();
            ok('All Topics' in links);
            notEqual(links['All Topics'], null);
            notEqual(links['All Topics'], '');
        });
    });

    test("new groups should have link to delete", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var links = groups[0].getLinks();
            ok('Delete' in links);
            notEqual(links['Delete'], null);
            notEqual(links['Delete'], '');
        });
    });

    test("new groups should have link to update", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var links = groups[0].getLinks();
            ok('Update' in links);
            notEqual(links['Update'], null);
            notEqual(links['Update'], '');
        });
    });

    module("HermesClient.CreateGroup");

    var removeGroup = function (groupName, action) {
        var client = new HermesClient(serviceUrl);
        stop();

        var deferred = $.Deferred();

        client.GetGroupByName(groupName)
            .done(function (group) {
                if (group == null) {
                    deferred.resolve();
                    return;
                };
                group.Delete()
                    .done(deferred.resolve)
                    .fail(function () {
                        console.log(groupName + ' could not be deleted. Test setup failed.');
                        deferred.reject();
                    });
            })
            .fail(function () {
                console.log('Unable to get ' + groupName + ' group.');
                deferred.reject();
            });

        deferred.done(action)
            .fail(function () {
                console.log(groupName + ' test setup failed.');
                start();
                ok(false, 'Failed to delete the group before running the test.');
            });
    };

    test("hermes.CreateGroup returns a promise", function () {
        var groupName = 'hermes.CreateGroup returns a promise';
        removeGroup(groupName, function () {
            start();
            var client = new HermesClient(serviceUrl);
            var createGroupPromise = client.CreateGroup('hermes.CreateGroup returns a promise');
            ok(createGroupPromise != null && 'done' in createGroupPromise && 'fail' in createGroupPromise);
        });
    });

    test("hermes.CreateGroup with no name", function () {
        var client = new HermesClient(serviceUrl);
        raises(function () { client.CreateGroup(); });
    });

    test("hermes.CreateGroup with null name", function () {
        var client = new HermesClient(serviceUrl);
        raises(function () { client.CreateGroup(null); });
    });

    test("hermes.CreateGroup with empty name", function () {
        var client = new HermesClient(serviceUrl);
        raises(function () { client.CreateGroup(''); });
    });

    var whenCreateGroupCompletes = function (name, description, action) {
        removeGroup(name, function () {
            var client = new HermesClient(serviceUrl);
            client.CreateGroup(name, description)
                .done(function (group) {
                    action(group);
                })
                .fail(function () {
                    start();
                    ok(false, 'Failed to create ' + name + ' group.');
                });
        });
    };

    test("hermes.CreateGroup completes", function () {
        whenCreateGroupCompletes('{hermes.CreateGroup completes}', '', function (group) {
            start();
            ok(true, 'Group created.');
        });
    });

    test("hermes.CreateGroup returns a Group", function () {
        whenCreateGroupCompletes('{hermes.CreateGroup returns a Group}', '', function (group) {
            start();
            ok(group instanceof Group);
        });
    });

    module('client.GetGroupByName');

    test('GetGroupByName returns group', function () {
        whenCreateGroupCompletes('GetGroupByName returns group', '', function () {
            var client = new HermesClient(serviceUrl);
            client.GetGroupByName('GetGroupByName returns group')
                .done(function (group) {
                    start();
                    ok(group != null);
                })
                .fail(function () {
                    start();
                    ok(false, 'GetGroupByName failed');
                });
        });
    });

    test('GetGroupByName returns null when group doesnt exist', function () {
        removeGroup('GetGroupByName returns null when group doesnt exist', function () {
            var client = new HermesClient(serviceUrl);
            client.GetGroupByName('GetGroupByName returns null when group doesnt exist')
                .done(function (group) {
                    start();
                    ok(group == null);
                })
                .fail(function () {
                    start();
                    ok(false, 'GetGroupByName failed');
                });
        });
    });

    module('client.GetGroup');

    test('GetGroup returns group', function () {
        whenCreateGroupCompletes('GetGroup returns group', '', function (group) {
            var id = group.getId();
            var client = new HermesClient(serviceUrl);
            client.GetGroup(id)
                .done(function (group) {
                    start();
                    ok(group != null);
                })
                .fail(function () {
                    start();
                    ok(false, 'GetGroup failed');
                });
        });
    });

    module('client.TryCreateGroup');

    test('TryCreateGroup returns a newly created group', function () {
        var groupName = 'TryCreateGroup returns a newly created group';
        removeGroup(groupName, function () {
            var client = new HermesClient(serviceUrl);
            client.TryCreateGroup(groupName)
                .done(function (group) {
                    start();
                    equal(group.Name, groupName);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateGroup failed.');
                });
        });
    });

    test('TryCreateGroup returns an existing group', function () {
        var groupName = 'TryCreateGroup returns an existing group';
        whenCreateGroupCompletes(groupName, '', function () {
            var client = new HermesClient(serviceUrl);
            client.TryCreateGroup(groupName)
                .done(function (group) {
                    start();
                    equal(group.Name, groupName);
                })
                .fail(function () {
                    start();
                    ok(false, 'TryCreateGroup failed.');
                });
        });
    });

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

    var usingGroup = function (groupName, action) {
        stop();
        var client = new HermesClient(serviceUrl);
        client.TryCreateGroup(groupName, '')
            .done(action)
            .fail(function () {
                start();
                ok(false, 'call to GetGroups failed.');
            });
    };

    var usingGroupWithoutTopics = function (groupName, action) {
        usingGroup(groupName, function (group) {
            group.GetTopics()
                .done(function (topics) {
                    var promises = [];
                    for (var i = 0; i < topics.length; i++)
                        promises.push(topics[i].Delete());
                    $.when.apply(this, promises)
                        .done(function () {
                            action(group);
                        })
                        .fail(function () {
                            start();
                            ok(false, 'unable to remove one or more groups');
                        });
                })
                .fail(function () {
                    start();
                    ok(false, 'call to GetTopics failed.');
                });
        });
    };

    var usingGroupAndTopic = function (groupName, topicName, action) {
        usingGroupWithoutTopics(groupName, function (group) {
            group.CreateTopic(topicName, '')
                .done(function (topic) {
                    action(group, topic);
                })
                .fail(function () {
                    start();
                    ok(false, 'unable to create topic ' + topicName);
                });
        });
    };

    var whenGetTopicsCompletes = function (groupName, action) {
        usingGroupAndTopic(groupName, groupName, function (group) {
            group.GetTopics()
                .done(action)
                .fail(function () {
                    start();
                    ok(false, 'call to GetTopics failed.');
                });
        });
    };

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

    module("topic.Delete");

    test("topic.Delete returns promise", function () {
        var groupName = 'topic.Delete returns promise';
        usingGroupAndTopic(groupName, groupName, function (group, topic) {
            var actual = topic.Delete();
            start();
            ok('done' in actual && 'fail' in actual);
        });
    });

    test("topic.Delete removes topic", function () {
        var groupName = 'topic.Delete removes topic';
        usingGroupAndTopic(groupName, groupName, function (group, topic) {
            topic.Delete()
                .done(function () {
                    group.GetTopics()
                        .done(function (topics) {
                            start();
                            equal(topics.length, 0);
                        })
                        .fail(function () {
                            start();
                            ok(false, 'group.GetTopics failed.');
                        });
                })
                .fail(function () {
                    start();
                    ok(false, 'topic.Delete failed.');
                });

        });
    });


    module('topic.SaveChanges()');

    test('topic.SaveChanges completes', function () {
        var groupName = 'topic.SaveChanges completes';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            topic.Description = 'This is a new description';
            topic.SaveChanges().done(function () {
                start();
                ok(true);
            })
                .fail(function () {
                    start();
                    ok(false, 'SaveChanges failed.');
                });
        });
    });

    test('topic.SaveChanges updates description', function() {
        var groupName = 'topic.SaveChanges updates description';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function(group, topic) {
            topic.Description = 'new description';
            topic.SaveChanges().done(function() {
                group.GetTopicByName(topic.Name)
                    .done(function(refreshedTopic) {
                        start();
                        equal(refreshedTopic.Description, topic.Description);
                    })
                    .fail(function() {
                        start();
                        ok(false, 'GetTopicByName failed.');
                    });
            })
                .fail(function() {
                    start();
                    ok(false, 'SaveChanges failed.');
                });
        });
    });

});
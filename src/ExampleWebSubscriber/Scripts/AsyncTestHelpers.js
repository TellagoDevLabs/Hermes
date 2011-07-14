///<reference path="jquery.json2xml.js" />
///<reference path="RestClient.js" />
///<reference path="Hermes.js" />

var serviceUrl = 'http://localhost:6156/';

var ensureAtLeastOneGroupExists = function (action) {
    // Yes, atempt to create this group before each test.
    var client = new HermesClient(serviceUrl);
    client.CreateGroup('junk', '').done(action).fail(action);
    stop();
};

var whenGetGroupsCompletes = function(action) {
    ensureAtLeastOneGroupExists(function() {
        var client = new HermesClient(serviceUrl);
        var promise = client.GetGroups();

        promise.done(function(groups) {
            action(groups);
        })
            .fail(function() {
                start();
                ok(false, 'call to GetGroups failed.');
            });
    });
};

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
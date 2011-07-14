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
            ok(groups[0] instanceof Group);
        });
    });

    test("GetGroups new groups should have id", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var id = groups[0].getId();
            notEqual(id, null);
            notEqual(id, '');
        });
    });

    test("GetGroups new groups should have name", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var name = groups[0].Name;
            notEqual(name, null);
            notEqual(name, '');
        });
    });

    test("GetGroups new groups should have description", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var description = groups[0].Description;
            notEqual(description, null);
        });
    });

    test("GetGroups new groups should have link to create topic", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var links = groups[0].getLinks();
            ok('Create Topic' in links);
            notEqual(links['Create Topic'], null);
            notEqual(links['Create Topic'], '');
        });
    });

    test("GetGroups new groups should have link to all topics", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var links = groups[0].getLinks();
            ok('All Topics' in links);
            notEqual(links['All Topics'], null);
            notEqual(links['All Topics'], '');
        });
    });

    test("GetGroups new groups should have link to delete", function () {
        whenGetGroupsCompletes(function (groups) {
            start();
            var links = groups[0].getLinks();
            ok('Delete' in links);
            notEqual(links['Delete'], null);
            notEqual(links['Delete'], '');
        });
    });

    test("GetGroups new groups should have link to update", function () {
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
            console.log('Now creating group ' + name);
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

});
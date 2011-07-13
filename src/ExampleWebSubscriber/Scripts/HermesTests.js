///<reference path="jquery.json2xml.js" />
///<reference path="RestClient.js" />
///<reference path="Hermes.js" />
$(document).ready(function () {

    var serviceUrl = 'http://localhost:6156/';

    test("Always passes", function () {
        ok(true, "Passes");
    });

    module("HermesClient");

    test("Call HermesClient constructor without new", function () {
        var hermes = HermesClient(serviceUrl);
        ok(hermes instanceof HermesClient, "The returned value should be an instance of Hermes");
    });

    test("Call HermesClient constructor without serviceUrl", function () {
        raises(function () { HermesClient(); }, "Calling constructor without a serviceUrl should raise an exception.");
    });

    test("Call HermesClient constructor with null serviceUrl", function () {
        raises(function () { HermesClient(null); }, "Calling constructor a null serviceUrl should raise an exception.");
    });

    test("Call HermesClient constructor with empty serviceUrl", function () {
        raises(function () { HermesClient(''); }, "Calling constructor with an empty serviceUrl should raise an exception.");
    });

    module("HermesClient.GetGroups");

    test("Call hermes.GetGroups returns a promise", function () {
        var client = new HermesClient(serviceUrl);
        var actual = client.GetGroups();
        ok(actual != null && 'done' in actual && 'fail' in actual, 'result of client.GetGroups() should have done and fail methods');
    });

    var whenGetGroupsCompletes = function (action) {
        var client = new HermesClient(serviceUrl);
        var promise = client.GetGroups();
        stop();

        promise.done(function (groups) {
            start();
            action(groups);
        })
            .fail(function () {
                start();
                ok(false, 'call to GetGroups failed.');
            });
    };

    test("when hermes.GetGroups completes, it returns groups", function () {
        whenGetGroupsCompletes(function (groups) {
            ok(groups[0] instanceof Group, "groups should be an array of Groups");
        });
    });

    test("GetGroups new groups should have id", function () {
        whenGetGroupsCompletes(function (groups) {
            var id = groups[0].getId();
            notEqual(id, null, 'Group Id should not be null');
            notEqual(id, '', 'Group Id should not be an empty string');
        });
    });

    test("GetGroups new groups should have name", function () {
        whenGetGroupsCompletes(function (groups) {
            var name = groups[0].getName();
            notEqual(name, null, 'Group name should not be null');
            notEqual(name, '', 'Group name should not be an empty string');
        });
    });

    test("GetGroups new groups should have description", function () {
        whenGetGroupsCompletes(function (groups) {
            var description = groups[0].getDescription();
            notEqual(description, null, 'Group description should not be null');
        });
    });

    test("GetGroups new groups should have link to create topic", function () {
        whenGetGroupsCompletes(function (groups) {
            var links = groups[0].getLinks();
            ok('Create Topic' in links);
            notEqual(links['Create Topic'], null);
            notEqual(links['Create Topic'], '');
        });
    });

    test("GetGroups new groups should have link to all topics", function () {
        whenGetGroupsCompletes(function (groups) {
            var links = groups[0].getLinks();
            ok('All Topics' in links);
            notEqual(links['All Topics'], null);
            notEqual(links['All Topics'], '');
        });
    });

    test("GetGroups new groups should have link to delete", function () {
        whenGetGroupsCompletes(function (groups) {
            var links = groups[0].getLinks();
            ok('Delete' in links);
            notEqual(links['Delete'], null);
            notEqual(links['Delete'], '');
        });
    });

    test("GetGroups new groups should have link to update", function () {
        whenGetGroupsCompletes(function (groups) {
            var links = groups[0].getLinks();
            ok('Update' in links);
            notEqual(links['Update'], null);
            notEqual(links['Update'], '');
        });
    });

    module("HermesClient.CreateGroup");

    var removeGroupBeforeTest = function (groupName, action) {
        var client = new HermesClient(serviceUrl);
        stop();
        client.GetGroupByName(groupName)
            .pipe(function (group) {
                if (group != null)
                    return group.Delete();
            })
            .pipe(function () {
                start();
                action();
            }, function () {
                start();
                ok(false, 'Failed to delete the group before running the test.');
            });
    };

    test("hermes.CreateGroup returns a promise", function () {
        var groupName = 'hermes.CreateGroup returns a promise';
        removeGroupBeforeTest(groupName, function () {
            var client = new HermesClient(serviceUrl);
            var createGroupPromise = client.CreateGroup('hermes.CreateGroup returns a promise');
            ok(createGroupPromise != null && 'done' in createGroupPromise && 'fail' in createGroupPromise, 'CreateGroup should return a promise');
        });
    });

    test("hermes.CreateGroup with no name", function () {
        var client = new HermesClient(serviceUrl);
        raises(function () { client.CreateGroup(); }, "Calling CreateGroup with no name should throw an exception");
    });

    test("hermes.CreateGroup with null name", function () {
        var client = new HermesClient(serviceUrl);
        raises(function () { client.CreateGroup(null); }, "Calling CreateGroup with null name should throw an exception");
    });

    test("hermes.CreateGroup with empty name", function () {
        var client = new HermesClient(serviceUrl);
        raises(function () { client.CreateGroup(''); }, "Calling CreateGroup with empty name should throw an exception");
    });

    var whenCreateGroupCompletes = function (name, description, action) {
        removeGroupBeforeTest(name, function () {
            stop();
            var client = new HermesClient(serviceUrl);
            client.CreateGroup(name, description)
                .done(function (group) {
                    start();
                    action(group);
                })
                .fail(function () {
                    start();
                    ok(false, 'Failed to create ' + name + ' group.');
                });
        });
    };

    test("hermes.CreateGroup completes", function () {
        whenCreateGroupCompletes('hermes.CreateGroup completes', '', function (group) {
            ok(true, 'Group created.');
        });
    });

    test("hermes.CreateGroup returns a Group", function () {
        whenCreateGroupCompletes('hermes.CreateGroup returns a Group', '', function (group) {
            ok(group instanceof Group);
        });
    });

});
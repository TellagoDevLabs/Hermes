///<reference path="jquery.json2xml.js" />
///<reference path="RestClient.js" />
///<reference path="Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

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

});
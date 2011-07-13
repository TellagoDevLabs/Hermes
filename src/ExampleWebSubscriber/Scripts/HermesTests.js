///<reference path="Hermes.js" />
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

module("HermesClient.CreateGroup");

test("hermes.CreateGroup returns a promise", function () {
    var client = new HermesClient(serviceUrl);
    var createGroupPromise = client.CreateGroup('junk');
    ok('done' in createGroupPromise && 'fail' in createGroupPromise, 'CreateGroup should return a promise');
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

test("hermes.CreateGroup completes", function () {
    var client = new HermesClient(serviceUrl);
    stop();

    client.CreateGroup('hermes.CreateGroup completes')
        .done(function (group) {
            ok(true, 'It worked!');
            start();
        })
        .fail(function (xhr, status) {
            ok(true, 'It completes, even if it failed.');
            start();
        });
});


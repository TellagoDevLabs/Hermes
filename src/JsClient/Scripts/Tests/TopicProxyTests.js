///<reference path="../jquery.json2xml.js" />
///<reference path="../Rx.js" />
///<reference path="../rx.jQuery.js" />
///<reference path="../RestClient.js" />
///<reference path="../Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

$(document).ready(function () {

    module('Promise as Topic proxy');

    var serviceUrl = 'http://localhost:6156';

    test("Always passes", function () {
        ok(true);
    });

    var assertIsTopicProxy = function (proxy) {
        start();
        ok('Delete' in proxy && $.isFunction(proxy.Delete), 'proxy should implement Delete()');
        ok('PostMessage' in proxy && $.isFunction(proxy.PostMessage), 'proxy should implement PostMessage(data, contentType)');
        ok('PostStringMessage' in proxy && $.isFunction(proxy.PostStringMessage), 'proxy should implement PostStringMessage(message)');
        ok('GetAllMessages' in proxy && $.isFunction(proxy.GetAllMessages), 'proxy should implement GetAllMessages()');
        ok('PollFeed' in proxy && $.isFunction(proxy.PollFeed), 'proxy should implement PollFeed(interval)');
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

    test('group.GetTopic returns Topic proxy', function () {
        var groupName = 'group.GetTopic returns Topic proxy';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var actual = group.GetTopic(topic.getId());
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

    test('topicProxy.Delete works', function () {
        var groupName = 'topicProxy.Delete works';
        var topicName = groupName;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var proxy = group.GetTopicByName(topicName);
            proxy.Delete()
                .done(function () {
                    // GetTopics filters results from GetTopics
                    // and this request is getting cached results
                    // causing the test to fail even when delete works correctly
                    group.GetTopic(topic.getId())
                        .fail(function () {
                            start();
                            ok(true, 'GetTopic(id) should fail if the topic is deleted.');
                        })
                        .done(function () {
                            start();
                            ok(false, 'GetTopic(id) should fail if the topic is deleted.');
                        });

                })
                .fail(function () {
                    start();
                    ok(false, 'group.GetTopicByName or topicProxy.Delete failed.');
                });
        });
    });

    test('topicProxy.PostMessage works', function () {
        var groupName = 'topicProxy.PostMessage works';
        var topicName = groupName;
        var message = 'The armadillo is rolling.';
        var gotMessage = false;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var proxy = group.GetTopicByName(topicName);
            proxy.PostMessage(message, 'text/plain')
                .done(function () {
                    topic.GetAllMessages()
                        .Where(function (msg) { return msg == message; })
                        .Subscribe(function (next) {
                            gotMessage = true;
                            start();
                            ok(true);
                        },
                        function (err) {
                            start();
                            ok(false, 'GetAllMessages observable failed.');
                        },
                        function () {
                            start();
                            ok(gotMessage);
                        });
                })
                .fail(function () {
                    start();
                    ok(false, 'group.GetTopicByName or topicProxy.PostMessage failed.');
                });
        });
    });

    test('topicProxy.PostStringMessage works', function () {
        var groupName = 'topicProxy.PostStringMessage works';
        var topicName = groupName;
        var message = 'The armadillo is rolling.';
        var gotMessage = false;
        usingGroupAndTopic(groupName, topicName, function (group, topic) {
            var proxy = group.GetTopicByName(topicName);
            proxy.PostMessage(message, 'text/plain')
                .done(function () {
                    topic.GetAllMessages()
                        .Where(function (msg) { return msg == message; })
                        .Subscribe(function (next) {
                            gotMessage = true;
                            start();
                            ok(true);
                        },
                        function (err) {
                            start();
                            ok(false, 'GetAllMessages observable failed.');
                        },
                        function () {
                            start();
                            ok(gotMessage);
                        });
                })
                .fail(function () {
                    start();
                    ok(false, 'group.GetTopicByName or topicProxy.PostMessage failed.');
                });
        });
    });

    test('topicProxy.GetAllMessages works', function () {
        var groupName = 'topicProxy.GetAllMessages works';
        var topicName = groupName;
        usingTopicWithMessages(groupName, topicName, function (topic) {
            var client = new HermesClient(serviceUrl);
            var groupProxy = client.TryCreateGroup(groupName);
            var topicProxy = groupProxy.TryCreateTopic(topicName);
            var observableProxy = topicProxy.GetAllMessages();

            observableProxy
                .Where(function (msg) { return true; })
                .Subscribe(function (next) {
                    console.log('Got message: ' + next);
                    gotMessage = true;
                    start();
                    ok(true);
                },
                function (err) {
                    console.log('errored');
                    start();
                    ok(false, 'GetAllMessages observable failed.');
                },
                function () {
                    console.log('completed');
                    start();
                    ok(gotMessage);
                });


        });
    });

    test('topicProxy.PollFeed works', function () {
        var groupName = 'topicProxy.PollFeed works';
        var topicName = groupName;
        usingTopicWithMessages(groupName, topicName, function (topic) {
            var client = new HermesClient(serviceUrl);
            var groupProxy = client.TryCreateGroup(groupName);
            var topicProxy = groupProxy.TryCreateTopic(topicName);
            var observableProxy = topicProxy.PollFeed(500);

            var filterd = observableProxy
                .Where(function (msg) { return true; });

            var subscription = filterd.Subscribe(function (next) {
                console.log('Got message: ' + next);
                gotMessage = true;
                start();
                ok(true);
                subscription.Dispose();
            },
                function (err) {
                    console.log('errored');
                    start();
                    ok(false, 'PollFeed observable failed.');
                    subscription.Dispose();
                },
                function () {
                    console.log('completed');
                    start();
                    ok(gotMessage);
                });
        });
    });


    test('topicProxy.PollFeed can manage multiple subscriptions', function () {
        var groupName = 'topicProxy.PollFeed can manage multiple subscriptions';
        var topicName = groupName;
        var message0 = 'Message 0.';
        var message1 = 'Message 1.';
        usingGroupAndTopic(groupName, topicName, function () {

            var client = new HermesClient(serviceUrl);
            var groupProxy = client.TryCreateGroup(groupName);
            var topicProxy = groupProxy.TryCreateTopic(topicName);
            var observableProxy = topicProxy.PollFeed(500);

            var fail = function () {
                console.log('Test failed');
                start();
                ok(false);
                subscription0.Dispose();
                subscription1.Dispose();
            };

            // Should receive both messages
            var subscription0items = [];
            var subscription0 = observableProxy.Subscribe(
                function (next) {
                    console.log('Subscription 0 got message ' + next);
                    subscription0items.push(next);
                },
                fail,
                fail);

            // Should receive message0, but not message1
            var subscription1items = [];
            var subscription1 = observableProxy.Subscribe(
                function (next) {
                    console.log('Subscription 1 got message ' + next);
                    subscription1items.push(next);
                },
                fail,
                fail);

            topicProxy.done(function (topic) {
                setTimeout(function () {
                    topic.PostStringMessage(message0);
                }, 100);

                setTimeout(function () {
                    subscription1.Dispose();
                }, 1500);

                setTimeout(function () {
                    topic.PostStringMessage(message1);
                }, 2500);
            });

            setTimeout(function () {
                subscription0.Dispose();
                start();
                ok(subscription0items.length == 2);
                ok(subscription0items[0] = message0);
                ok(subscription0items[1] = message1);
                ok(subscription1items.length == 1);
                ok(subscription1items[0] == message0);
            }, 3500);

        });

    });


});
///<reference path="../jquery.json2xml.js" />
///<reference path="../Rx.js" />
///<reference path="../rx.jQuery.js" />
///<reference path="../RestClient.js" />
///<reference path="../Hermes.js" />
///<reference path="AsyncTestHelpers.js" />

$(document).ready(function () {

    module('Topic Tests');

    var serviceUrl = 'http://localhost:6156';

    test("Always passes", function () {
        ok(true);
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
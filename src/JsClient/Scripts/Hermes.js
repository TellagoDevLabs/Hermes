///<reference path="jquery.json2xml.js" />
///<reference path="jquery.jfeed.js" />
///<reference path="RestClient.js" />
///<reference path="rx.js" />

function HermesClient(serviceUrl) {
    if (!(this instanceof HermesClient))
        return new HermesClient(serviceUrl);
    if (serviceUrl == null || serviceUrl == '')
        throw "serviceUrl should not be null or empty";
    
    var restClient = new RestClient(serviceUrl);
    var operations = {
        Groups: "groups",
        Messages: "messages",
        Subscriptions: "subscriptions",
        GetGroup: function (id) {
            return this.Groups + "/" + id;
        }
    };

    var buildGroupFromXml = function(groupElement) {
        var linkElements = groupElement.find('links > link');
        var linkMap = { };
        for (var linkIndex = 0; linkIndex < linkElements.length; linkIndex++) {
            var linkElement = $(linkElements[linkIndex]);
            linkMap[linkElement.attr('rel')] = linkElement.attr('uri');
        };
        var id = groupElement.find("id").text();
        var name = groupElement.find("name").text();
        var description = groupElement.find("description").text();
        return new Group(restClient, id, name, description, linkMap);
    };

    this.GetGroups = function () {
        var deferred = $.Deferred();
        var url = restClient.getUrl(operations.Groups);
        var request = restClient.Get(url);
        request.done(function (data, status, xhr) {
            var groups = [];
            var doc = $(data);
            var groupElements = doc.find("Group");
            for (var groupIndex = 0; groupIndex < groupElements.length; groupIndex++) {
                var groupElement = $(groupElements[groupIndex]);
                groups.push(buildGroupFromXml(groupElement));
            };
            deferred.resolve(groups);
        })
        .fail(function (xhr, status, ex) {
            deferred.reject(xhr, status, ex);
        });
        return deferred.promise();
    };

    this.CreateGroup = function (name, description) {
        if (name == null || name == '')
            throw 'Group name is null or empty';
        if (typeof description == 'undefined' || description == null)
            description = '';

        var group = { name: name, description: description };
        group.xmlns = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade";
        var data = $.json2xml(group, {
            formatOutput: true,
            rootTagName: 'Group',
            nodes: ['name', 'description']
        });

        var url = restClient.getUrl(operations.Groups);
        var action = restClient.Post(url, {}, data);
        var deferred = $.Deferred();

        action
            .fail(function (ex) {
                console.log('Error creating group ' + name + ': ');
                console.log(ex);
                deferred.reject(ex);
            })
            .done(function (data) {
                deferred.resolve(buildGroupFromXml($(data)));
            });

        return deferred.promise();
    };

    this.GetGroupByName = function(name) {
        return this.GetGroups().pipe(function(groups) {
            for (var i = 0; i < groups.length; i++) {
                var group = groups[i];
                if (group.Name == name)
                    return group;
            }
            return null;
        });
    };

    this.GetGroup = function(id) {
        var operation = operations.GetGroup(id);
        var url = restClient.getUrl(operation);
        var deferred = $.Deferred();
        restClient.Get(url)
            .done(function(data) {
                deferred.resolve(buildGroupFromXml($(data)));
            })
            .fail(deferred.reject);
        return deferred.promise();
    };

    this.TryCreateGroup = function (name, description) {
        var deferred = $.Deferred();
        var createGroup = this.CreateGroup;
        this.GetGroupByName(name)
            .done(function (group) {
                if (group != null) {
                    deferred.resolve(group);
                } else {
                    createGroup(name, description)
                        .done(deferred.resolve)
                        .fail(deferred.reject);
                }
            })
            .fail(deferred.reject);

        return deferred.promise();
    };

}

function Group(restClient, id, groupName, groupDescription, linkMap) {
    if (!(this instanceof Group))
        return new Group(groupName, groupDescription);
    if (groupName == null || groupName == '')
        throw new "name should not be null or empty";

    var thisGroup = this;

    var buildTopicFromXml = function (topicElement) {
        var linkElements = topicElement.find('links > link');
        var linkMap = {};
        for (var linkIndex = 0; linkIndex < linkElements.length; linkIndex++) {
            var linkElement = $(linkElements[linkIndex]);
            linkMap[linkElement.attr('rel')] = linkElement.attr('uri');
        };
        var id = topicElement.find("id").text();
        var name = topicElement.find("name").text();
        var description = topicElement.find("description").text();
        return new Topic(restClient, thisGroup, id, name, description, linkMap);
    };

    
    this.getId = function () { return id; };
    this.Name = groupName;
    this.Description = groupDescription;
    this.getLinks = function() {
        var links = { };
        for (var key in linkMap)
            links[key] = linkMap[key];
        return links;
    };

    this.Delete = function () {
        var url = linkMap['Delete'];
        return restClient.Delete(url);
    };

    this.SaveChanges = function () {
        var group = { id: this.getId(), name: this.Name, description: this.Description };
        group.xmlns = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade";
        var data = $.json2xml(group, {
            formatOutput: true,
            rootTagName: 'Group',
            nodes: ['id', 'name', 'description']
        });
        var url = linkMap['Update'];
        return restClient.Put(url, null, data);
    };

    this.GetTopics = function () {
        var deferred = $.Deferred();
        var url = linkMap['All Topics'];
        restClient.Get(url, null)
                    .done(function (data, status, xhr) {
                        var topics = [];
                        var doc = $(data);
                        var topicElements = doc.find("Topic");
                        for (var topicIndex = 0; topicIndex < topicElements.length; topicIndex++) {
                            var topicElement = $(topicElements[topicIndex]);
                            topics.push(buildTopicFromXml(topicElement));
                        };
                        deferred.resolve(topics);
                    })
                    .fail(deferred.reject);
        return deferred.promise();
    };

    this.CreateTopic = function (topicName, topicDescription) {
        var deferred = $.Deferred();
        var url = linkMap['Create Topic'];
        var topic = { name: topicName, description: topicDescription, groupId: this.getId() };
        topic.xmlns = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade";
        var data = $.json2xml(topic, {
            formatOutput: true,
            rootTagName: 'topic',
            nodes: ['name', 'description','groupId']
        });

        restClient.Post(url, null, data)
            .done(function(data, status, xhr) {
                deferred.resolve(buildTopicFromXml($(data)));
            }).fail(deferred.reject);
        return deferred.promise();
    };

    this.GetTopicByName = function (topicName) {
        return this.GetTopics().pipe(function (topics) {
            for (var i = 0; i < topics.length; i++) {
                var topic = topics[i];
                if (topic.Name == topicName)
                    return topic;
            }
            return null;
        });
    };

    this.TryCreateTopic = function (name, description) {
        var deferred = $.Deferred();
        thisGroup.GetTopicByName(name)
            .done(function (topic) {
                if (topic != null) {
                    deferred.resolve(topic);
                } else {
                    thisGroup.CreateTopic(name, description)
                        .done(deferred.resolve)
                        .fail(deferred.reject);
                }
            })
            .fail(deferred.reject);

        return deferred.promise();
    };
    
}

function Topic(restClient, group, id, topicName, topicDescription, linkMap) {
    if (!(this instanceof Topic))
        return new Topic(topicName, topicDescription);
    if (topicName == null || topicName == '')
        throw new "name should not be null or empty";

    var thisTopic = this;
    
    this.getGroup = function () {
        return group;
    };
    
    this.getId = function () {
        return id;
    };

    this.Name = topicName;
    this.Description = topicDescription;
    
    this.getLinks = function () {
        var links = {};
        for (var key in linkMap)
            links[key] = linkMap[key];
        return links;
    };
    
    this.Delete = function () {
        var url = linkMap['Delete'];
        return restClient.Delete(url);
    };

    this.SaveChanges = function () {
        var url = linkMap['Update'];
        var topic = {
            id: thisTopic.getId(),
            name: thisTopic.Name,
            description: thisTopic.Description,
            groupId: thisTopic.getGroup().getId()
        };
        topic.xmlns = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade";
        var data = $.json2xml(topic, {
            formatOutput: true,
            rootTagName: 'topic',
            nodes: ['id', 'name', 'description', 'groupId']
        });
        return restClient.Put(url, null, data);
    };

    this.PostMessage = function (data, contentType) {
        var deferred = $.Deferred();
        var url = linkMap['Post Message'];
        restClient.Post(url, { 'Content-Type': contentType }, data)
            .done(function(data, status, xhr, location) {
                deferred.resolve(location);
            })
            .fail(deferred.reject);
        return deferred.promise();
    };

    this.PostStringMessage = function(message) {
        return thisTopic.PostMessage(message, 'text/plain');
    };

    this.GetFeed = function () {
        var url = linkMap['Current Feed'];
        var deferred = $.Deferred().resolve();
        jQuery.getFeed({
            url: url,
            success: function (feed) {
                console.log(feed);
                deferred.resolve(feed);
            },
            error: deferred.reject
        });
        return deferred.promise();
    };
    
}

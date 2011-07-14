///<reference path="jquery.json2xml.js" />
///<reference path="RestClient.js" />

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
            return Groups + "/" + id;
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
            console.log('Returning ' + groups.length + ' groups.');
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

        console.log('Creating group ' + name);

        var group = { name: name, description: description };
        group.xmlns = "http://schemas.datacontract.org/2004/07/TellagoStudios.Hermes.RestService.Facade";
        var data = $.json2xml(group, {
            formatOutput: true,
            rootTagName: 'Group',
            nodes: ['name', 'description']
        });

        var url = restClient.getUrl(operations.Groups);
        var action = restClient.Post(url, {}, data);
        var result = $.Deferred();

        action
            .fail(function (ex) {
                console.log('Error creating group ' + name + ': ');
                console.log(ex);
                result.reject(ex);
            })
            .done(function (data) {
                console.log('Group ' + name + ' created.');
                result.resolve(buildGroupFromXml($(data)));
            });

        return result.promise();
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

}

function Group(restClient, id, groupName, groupDescription, linkMap) {
    if (!(this instanceof Group))
        return new Group(groupName, groupDescription);
    if (groupName == null || groupName == '')
        throw new "name should not be null or empty";
    
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
        console.log('Deleting ' + groupName + ' group.');
        var url = linkMap['Delete'];
        return restClient.Delete(url);
    };
    
}

function Topic(topicName, topicDescription) {

};

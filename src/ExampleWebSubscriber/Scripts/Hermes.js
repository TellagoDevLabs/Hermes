
function HermesClient(serviceUrl) {
    if (!(this instanceof HermesClient))
        return new HermesClient(serviceUrl);
    if (serviceUrl == null || serviceUrl == '')
        throw "serviceUrl should not be null or empty";

    this.CreateGroup = function (name, description) {
        if (name == null || name == '')
            throw 'Group name is null or empty';

        console.log('Creating group ' + name);

        var template = $('<ignore><Group><name/><description/><parentId/></Group></ignore>');
        $(template).find('name').text(name);
        if (typeof description != 'undefined' && description != null && description != '')
            $(template).find('description').text(description);
        var data = $(template).html();
        var action = restClient.Post(restClient.Operations.Groups, null, data);
        var result = $.Deferred();

        action
            .fail(function (ex) {
                console.log('Error creating group ' + name + ': ');
                console.log(ex);
                result.reject(ex);
            })
            .done(function (data) {
                console.log('Group ' + name + ' created.');
                console.log(data);
                throw "Not implemented: deserializing data in to a group object.";
            });

        return result.promise();
    };
    
    var restClient = new RestClient();
    function RestClient() {

        var makeAbsolute = function (base, relativeUrl) {
            // far from perfect...
            return base + relativeUrl;
        };

        var client = function(operation, method, headers) {
            var url;
            if (operation == null || operation == '') {
                url = serviceUrl;
            } else {
                url = makeAbsolute(serviceUrl, operation);
            }
            return buildRequest(url, method, headers);
        };

        var buildRequest = function (url, method, headers) {
            console.log('Building ' + method + ' request to ' + url);
            console.log(headers);

            return function (data) {
                console.log('Sending ' + method + ' request to ' + url);
                var requestSettings = {
                    type: method,
                    url: url,
                    data: data,
                    contentType: "application/xml",
                    processData: false,
                    headers: headers
                };
                console.log(requestSettings);
                return $.ajax(requestSettings);
            };
        };
        
        this.Operations = {
            Groups: "groups",
            Messages: "messages",
            Subscriptions: "subscriptions",
            GetGroup: function(id) {
                return Groups + "/" + id;
            }
        };
        
        this.Get = function(operation, headers) {
            return client(operation, "GET", headers)(null);
        };

        this.Put = function(operation, headers, data) {
            return client(operation, "PUT", headers)(data);
        };

        this.Post = function (operation, headers, data) {
            var createGroupRequest = client(operation, "POST", headers)(data);
            var promise = $.Deferred();

            createGroupRequest.fail(function (xhr, status) {
                console.log('POST for operation ' + operation + ' failed.');
                console.log(xhr);
                console.log(createGroupRequest.getAllResponseHeaders());
                promise.reject(xhr, status);
            }).done(function (data, status, xhr) {
                console.log('POST for operation ' + operation + ' completed successfully.');
                var location = xhr.getResponseHeader('Location');
                console.log('GET result from ' + location);
                $.get(location).fail(function (xhr, status) {
                    promise.reject(xhr, status);
                }).done(function (data) {
                    promise.resolve(data);
                });
            });

            return promise.promise();
        };

        this.Delete = function(operation, headers, data) {
            return client(operation, "DELETE", headers)(data);
        };
    };
    
    
    


}

function Group(groupName, groupDescription) {
    if (!(this instanceof Group))
        return new Group(groupName, groupDescription);
    if (groupName == null || groupName == '')
        throw new "name should not be null or empty";

    this.TryCreateTopic = function (name, description) {
    };

}

function Topic(topicName, topicDescription) {

};

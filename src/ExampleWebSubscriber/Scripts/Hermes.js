
function HermesClient(serviceUrl) {
    if (!(this instanceof HermesClient))
        return new HermesClient(serviceUrl);
    if (serviceUrl == null || serviceUrl == '')
        throw "serviceUrl should not be null or empty";

    this.CreateGroup = function (name, description) {
        if (name == null || name == '')
            throw 'Group name is null or empty';

        var data = {
            name: name,
            description: description
        };

        var action = restClient.Post(restClient.Operations.Groups, null, data);
        var result = $.Deferred();

        action
            .fail(function(msg) { result.reject(msg); })
            .done(function(data) {
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

        var buildRequest = function(url, method, headers) {
            return function(data) {
                return $.ajax(url, {
                    type: method,
                    data: data,
                    headers: headers
                });
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

            createGroupRequest.error(function(msg) {
                promise.reject(msg);
            }).success(function(data, status, xhr) {
                var location = xhr.getResponseHeader('Location');
                $.get(location).error(function(msg) {
                    promise.reject(msg);
                }).success(function(data) {
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

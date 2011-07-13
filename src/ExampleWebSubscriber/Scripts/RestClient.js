function RestClient(serviceUrl) {
    if (!(this instanceof RestClient))
        return new RestClient(serviceUrl);

    this.getUrl = function (operation) {
        if (operation == null || operation == '')
            return serviceUrl;
        return serviceUrl + operation;
    };

    var makeRequest = function (url, method, headers, data) {
        var settings = {
            type: method,
            url: url,
            data: data,
            dataType: 'xml',
            contentType: 'application/xml'
        };
        return $.ajax(settings);
    };

    this.Get = function(url, headers) {
        return makeRequest(url, 'GET', headers, '');
    };

    this.Put = function (url, headers, data) {
        throw "Not Implemented";
    };

    this.Post = function (url, headers, data) {
        var deferred = $.Deferred();

        makeRequest(url, 'POST', headers, data)
            .done(function(data, status, xhr) {
                var location = xhr.getResponseHeader('Location');
                makeRequest(location, 'GET')
                    .done(deferred.resolve)
                    .fail(deferred.reject);
            })
            .fail(deferred.reject);

        return deferred.promise();
    };

    this.Delete = function (url, headers) {
        return makeRequest(url, 'DELETE', headers);
    };
};
function RestClient(serviceUrl) {
    if (!(this instanceof RestClient))
        return new RestClient(serviceUrl);

    this.getUrl = function (operation) {
        if (operation == null || operation == '')
            return serviceUrl;
        return serviceUrl + operation;
    };

    var hasContentType = function(headers) {
        if (headers == null)
            return false;
        return 'Content-Type' in headers;
    };

    var makeRequest = function (url, method, headers, data) {
        var settings = {
            type: method,
            url: url,
            data: data,
            headers: headers
        };

        if (!hasContentType(headers)) {
            settings['dataType'] = 'xml';
            settings['contentType'] = 'application/xml';
        }

        return $.ajax(settings)
            .fail(function () {
                var args = Array.prototype.slice.call(arguments);
                console.log(settings);
                console.log(args);
            });
    };

    this.Get = function(url, headers) {
        return makeRequest(url, 'GET', headers, '');
    };

    this.Put = function (url, headers, data) {
        return makeRequest(url, 'PUT', headers, data);
    };

    this.Post = function (url, headers, data) {
        var deferred = $.Deferred();

        makeRequest(url, 'POST', headers, data)
            .done(function (data, status, xhr) {
                var location = xhr.getResponseHeader('Location');
                makeRequest(location, 'GET', headers)
                    .done(function (data, status, xhr) {
                        deferred.resolve(data, status, xhr, location);
                    })
                    .fail(deferred.reject);
            })
            .fail(deferred.reject);

        return deferred.promise();
    };

    this.Delete = function (url, headers) {
        return makeRequest(url, 'DELETE', headers);
    };
};
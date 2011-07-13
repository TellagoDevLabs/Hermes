function RestClient(serviceUrl) {
    if (!(this instanceof RestClient))
        return new RestClient(serviceUrl);

    var makeAbsolute = function (base, relativeUrl) {
        if (relativeUrl == null || relativeUrl == '')
            return base;
        return base + relativeUrl;
    };

    var makeRequest = function (operation, method, headers, data) {
        var url = makeAbsolute(serviceUrl, operation);
        var settings = {
            type: method,
            url: url,
            data: data,
            dataType: 'xml',
            contentType: 'application/xml'
        };
        return $.ajax(settings);
    };

    this.Get = function(operation, headers) {
        return makeRequest(operation, 'GET', headers, '');
    };

    this.Put = function (operation, headers, data) {
        throw "Not Implemented";
    };

    this.Post = function (operation, headers, data) {
        throw "Not Implemented";
        //        var url = makeAbsolute(serviceUrl, operation);
        //        var settings = {
        //            type: 'POST',
        //            url: url,
        //            data: data,
        //            dataType: 'xml',
        //            contentType: 'application/xml'
        //        };
        //        console.log(settings);
        //        return $.ajax(settings)
        //            .fail(function (xhr, status, errorThrown) {
        //                console.log('POST to ' + operation + ' failed: ' + status);
        //                console.log(xhr.getAllResponseHeaders());
        //            });
    };

    this.Delete = function (operation, headers, data) {
        throw "Not Implemented";
    };
};
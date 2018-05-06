function NetService(url) {
    this.url = url;
}

NetService.prototype.register = function (url) {
    return fetch(this.url + '/api/client/register/', {
        method: "POST",
        body: JSON.stringify(url),
        headers: {
            'Accept': 'application/json, text/plain, */*',
            'Content-Type': 'application/json'
        },
    });
};

NetService.prototype.send = function (data) {
    fetch(this.url + '/api/client/send/', {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
            'Accept': 'application/json, text/plain, */*',
            'Content-Type': 'application/json'
        },
    });
};

window.NetService = NetService;
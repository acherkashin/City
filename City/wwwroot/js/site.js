class NetClient {
    //TODO: Добавить host для элемента, в котором будут выводиться логи
    constructor(options) {
        this.options = options;

        var httpConnection = new signalR.HttpConnection(options.hubUrl);
        this.hubConnection = new signalR.HubConnection(httpConnection);

        this.configureMethods();

        this.hubConnection.start();
    }

    configureMethods() {
        this.hubConnection.on("onRecieve", (packate) => {
            console.dir(packate);
            this.options.onRecieve && this.options.onRecieve(packate);
        });

        this.hubConnection.on("onUpdateOnlineList", (users) => {
            console.dir(users);
            this.options.onUpdateOnlineList && this.options.onUpdateOnlineList(users);
        });

        this.hubConnection.on("onStateChanged", (state) => {
            this.options.onStateChanged && this.options.onStateChanged(state);
        });
    }
}
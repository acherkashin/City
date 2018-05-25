class NetClient {
    constructor(options) {
        this.options = options;

        var httpConnection = new signalR.HttpConnection(options.hubUrl);
        this.hubConnection = new signalR.HubConnection(httpConnection);

        this.configureMethods();

        this.hubConnection.start();

        this.options && this.options.host && this.configurePackagesList();
    }

    configureMethods() {
        this.hubConnection.on("onRecievePackage", (packate) => {
            console.dir(packate);
            this.packagesList && this.packagesList.push(packate);
            this.options.onRecievePackage && this.options.onRecievePackage(packate);
        });

        this.hubConnection.on("onUpdateOnlineList", (users) => {
            console.dir(users);
            this.options.onUpdateOnlineList && this.options.onUpdateOnlineList(users);
        });

        this.hubConnection.on("onStateChanged", (state) => {
            this.options.onStateChanged && this.options.onStateChanged(state);
        });
    }

    configurePackagesList() {
        this.packagesList = new Vue({
            el: this.options.host,
            data: {
                packages: []
            },
            methods: {
                update: function() {
                    axios.get("/api/network/packages").then(packages => {
                        this.packages = packages.data;
                    });
                }
            },
            template: `
            <div class="package-list">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Отправитель</th>
                            <th>Получатель</th>
                            <th>Метод</th>
                            <th>Параметры</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="package in packages">
                            <td> {{package.from}} </td>
                            <td> {{package.to}} </td>
                            <td> {{package.method}} </td>
                            <td> {{package.params}} </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            `
        });

        this.packagesList.update();
    }
}
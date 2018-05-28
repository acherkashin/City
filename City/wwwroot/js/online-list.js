class OnlineList {
    constructor(options) {
        this.options = options;
        this.render();

        if (options.host) {
            this.options.host.append(this.$view);    
        }
    }

    render() {
        this.$view = $($.parseHTML(`<table class="online-users table table-striped">
                <thead>
                    <tr>
                        <th>Статус</th>
                        <th>Имя</th>
                        <th>Фамилия</th>
                        <th>Логин</th>
                        <th>Объект</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>`));

        this.renderList();
    }

    renderList() {
        (this.options.users || []).map(user => this.$view.find('tbody').empty().append(this.createRow(user)));
    }

    createRow(user) {
        return $($.parseHTML(`<tr class="online-user">
            <td class="online-user__status online-user__status--${user.isOnline ? "online" : "offline"}" title="${user.isOnline ? "online" : "offline"}"></td>
            <td>${user.firstName}</td>
            <td>${user.lastName}</td>
            <td>${user.login}</td>
            <td>${user.subjectName}</td>
        </tr>`));
    }

    updateUsers(users) {
        this.options.users = users;

        this.renderList();
    }
}
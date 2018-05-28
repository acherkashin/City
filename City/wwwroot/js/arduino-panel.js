Vue.component('arduino-panel', {
    props: ['url'],
    data: function () {
        return {
            arduinoUrl: this.url
        }
    },
    methods: {
        updateUrl: function () {
            axios.put("/api/account/update-arduino-url", { arduinoUrl: this.arduinoUrl })
                .then((resp) => {
                    console.log(resp);
                });
        }
    },
    template: `<div class="arduino-panel">
                    <label for="arduino-url">URL-адрес макета:</label>
                    <input id="arduino-url" type="url" v-model="arduinoUrl"/>
                    <button v-on:click="updateUrl" class="btn btn-primary">Обновить</button>
                </div>`
})
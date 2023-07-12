import { createApp } from "vue";
import "./style.css";
import App from "./App.vue";
import { HubConnectionBuilder, HttpTransportType } from "@microsoft/signalr";

const app = createApp(App);
app.config.globalProperties.$chatHub = new HubConnectionBuilder()
  .withUrl("https://localhost:7203/chat", {
    skipNegotiation: true,
    transport: HttpTransportType.WebSockets,
  })
  .withAutomaticReconnect()
  .build();

app.mount("#app");

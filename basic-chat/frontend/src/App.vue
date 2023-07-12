<script lang="ts">
export default {
  data(): {
    selectedGroupName: string | null;
    groups: Array<string>;
    myGroups: Array<string>;
    message: string;
    messages: Array<{ message: string; user: string }>;
    connected: boolean;
    username: string;
    groupNameBox: string;
  } {
    return {
      selectedGroupName: null,
      groups: [],
      myGroups: [],
      message: "",
      messages: [],
      connected: false,
      username: "",
      groupNameBox: "",
    };
  },
  mounted() {
    this.$chatHub.start();

    this.$chatHub.on("GetMessage", (message: string, user: string) => {
      this.messages.push({ message, user });
      const chatbox = this.$refs.chatbox as HTMLDivElement;
      setTimeout(() => {
        chatbox.scrollTo({
          top: chatbox.scrollHeight,
          left: 0,
          behavior: "smooth",
        });
      }, 50);
    });

    this.$chatHub.on("NewGroupAdded", (groupName: string) =>
      this.groups.push(groupName)
    );
    this.$chatHub.on(
      "UserJoinedToGroup",
      (groupName: string, userName: string) =>
        this.messages.push({
          message: `${userName} is connected to ${groupName} group`,
          user: "System",
        })
    );
  },
  watch: {
    connected(newValue, _) {
      if (newValue) {
        this.$chatHub.invoke("ConnectChat", this.username);
      }
    },
  },
  methods: {
    sendMessage(e: KeyboardEvent | MouseEvent) {
      if (this.message !== "") {
        if (e.shiftKey) {
          this.$chatHub.invoke(
            "SendToGroup",
            this.selectedGroupName,
            this.message
          );
        } else {
          this.$chatHub.invoke("SendAll", this.message);
        }

        this.message = "";
      }
    },
    joinGroup() {
      this.$chatHub.invoke("JoinToGroup", this.groupNameBox);
      this.myGroups.push(this.groupNameBox);
      this.groupNameBox = "";
    },
  },
};
</script>

<template>
  <div v-if="connected">
    <div>
      <div class="message-box" ref="chatbox">
        <div v-for="msg in messages" :key="msg.message + msg.user">
          {{ msg.user }} : {{ msg.message }}
        </div>
      </div>
      <input type="text" v-model="message" @keypress.enter="sendMessage" />
      <input type="button" @click="sendMessage" value="Gönder" />
    </div>
    <div>
      <select v-model="selectedGroupName">
        <option v-for="group in groups" :value="group">{{ group }}</option>
      </select>
      <input type="text" v-model="groupNameBox" @keypress.enter="joinGroup" />
    </div>
  </div>
  <div v-else>
    <input
      type="text"
      v-model="username"
      @keypress.enter="
        () => {
          connected = true;
        }
      "
    />
  </div>
</template>

<style scoped>
.message-box {
  height: 300px;
  overflow-y: scroll;
  background-color: gray;
  text-align: start;
}
</style>

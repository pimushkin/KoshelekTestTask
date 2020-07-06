export const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:8080/chat")
    .build();
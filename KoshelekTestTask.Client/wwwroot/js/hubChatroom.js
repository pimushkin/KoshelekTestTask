import { hubConnection } from './hubConnection.js';

hubConnection.on("Send", function (data) {
    let elem = document.createElement("p");
    elem.appendChild(document.createTextNode(data));
    let firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);
});

hubConnection.start();
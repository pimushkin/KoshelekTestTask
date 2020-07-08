import { hubConnection } from './hubConnection.js';

hubConnection.on("Send", function (serialNumber, content, moscowDateTime) {
    let messageInfoElem = document.createElement("b");
    messageInfoElem.appendChild(document.createTextNode('(Serial number: ' + serialNumber + ', time (Moscow): ' + moscowDateTime + ') - '));

    let elem = document.createElement("p");
    elem.appendChild(messageInfoElem);
    elem.appendChild(document.createTextNode(content));

    var firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);
});

hubConnection.start();
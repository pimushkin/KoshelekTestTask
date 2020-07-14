import { hubConnection } from './hubConnection.js';

hubConnection.on("Send", function (serialNumber, text, timeOfSending) {
    let options = {
        year: 'numeric', month: 'numeric', day: 'numeric',
        hour: 'numeric', minute: 'numeric', second: 'numeric',
        hour12: false
    };

    let messageInfoElem = document.createElement("b");
    messageInfoElem.appendChild(document.createTextNode('(Serial number: ' + serialNumber + ', time (UTC+04:00): ' + new Date(timeOfSending).toLocaleString('en-US', options) + ') - '));

    let elem = document.createElement("p");
    elem.appendChild(messageInfoElem);
    elem.appendChild(document.createTextNode(text));

    var firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);
});

hubConnection.start();
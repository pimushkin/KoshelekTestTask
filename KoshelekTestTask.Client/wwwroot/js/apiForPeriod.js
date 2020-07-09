async function GetMessagesForPeriod() {
    const response = await window.fetch("http://localhost:8080/api/message", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const message = await response.json();
        let options = {
            year: 'numeric', month: 'numeric', day: 'numeric',
            hour: 'numeric', minute: 'numeric', second: 'numeric',
            hour12: false
        };
        document.getElementById("chatroom").innerHTML = "";
        message.forEach(item => {
            {
                let messageInfoElem = document.createElement("b");
                messageInfoElem.appendChild(document.createTextNode('(Serial number: ' +
                    item.serialNumber +
                    ', time (UTC+04:00): ' +
                    new Date(item.moscowDateTime).toLocaleString('en-US', options) +
                    ') - '));

                let elem = document.createElement("p");
                elem.appendChild(messageInfoElem);
                elem.appendChild(document.createTextNode(item.content));

                var firstElem = document.getElementById("chatroom").firstChild;
                document.getElementById("chatroom").insertBefore(elem, firstElem);
            }
        });

    }
}

GetMessagesForPeriod();

document.getElementById("updateBtn").addEventListener("click", function (e) {
    GetMessagesForPeriod();
});

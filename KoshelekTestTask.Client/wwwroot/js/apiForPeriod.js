async function GetMessagesForPeriod() {
    var currentTime = new Date();
    currentTime.setHours(currentTime.getHours() + 4);
    var moscowDateIsoEnd = (new Date(currentTime.toLocaleString('en-US', { timeZone: 'Europe/Moscow' }))).toISOString();
    currentTime.setMinutes(currentTime.getMinutes() - 10);
    var moscowDateIsoBeginning = (new Date(currentTime.toLocaleString('en-US', { timeZone: 'Europe/Moscow' }))).toISOString();
    const response = await window.fetch("http://localhost:8080/api/MessageHandler/FindOverPeriodOfTime", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            beginning: moscowDateIsoBeginning,
            end: moscowDateIsoEnd
        })
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

﻿import * as snackbar from './showSnackBar.js';
var i = 1;
async function createUser() {
    try {
        let message = document.getElementById("message").value;
        const response = await window.fetch("http://localhost:8080/api/sendmessage", {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                content: message,
                serialNumber: i
            })
        });
        if (response.ok === true) {
            snackbar.showSuccessSnackBar();
            i++;
        } else {
            snackbar.showFaultSnackBar();
        }
    } catch (e) {
        snackbar.showFaultSnackBar();
    }
}
document.getElementById("sendBtn").addEventListener("click", function (e) {
    createUser();
});
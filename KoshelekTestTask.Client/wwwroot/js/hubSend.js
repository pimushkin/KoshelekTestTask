import { hubConnection } from './hubConnection.js';
import * as snackbar from './showSnackBar.js';

document.getElementById("sendBtn").addEventListener("click", function (e) {
    let message = document.getElementById("message").value;
    hubConnection.invoke("Send", message);
    snackbar.showSnackBar();
});

hubConnection.start();
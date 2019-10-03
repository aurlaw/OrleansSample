"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/todoHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    var encodedMsg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});
connection.on("SubscribeReceived", function(results) {
    console.log("Subscription active", results);
});

connection.start().then(function(){
    document.getElementById("sendButton").disabled = false;
    document.getElementById("subscribeButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.querySelector("#sendButton").addEventListener("click", function (event) {
    var message = document.querySelector("#messageInput").value;
    sendMessage(message);
    clear();
    event.preventDefault();
});
document.querySelector('#messageInput').addEventListener('keypress', function (event) {
    var key = event.which || event.keyCode;
    if (key === 13) { // 13 is enter
      // code for enter
      var message = document.querySelector("#messageInput").value;
      sendMessage(message);
      clear();
      event.preventDefault();
    }
});
document.querySelector("#subscribeButton").addEventListener("click", function (event) {
    connection.invoke("Subscribe").catch(function(err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.addEventListener("DOMContentLoaded", function(){
    // Handler when the DOM is fully loaded
    console.log("Ready");
  });


function sendMessage(message) {
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });

}
function clear() {
    document.querySelector("#messageInput").value = "";
}
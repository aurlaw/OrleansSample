"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/todoHub").build();
var subId = '';
//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
document.getElementById("clearButton").disabled = true;
document.getElementById("subscribeButton").disabled = true;

connection.on("TodoAdded", function (todo) {
    appendTodo(todo);
});
connection.on("TodosCleared", function() {
    clearTodos();
});
connection.on("SubscribeReceived", function(results) {
    console.log("Subscription active", results);

    subId = results.handleId;
    localStorage.setItem("subscription", subId);
    clearTodos();
    results.result.forEach(todo => {
        appendTodo(todo);
    });
    clearLog();
    appendLog('Starting Log...');

});
connection.on("Notification", function(record) {
    console.log(record);
    appendLog(record);
});
connection.on("SubscriptionCleared", function() {
    clearLog();
});

connection.start().then(function(){
    document.getElementById("sendButton").disabled = false;
    document.getElementById("clearButton").disabled = false;
    document.getElementById("subscribeButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.querySelector("#sendButton").addEventListener("click", function (event) {
    var message = document.querySelector("#name").value;
    addTodo(message);
    clear();
    event.preventDefault();
});
document.querySelector('#name').addEventListener('keypress', function (event) {
    var key = event.which || event.keyCode;
    if (key === 13) { // 13 is enter
      // code for enter
      var message = document.querySelector("#name").value;
      addTodo(message);
      clear();
      event.preventDefault();
    }
});
document.querySelector("#clearButton").addEventListener("click", function(event) {
    connection.invoke("ClearTodos").catch(function(err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
document.querySelector("#subscribeButton").addEventListener("click", function (event) {
    if(subId !== '' ) {
        subId = localStorage.getItem("subscription");
    }
    connection.invoke("Subscribe", subId ).catch(function(err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.addEventListener("DOMContentLoaded", function(){
    // Handler when the DOM is fully loaded
    console.log("Ready");
  });


function addTodo(message) {
    connection.invoke("AddTodo", message).catch(function (err) {
        return console.error(err.toString());
    });

}
function clear() {
    document.querySelector("#name").value = "";
}

function clearTodos() {
    var list = document.getElementById("messagesList");
    // console.log(list, list.firstChild);
    while(list.firstChild) {
        list.removeChild(list.firstChild);

    }
}

function appendTodo(todo) {
    var li = document.createElement("li");
    li.textContent = todo.key + "-" + todo.title;
    document.getElementById("messagesList").appendChild(li);
}

function clearLog() {
    var log = document.getElementById('log');
    while(log.firstChild) {
        log.removeChild(log.firstChild);

    }
}
function appendLog(record) {
    var div = document.createElement("div");
    div.textContent = record;
    document.getElementById("log").appendChild(div);
}
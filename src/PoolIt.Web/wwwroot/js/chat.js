var currentUserId = $('#currentUserId').val();
var conversationId = $('#conversationId').val();

window.scrollTo(0, document.body.scrollHeight);

var sound = new Audio('/misc/pling.mp3');

var connection =
    new signalR.HubConnectionBuilder()
        .withUrl('/conversationsHub')
        .build();

connection.on('MessageReceived',
    function (message) {
        $('#messages').append(
            `<li class='chat-${(message.authorId === currentUserId ? 'user' : 'other')}'>
                <small class='text-muted px-2'>${message.authorName}, ${message.sentOn.replace('T', ' ').split('.')[0]}</small>
                <div>${message.content}</div>
            </li>`);

        window.scrollTo(0, document.body.scrollHeight);

        if (message.authorId !== currentUserId) {
            sound.play();
        }
    });

connection.start()
    .then(function () {
        connection.invoke('Setup', conversationId);
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

document.getElementById('messageInput').addEventListener('keydown', function (e) {
    if (e.keyCode === 13) {
        sendMessage();
    }
});

function sendMessage() {
    var messageInput = $('#messageInput');
    var message = messageInput.val();

    if (message.length < 1) {
        return;
    }

    connection.invoke('SendMessage', message, conversationId);
    messageInput.val('');
}
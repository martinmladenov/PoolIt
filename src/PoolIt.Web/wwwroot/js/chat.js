var currentUserId = $('#currentUserId').val();
var conversationId = $('#conversationId').val();

window.scrollTo(0, document.body.scrollHeight);

var sound = new Audio('/misc/pling.mp3');

var connection =
    new signalR.HubConnectionBuilder()
        .withUrl('/conversationsHub')
        .build();

var lastUser;
var lastTime;

connection.on('MessageReceived',
    function (message) {
        var messageHtml = `<li class='chat-${(message.authorId === currentUserId ? 'user' : 'other')}'>`;

        var newTime = moment.utc(message.sentOn);

        if (message.authorId !== lastUser || lastTime == null || lastTime.diff(newTime, 'minutes')) {
            lastUser = message.authorId;
            messageHtml += `<small class='text-muted px-2'>${message.authorName}, ${getFriendlyDate(moment.utc(message.sentOn).local())}</small>`;
        }

        lastTime = newTime;

        messageHtml += `<div>${message.content}</div></li>`;

        $(messageHtml).hide().appendTo('#messages').fadeIn(350);

        window.scrollTo({
            top: document.body.scrollHeight,
            behavior: 'smooth'
        });

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
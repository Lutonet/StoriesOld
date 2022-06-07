/* Signal R Scripts */


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/indexhub")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect([2000, 4000, 6000, 10000, 20000, 30000, 30000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 600000, 600000, 600000, 600000, 600000, 600000])
    .build();

var connectedVisitors = 0;
var connectedAuthors = 0;
var connectedFriends = 0;
var friendRequests = 0;

connection.start().then(function () {
    console.info("Connected");
    connection.invoke("LoadFriendsPage").catch(function (err) {
        return console.error(err.toString());
    });
    /* We register all online users so we can make statistics */
    connection.invoke("RegisterUser", "@Model.UserIp", "@Model.UserClientAgent").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("RequestOnlineUsers").catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});
/* User Tracking */

/* Signal-R Received list of connected users  Modified 01/07/2021 */

connection.on("connectedUsers", (final, annonymousCount, registeredCount, friendsCount, FriendsJson, friendsRequestsCount) => {
    var friendIds;
    var leftHtml = "";
    if (FriendsJson != "") {
        friendIds = JSON.parse(FriendsJson);
    }
    connectedVisitors = annonymousCount;
    connectedAuthors = registeredCount;
    connectedFriends = friendsCount;
    friendRequests = friendsRequestsCount;
    leftHtml += "<b>Visitors Online: <span id='a-count'>" + annonymousCount + "</span></b></br>";
    leftHtml += "<b>Authors Online: <span id='v-count'>" + registeredCount + "</span></b></br>";
    leftHtml += "<b>Friends Online: <span id='f-count'>" + friendsCount + "</span></b></br>";
    

    leftHtml += "<hr style='width: 100%'>";

    $("#left").html(leftHtml);
    var userList = JSON.parse(final);
    var userCount = userList.length;
    console.info("userCount= " + userCount);
    if (friendsCount > 0) {
        $("#countFriends").html(friendsCount);
        $("#countFriends").show(300);
    } else {
        $("#countFriends").hide(300);
    }

    if (friendRequests > 0) {
        $("#countFriendRequests").html(friendRequests);
        $("#countFriendRequests").show(300);
    } else {
        $("#countFriendRequests").hide(300);
    }

    for (cnt = 0; cnt < userCount; cnt++) {
        console.info(userList);
        console.info(final);
        console.info(cnt);
        var userHtml = "<div id='left_" + userList[cnt].UserId + "' style='display: none; margin: 7px;'>";
        userHtml += "<img style='margin-right: 10px;'src='/images/" + userList[cnt].UserId + "/ProfilePhoto_32.png'>";
        userHtml += userList[cnt].DisplayedName;
        userHtml += "</div>";
        $("#left").append(userHtml);
        $("#left_" + userList[cnt].UserId).show(300);

    }
});
connection.on("Refresh", () => {
    connection.invoke("LoadFriendsPage");
})


connection.on("FriendsRequestReceived", () => {
    friendRequests++;
    RefreshFriends();
})

connection.on("FriendConnected", () => {
    connectedFriends++;
    RefreshFriends();
})



connection.on("FriendDisconnected", () => {
    connectedFriend--;
    RefreshFriends();
})

function RefreshFriends() {
    ("#f-count").html(connectedFriends);
    if (connectedFriends == 0) {
        $("#countFriends").hide(300);
    } else {
        $("#countFriends").html(connectedFriends);
        $("#countFriends").show(300);
    }
    if (friendRequests == 0) {
        $("#countFriendRequests").hide(300);
    } else {
        $("#countFriendRequests").html(friendRequests);
        $("#countFriendRequests").show(300);
    }
}


/* Signal-R Author logged in - Created 01/07/2021  */

connection.on("annonymousConnected", () => {
    connectedVisitors = connectedVisitors + 1;
    $("#a-count").html(connectedVisitors);
})

connection.on("userConnected", (userId, userDisplayedName) => {
    connectedAuthors = connectedAuthors + 1;
    $("#v-count").html(connectedAuthors);
    var userHtml = "<div id='left_" + userId + "' style='display: none; margin: 7px;'>";
    userHtml += "<img style='margin-right: 10px;'src='/images/" + userId + "/ProfilePhoto_32.png'>";
    userHtml += userDisplayedName;
    userHtml += "</div>";
    $("#left").append(userHtml);
    $("#left_" + userId).show(300);

});

connection.on("userDisconnected", (userId) => {
    connectedAuthors = connectedAuthors - 1;
    $("#v-count").html(connectedAuthors);
    $("#left_" + userId).hide(300);
    setTimeout(function () {
        $("left_" + userId).remove();
    })
    connection.invoke("RequestOnlineUsers").catch(function (err) {
        return console.error(err.toString());
    });
})

connection.on("anonymousDisconnected", () => {
    connectedVisitors = connectedVisitors - 1;
    $("#a-count").html(connectedVisitors);
})

/*    End of the User tracking */

/* Messaging for Friends */

const connect = new signalR.HubConnectionBuilder()
    .withUrl("/messagehub")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect([2000, 4000, 6000, 10000, 20000, 30000, 30000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 60000, 600000, 600000, 600000, 600000, 600000, 600000])
    .build();

connect.start().then(function () {
    console.info("Connected");
    /* We register all online users so we can make statistics */
}).catch(function (err) {
    return console.error(err.toString());
});
    connect.on("Fill",(unreadCount, allCount, deletedCount) => {
        if (unreadCount > 0) {
            $("#countUnread").html(unreadCount);
            $("#countUnread").show(300);
        }
        else {
            $("#countUnread").hide(300);
        }
        

    });

function ConfirmFriendship(userId, friendId) {
    console.log("Friendship Confirmation trigerred");
    connect.invoke("ConfirmFriendship", userId, friendId).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("RequestOnlineUsers").catch(function (err) {
        return console.error(err.toString());
    });
}

function RemoveRequest(userId, friendId) {
    console.log("Removing declined request");
    connect.invoke("RemoveDeclined", userId, friendId).catch(function (err) {
        return console.error(err.toString());
    });
}

function DeclineFriendship(userId, friendId) {
    if (confirm("Decline Friendship?")) {
        connect.invoke("DeclineFriendship", userId, friendId).catch(function(err) {
            return console.error(err.toString());
        });
    }
}

function unfriend(id, name) {
    console.log("Unfriending ID " + id + " name: " + name);
    var r = confirm("Unfriend " + name + "?");
    if (r == true) {
        console.log("Unfriending" + id);
        connection.invoke("Unfriend", id).catch(function(err) {
            return console.error(err.toString());
        })
        connection.invoke("LoadFriendsPage");
    }
}
connect.on("Refresh", () => {
    connection.invoke("LoadFriendsPage");
})
// On Page load we must apply Theme
function ToggleTheme() {
    $.get({
        url: "/api/ToggleTheme/",
        success: function(result){
            console.log(result);
            document.location.reload();
        }
    })
}

function getPrefix(id) {
    $.get({
        url: "/api/getprefix/" + id, success: function (result) {
            $("#Prefix").html("+" + result);
        }
    })
}

function LoadInfo(infoPage) {
    $("#info").fadeOut(10);
    setTimeout(function () {
        $.ajax({
            type: "GET",
            url: "/Info/" + infoPage,
        }).done(function (msg) {
            $("#info").html(msg);
            $("#info").fadeIn(1500);
        })
    })
}

function typeWrite(textScript, target, useCursor = true, delay = 100) {
    let endOfTheLine = "##";
    let colorSeparator = "#";

    // createSpan("sentence", "sentence", "dummy");
    // createSpan("feature-text", "feature-text", "dummy");

    var typewriter = new Typewriter(document.getElementById(target), {
        loop: false,
        cursor: ""
    });
    let requestList = textScript.split(endOfTheLine);
    for (var i = 0; i < (requestList.length - 1); i++) {
        let splitLine = requestList[i].split(colorSeparator);
        let text = splitLine[0];
        let style = splitLine[1];
        typewriter.typeString("<span class='" + style + "'>" + text + " </span>");
    }
    typewriter.start();
}

function noLeft() {
    $("#left").hide(0);
    $("#preLeft").removeClass("col-md-1");
    $("#preLeft").addClass("col-md-2");
}

function checkDisplayedName(inputName) {
    $.ajax({
        type: "GET",
        url: "/Api/DisplayedNameExists/" + encodeURIComponent(inputName),
    }).done(function (msg) {
        if (msg == "true") {
            $("#nameError").html("This name is already registered by other user.");
            $("#temp").html("<ul><li> This name is already registered by other user.</li></ul>");
        }
        else {
            $("#nameError").html("");
            $("#temp").html("");
        }
    })
}
function checkEmail(inputEmail) {
    $.ajax({
        type: "GET",
        url: "/Api/EmailExists/" + inputEmail,
    }).done(function (msg) {
        if (msg == "true") {
            $("#emailError").html("This Email is already registered by other user.");
            $("#temp1").html("<ul><li> This Email is already registered by other user.</li></ul>");
        }
        else {
            $("#emailError").html("");
            $("#temp1").html("");
        }
    })
}
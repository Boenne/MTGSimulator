function cardZoom(cardDiv) {
    var cardImg = cardDiv.getElementsByTagName("img")[0];
    var cardZoomPreview = $("#cardZoomPreview")[0];
    cardZoomPreview.src = cardImg.src;
    cardZoomPreview.style.visibility = "visible";
    cardZoomPreview.onmousewheel = function() { cardZoomPreview.style.visibility = "hidden"; };
}

function tapCardImage(cardId) {
    var cardImg = $(document.getElementById(cardId));
    if (cardImg.hasClass("rotate90")) {
        cardImg.removeClass("rotate90");
        cardImg.addClass("resetRotation");
    } else {
        cardImg.removeClass("resetRotation");
        cardImg.addClass("rotate90");
    }
}

function addTextToCombatLog(text) {
    var log = document.getElementById("log");
    log.innerHTML += getTimestamp() + text + "</br>";
    log.scrollTop = log.scrollHeight;
}

function getTimestamp() {
    var date = new Date();
    var hours = addAZero(date.getHours());
    var minutes = addAZero(date.getMinutes());
    var seconds = addAZero(date.getSeconds());
    return "[" + hours + ":" + minutes + ":" + seconds + "] ";
}

function addAZero(number) {
    return (number >= 0 && number < 10) ? "0" + number : number;
}

function cardMouseOver(card, otherZ) {
    if (otherZ != undefined) {
        card.style.zIndex = otherZ;
    } else {
        card.style.zIndex = 1;
    }
}

function cardMouseOut(card, otherZ) {
    if (otherZ != undefined) {
        card.style.zIndex = otherZ;
    } else {
        card.style.zIndex = 0;
    }
}
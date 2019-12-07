//following variable and function are defined for slide show
var slideItem = document.getElementsByClassName("slideItem");
var slideNum = 0;
var currentItem = 0;
var auto = false;
var time = true;
var timer;
var image_width = -1;
var image_height = -1;

function setSlidesNumber(size) {
    slideNum = size;
}

//display the current item, and hide all other items
function slideDisplay() {

    var type = window.localStorage.getItem("type");

    if (type == "auto") {
        auto = true;
        hideButton();
        document.getElementById("auto").checked = true;
        window.localStorage.setItem("type", "auto");
    }
    else {
        auto = false;
        showButton()
        document.getElementById("manual").checked = true;
        window.localStorage.setItem("type", "manual");
    }

    for (var i = 0; i < slideNum; i++) {
        slideItem[i].style.display = "none";
    }

    slideItem[currentItem].style.display = "inline";

    if (auto) timer = setTimeout(moveToNext, time * 1000, true);
}

function setTime() {
    var selector = document.getElementById("timeSelector");
    var index = window.localStorage.getItem("time");

    if (index == null) index = selector.selectedIndex;

    selector.selectedIndex = index;
    time = selector.options[selector.selectedIndex].value;
    window.localStorage.setItem("time", index);
}

function chagneTime() {
    var selector = document.getElementById("timeSelector");
    var index = selector.selectedIndex;
    time = selector.options[selector.selectedIndex].value;
    window.localStorage.setItem("time", index);
    window.clearTimeout(timer);
    slideDisplay();
}

//triggered by clicking on previous button, display previous item
//if current item is first, then move to last item
function moveToPrevious() {
    currentItem = currentItem - 1;
    if (currentItem < 0) currentItem = slideNum - 1;

    slideDisplay();
}

//triggered by clicking on next button, display next item
//if current item is last, then move to first item
function moveToNext(isAuto) {
    if (isAuto && !auto) return;
    currentItem = (currentItem + 1) % slideNum;

    slideDisplay();
}

function manualPlay() {
    auto = false;
    showButton()
    window.localStorage.setItem("type", "manual");
    slideDisplay();
}

function autoPlay() {
    auto = true;
    hideButton();
    window.localStorage.setItem("type", "auto");
    slideDisplay();
}

function increaseSize() {
    var img = document.getElementsByClassName("slide-image");

    if (img[0].width * 1.2 < 1200) {
        for (let i = 0, l = img.length; i < l; i++) {
            img[i].width = img[i].width * 1.2;
            img[i].height = img[i].height * 1.2;
        }
    }

}

function decreaseSize() {
    var img = document.getElementsByClassName("slide-image");

    for (let i = 0, l = img.length; i < l; i++) {
        img[i].width = img[i].width / 1.2;
        img[i].height = img[i].height / 1.2;
    }
}

function hideButton() {
    var button1 = document.getElementsByClassName("next");
    var button2 = document.getElementsByClassName("prev");
    var button3 = document.getElementById("increase");
    var button4 = document.getElementById("decrease");
    button1[0].style.display = "none";
    button2[0].style.display = "none";
    button3.style.display = "none";
    button4.style.display = "none";
}

function showButton() {
    var button1 = document.getElementsByClassName("next");
    var button2 = document.getElementsByClassName("prev");
    var button3 = document.getElementById("increase");
    var button4 = document.getElementById("decrease");
    button1[0].style.display = "block";
    button2[0].style.display = "block";
    button3.style.display = "inline";
    button4.style.display = "inline";
}


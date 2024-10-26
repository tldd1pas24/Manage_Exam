
function playAudio(elementId, so_lan_nghe) {
    const audioElement = document.getElementById(elementId);
    audioElement.currentTime = 0;
    audioElement.play();
    if (so_lan_nghe >= 3) {
        audioElement.pause(); // Dừng audio lại nếu sv không muốn nhiều ở lần thứ 3
        audioElement.currentTime = 0; 
    }
}

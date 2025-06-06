window.playRadio = () => {
    const audio = document.getElementById('geronimoAudio');
    audio.play();
};

window.pauseRadio = () => {
    const audio = document.getElementById('geronimoAudio');
    audio.pause();
};

window.setVolume = (volume) => {
    const audio = document.getElementById("geronimoAudio");
    if (audio) audio.volume = parseFloat(volume);
};

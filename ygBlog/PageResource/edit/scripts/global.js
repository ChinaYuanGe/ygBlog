$(function(){
    window.addEventListener('keydown',function(e){
        if(e.ctrlKey && e.keyCode == 83){
            e.preventDefault();
            saveArtContent();
        }
    });
});

//定时
$(function () {
    setInterval(() => {
        if (global_saved == false) {
            saveArtContent(() => {
                bsMKalert('已自动保存内容', 'success', '#alertHolder', 3000);
            }, false);
        }
    }, 60 * 1000);
});

$(function () { initMCE(); });
$(function(){
    window.addEventListener('keydown',function(e){
        if(e.ctrlKey && e.keyCode == 83){
            e.preventDefault();
            saveArtContent();
        }
    });
});

//��ʱ
$(function () {
    setInterval(() => {
        if (global_saved == false) {
            saveArtContent(() => {
                bsMKalert('���Զ���������', 'success', '#alertHolder', 3000);
            }, false);
        }
    }, 60 * 1000);
});

$(function () { initMCE(); });
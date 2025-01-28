$(function(){
    $(window).on('beforeunload',function(){
        if(!global_saved){
            return "当前数据未保存，是否退出本页?";
        }
    });
});
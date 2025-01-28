$(function(){
    $('.art_tag').on('click',function(){
        $(this).remove();
    });
    $('#input_tag').on('keydown',function(e){
        if(e.originalEvent.keyCode == 13){
            InsertATag();
        }
    })
});

function InsertATag(){
    global_saved = false;
    var tagValue = $('#input_tag').val();
    var allTags = tagValue.split(',');

    for(var i=0;i<allTags.length;i++){
        var currentTag = allTags[i];
        var targetColor = '#'+(md5(currentTag).substring(0,6));
        var elem = $('<div class="art_tag">');
        elem.css({"background-color":targetColor,"color":targetColor});
        elem.append('<span>'+currentTag+'</span>');
        elem.on('click',function(){
            $(this).remove();
        })
        $('#tagDisplayer').append(elem);
    }

    
    $('#input_tag').val('');
}
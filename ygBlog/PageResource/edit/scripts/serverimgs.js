function GetServerImages(){
    var holder = $('#serverPicList');

    ajax_post('/api/post/getimagelist',{id:artID},{
        before:()=>{
            holder.empty();
            holder.append('<div class="spinner-border" role="status"><span>从服务器获取信息...</span></div>');
        },
        success:(data)=>{
            holder.empty();

            switch (data["status"]) {
                case 200:
                    var imgs = data['data']['imgs'];
                    for (var i = 0; i < imgs.length; i++) {
                        let thisImg = imgs[i];
                        let picHolder = $("<div class=\"mb-3\">");
                        picHolder.append('<img class="prevServerPic mb-2" src="/post_img/' + imgs[i] + '">');
                        let insertBtn = $('<button class="btn btn-block btn-success mb-1">插入光标位置</button>');
                        let deleteBtn = $('<button class="btn btn-block btn-danger mb-1">删除此图片</button>');
                        insertBtn.click(function () {
                            tinymce.activeEditor.insertContent('<img src="/post_img/' + thisImg + '">');
                            $('#serverPicList').modal('hide');
                        });
                        deleteBtn.click(function () {
                            var modal = mkAutoAlertModal('请求删除', '向服务器请求删除选中图片...', () => {
                                ajax_post('/api/post/deleteimage', { id: artID, pic: thisImg }, {
                                    success: (data) => {
                                        switch (data["status"]) {
                                            case 200:
                                                GetServerImages();
                                                break;
                                            default:
                                                mkAlertModal('删除失败', '请求删除时发生了错误:' + data["data"]['msg']);
                                                break;
                                        }
                                    },
                                    fail: (t) => {
                                        mkAlertModal('删除失败', '请求删除时发生了错误:' + t);
                                    },
                                    always: () => {
                                        dismissModal(modal);
                                    },
                                }, 6000);
                            }, '删除...', 'danger');
                        });
                        picHolder.append(insertBtn);
                        picHolder.append(deleteBtn);
                        holder.append(picHolder);
                        holder.append('<hr/>');
                    }
                    break;
                default:
                    mkAlertModal('获取失败', '信息获取失败:' + data['data']['msg']);
            }
        },
        fail:(t)=>{
            holder.empty();
            mkAlertModal('获取失败',"获取信息时发生错误:" + t);
        },
        always:()=>{

        }
    },6000);
}

$(function(){
    GetServerImages();
});
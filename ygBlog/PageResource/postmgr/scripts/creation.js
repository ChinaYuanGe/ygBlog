function createArtDraft(){
    var modal = mkInputModal('新建草稿','标题',(text)=>{
        ajax_post('/api/post/create',{title:text},{
            before:()=>{
                confirmModal_setProcess(modal);
            },
            success:(data)=>{
                var jData = data;

                switch (jData["status"]) {
                    case 200:
                        window.location.href = "/edit/" + jData['data']['id'];
                        break;
                    default:
                        mkAlertModal('请求错误', '请求时发生错误:' + jData['data']['msg']);
                        break;
                }
            },
            fail:(t)=>{
                mkAlertModal('请求错误',"与服务器通讯时发生错误:"+t);
            },
            always:()=>{
                confirmModal_unsetProcess(modal);
            }
        },6000);
    },'新建','success',false);
}
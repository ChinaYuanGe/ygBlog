function DeleteArt(){
    artID = g_artID;
    var modal = mkConfirmModal('删除确认','确定要删除该文章吗?<br>此操作不可逆!',()=>{
        ajax_post('/api/arts/delete.php',{id:artID},{
            before:()=>{
                confirmModal_setProcess(modal);
            },
            success:(data)=>{
                var jData = JSON.parse(data);
                if(jData['status']=='ok'){
                    mkAlertModal('删除完成','点击确认跳转至主页',()=>{
                        window.location.href = "/";
                    });
                }
                else if(jData['status'] == "fail"){
                    mkAlertModal('删除出错','请求发生错误:'+jData['data']['msg']);
                }
                else{
                    mkAlertModal('删除出错','请求发生错误:服务器发送了无效的响应');
                }
            },
            fail:(t)=>{
                mkAlertModal('删除出错','与服务器通讯时发生错误:'+t);
            },
            always:()=>{
                confirmModal_unsetProcess(modal);
                dismissModal(modal);
            }
        },6000);
    },false,'danger','删除');
}
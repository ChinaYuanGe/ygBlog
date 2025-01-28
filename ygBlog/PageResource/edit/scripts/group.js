$(function(){
    var elem = $('#input_group');
    elem.on('click',function(){
        g_groupSelection = elem.find("option:selected").index();
    });
    elem.on('change',function(){
        global_saved = false;
        let selection = elem.find("option:selected").val();
        if(selection == -1){
            $('#input_group')[0].selectedIndex = g_groupSelection;
            var modal = mkInputModal('输入新分组','分组名称',(t)=>{
                ajax_post('/api/post/create_group',{name:t},{
                    before:()=>{
                        confirmModal_setProcess(modal);
                    },
                    success: (jData) => {
                        switch (jData['status']) {
                            case 200:
                                let newGroupID = jData['data']['id'];
                                let groupName = jData['data']['name'];
                                $('#input_group').append('<option value="' + newGroupID + '">' + groupName + '</option>');
                                $('#input_group')[0].selectedIndex = $('#input_group option').length - 1;
                                dismissModal(modal);
                                break;
                            default:
                                mkAlertModal('请求失败', '请求发生错误:' + jData['data']['msg']);
                                confirmModal_unsetProcess(modal);
                                break;
                        }
                    },
                    fail:(t)=>{
                        mkAlertModal('请求失败','与服务器通讯时发生错误:'+t);
                        confirmModal_unsetProcess(modal);
                    },
                    always:()=>{

                    }
                },6000);
            },"添加",'success',false);
        }
    });
});
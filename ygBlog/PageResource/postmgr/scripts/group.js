function deleteGroup(id) {
    mkConfirmModal("操作确认", "确认删除此分组吗?", () => {
        var modal = mkAutoAlertModal('正在请求', '成功后页面将刷新', () => {
            ajax_post('/api/post/delete_group', { id: id }, {
                before: () => { },
                success: (jData) => {
                    switch (jData["status"]) {
                        case 200:
                            window.location.reload();
                            break;
                        default:
                            mkAlertModal('请求错误', '与服务器通讯时发生错误:' + jData['data']['msg']);
                    }
                },
                fail: (t) => {
                    mkAlertModal('请求错误', '与服务器通讯时发生错误:' + t);
                },
                always: () => {
                    dismissModal(modal);
                }
            }, 6000)
        }, '删除...', 'danger');
    },true,"danger","删除");
}
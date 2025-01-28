function setRemoteConfig(target, value) {
    mkConfirmModal("操作确认", "确认修改" + target + " ?", () => {
        var modal = mkAutoAlertModal('正在请求', '成功后页面将刷新', () => {
            ajax_post('/api/server/setsetting', { s:target, v:value }, {
                before: () => { },
                success: (jData) => {
                    switch (jData["status"]) {
                        case 200:
                            window.location.reload();
                            break;
                        default:
                            mkAlertModal('请求错误', jData['data']['msg']);
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
    }, true, "danger", "提交");
}
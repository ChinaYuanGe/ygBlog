global_saved = true;
global_saving = false;
function saveArtContent(successCallback, showModal = true) {
    if (global_saving) return;
    global_saving = true;
    var infoContent = btoa(encodeURIComponent(tinymce.activeEditor.getContent()));
    var infoGroup = $('#input_group option:selected').val();
    var infoTitle = $('#input_title').val();

    var infoTags = "";

    var tags = $('#tagDisplayer>div>span');
    for (var i = 0; i < tags.length; i++) {
        infoTags += tags[i].innerText + ",";
    }
    infoTags = infoTags.substring(0, infoTags.length - 1);
    var modal;
    var saveFun = () => {
        let showmodal = showModal;
        ajax_post('/api/post/savecontent', { id: artID, title: infoTitle, content: infoContent, group: infoGroup, tags: infoTags }, {
            success: (data) => {
                switch (data['status']) {
                    case 200:
                        if (showmodal) dismissModal(modal);
                        global_saved = true;
                        if (typeof (successCallback) == 'function') successCallback();
                        break;
                    default:
                        mkAlertModal('保存失败', '服务器响应如下:' + data['data']['msg']);
                }
            },
            fail: (t) => {
                mkAlertModal('保存失败(' + t + ')', '请检查您的网络连接是否正常');
            },
            always: () => {
                global_saving = false;
                if (showmodal) dismissModal(modal);
            }
        }, 10000)
    };

    if (showModal) {
        modal = mkAutoAlertModal('保存...', '执行时请勿关闭本页.', saveFun, "保存中", 'success');
    }
    else {
        saveFun();
    }
}

function uploadImage(type, file, method) {
    var formData = new FormData();

    formData.append('id', artID);
    formData.append('type', type);
    formData.append('file', file);
    a = formData;

    fetch("/api/post/upload_image", {
        method: "POST",
        headers: {
            contentType: "multipart/form-data"
        },
        body: formData
    }).then((resp) => {
        return resp.json();
    }).then((data) => {
        switch (data['status']) {
            case 200:
                method.success(data['data']);
                break;
            default:
                console.error('无法上传图片:' + data['data']['msg']);
                method.fail(data['data']['msg']);
                break;

        }
        method.always();
    }).catch((t) => {
        method.error(t);
        method.always();
    });
    /*
    $.ajax({
        type: 'post',
        url: "/api/post/upload_image",
        contentType: false,
        data: formData,
        processData: false,
        timeout: 15000,
        success: (data) => {
            switch (data['status']) {
                case 200:
                    method.success(data['data']);
                    break;
                default:
                    console.error('无法上传图片:' + data['data']['msg']);
                    method.fail(data['data']['msg']);
                    break;

            }
            method.always();
        },
        error: (t) => {
            method.error(t);
            method.always();
        }
    });*/
}

function uploadTitleImage() {
    let input = document.createElement('input');
    $("#for_ios_shit_input").append(input);

    input.setAttribute('type', 'file');
    input.setAttribute('accept', 'image/*');

    input.onchange = () => {
        var modal = mkAutoAlertModal('上传封面', '正在上传图片,请稍后...', () => {
            uploadImage(1, input.files[0], {
                success: (d) => {
                    $('#prev_titleimage').attr('src', d['src']);
                },
                fail: (m) => {
                    mkAlertModal('发生错误！', '上传图片失败：' + m, () => { }, true, '好.', 'danger');
                },
                error: (t) => {
                    mkAlertModal('发生错误！', '上传图片失败：' + t.statusText, () => { }, true, '好.', 'danger');
                },
                always: () => {
                    dismissModal(modal);
                    GetServerImages();
                    input.remove();
                }
            });
        }, '上传...', 'primary');
    }

    input.click();
}

function DeleteArt() {
    var modal = mkConfirmModal('删除确认', '确定要删除该文章吗?<br>此操作不可逆!', () => {
        ajax_post('/api/post/delete', { id: artID }, {
            before: () => {
                confirmModal_setProcess(modal);
            },
            success: (data) => {
                switch (data['status']) {
                    case 200:
                        mkAlertModal('删除完成', '点击确认跳转至主页', () => {
                            window.location.href = "/";
                        });
                        break;
                    default:
                        mkAlertModal('删除出错', '请求发生错误:' + data['data']['msg']);
                        break;
                }
            },
            fail: (t) => {
                mkAlertModal('删除出错', '与服务器通讯时发生错误:' + t);
            },
            always: () => {
                confirmModal_unsetProcess(modal);
                dismissModal(modal);
            }
        }, 6000);
    }, false, 'danger', '删除');
}
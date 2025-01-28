function goAuth(password) {
    let md5pass = md5(password);
    ajax_post("/api/auth", { passmd5: md5pass }, {
        before: () => {
            $('#btn_login').attr("disabled", true);
            $('#passmd5').attr('disabled', true);
        },
        success: (data) => {
            switch (data['status']) {
                case 200:
                    bsMKalert('登录成功', 'success', '#alertPlacer', 3000);
                    setTimeout(() => { window.location.href = "/" }, 500);
                    break;
                case 403:
                    bsMKalert('验证失败', 'danger', '#alertPlacer', 3000);
                    break;
                default:
                    bsMKalert('发生错误:' + data["data"]["msg"], 'danger', '#alertPlacer', 3000);
                    break;
            }
        },
        fail: (t) => {
            bsMKalert('请求失败:' + t, 'danger', '#alertPlacer', 3000);
        },
        always: () => {
            $('#btn_login').removeAttr("disabled");
            $('#passmd5').removeAttr("disabled");
        }
    }, 6000);
}
var m;
function DelComment(id) {
    m = mkAutoAlertModal('请稍后', '正在删除', () => {
        SetCommentStatus(id, -1);
    }, '删除', 'danger');
}

function PassComment(id, white = 0) {
    m = mkAutoAlertModal('请稍后...', "正在请求放行", () => {
        SetCommentStatus(id, 1, white == 1);
    }, '放行', 'success');
}

function SetCommentStatus(id, status, white = false) {
    ajax_post('/api/comment/setstatus', { id: id, status:status, white: white }, w = {
        success: (d) => {
            switch (d['status']) {
                case 200:
                    ReloadComment(0);
                    break;
                default:
                    w.fail(d['data']['msg']);
                    break;
            }
        },
        fail: (t) => {
            mkAlertModal('错误', t, () => { }, true, '好', 'danger');
        },
        always: () => {
            dismissModal(m);
        }
    });
}

function ReloadComment(page) {
    let typeFilter = $("#typeSelection").val();

    ajax_get('/api/comments/get', { postid: -1, page: page, ss: true, h: parseInt(typeFilter) }, w = {
        before: () => {
            $("#comments tbody").empty();
            $('#comments tbody').append('加载中...');
            $('#pagena').empty();
        },
        success: (d) => {
            switch (d['status']) {
                case 201:
                    w.fail("评论功能已禁用");
                    break;
                case 200:
                    let Holder = $('#comments tbody');
                    Holder.empty();
                    //评论内容
                    d['data']['commits'].forEach((x) => {
                        let tr = $('<tr></tr>');
                        tr.append('<td><img width="128" height="128" src="/comment/avator/' + md5(x['email']) + '"></td>');
                        tr.append('<td>' + x['name'] + '</td>');

                        tr.append('<td>' +
                            (parseInt(x['respid']) > 0 ? '@' + x['repname'] + ': ' + fdbase(x['repsrc']) + '<br><br>' : '') +
                            fdbase(x['content']) +
                            '<br><a href="/read/' + x['artid'] + '">[前往现场]</a>' +
                            '</td>');

                        // 鉴别信息
                        tr.append('<td>' + x['email'] + '<br/> ' + x['endpoint'] + '</td>');
                        tr.append('<td>' + x['time'] + '</td>');

                        let btnTd = $('<td></td>');
                        let passBtn = $('<button class="btn btn-success mt-1">展示</button>');
                        passBtn.click(function () {
                            mkConfirmModal('放行确认', '确认展示该评论吗?', () => {
                                PassComment(x['id']);
                            }, true, 'success', '确认');

                        });

                        btnTd.append(passBtn);

                        let whiteBtn = $('<button class="btn btn-warning mt-1">白名单</button>')
                        whiteBtn.click(function () {
                            mkConfirmModal('白名单确认', '确认要放行并将其 Email 加入白名单吗?', () => {
                                PassComment(x['id'], 1);
                            }, true, 'warning', '确认');

                        });
                        btnTd.append(whiteBtn);

                        let delBtn = $('<button class="btn btn-danger mt-1">隐藏</button>');
                        delBtn.click(function () {
                            mkConfirmModal('删除确认', '确认要隐藏该评论吗?', () => {
                                DelComment(x['id']);
                            }, true, 'danger', '确认');
                        });
                        btnTd.append(delBtn);
                        tr.append(btnTd);
                        Holder.append(tr);
                    });
                    let pagenation = $('<div class="pagenation"></div>')
                    let navPagenation = $('<nav aria-label="Page navigation example"></nav>')
                    let ulPagenation = $('<ul class="pagination">');

                    let MaxPage = parseInt(d['data']['maxPage']) - 1;
                    let prevPage = page <= 0 ? 0 : page - 1;
                    let prevBtn = $('<li class="page-item"><button class="page-link">&lt;</button></li>');
                    prevBtn.find('button').click(function () {
                        ReloadComment(prevPage);
                    });
                    ulPagenation.append(prevBtn);
                    let nextPage = page >= MaxPage ? MaxPage : page + 1;
                    ulPagenation.append('<li class="page-item"><button class="page-link" href="#">' + (page + 1) + '/' + (MaxPage + 1) + '</button></li>');
                    let nextBtn = $('<li class="page-item"><button class="page-link">&gt;</button></li>');
                    nextBtn.find('button').click(function () {
                        ReloadComment(nextPage);
                    });
                    ulPagenation.append(nextBtn);
                    navPagenation.append(ulPagenation);
                    pagenation.append(navPagenation);
                    $('#pagena').append(pagenation);
                    break;
                default:
                    w.fail('请求错误:' + data['data']['msg']);
                    break;
            }
        },
        fail: (t) => {
            let holder = $('#comments tbody');
            let retryBtn = $('<button class="btn btn-warning">重试</button>');
            retryBtn.click(function () {
                ReloadComment(page);
            });
            bsMKalert("错误:" + t, 'danger', '#comments tbody', 60000);
            holder.append(retryBtn);
        },
        always: () => {

        }
    });
}
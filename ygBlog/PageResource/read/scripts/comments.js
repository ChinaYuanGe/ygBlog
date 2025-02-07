function PostComment(name, email, rawcontent, rep) {
    var emailhash = email.toLowerCase();
    var content = febase(rawcontent);
    let modal = mkAutoAlertModal('请稍后...', '正在向服务器递交您的评论', () => {
        ajax_post('/api/comment/post', { a: g_artID, n: name, e: emailhash, c: content, r: rep }, w = {
            success: (d) => {
                switch (d['status']) {
                    case 200:
                        confirmModal_unsetProcess(modal);
                        dismissModal(modal);
                        CleanUpInputComment();
                        if (d['data']['checking'] == false) {
                            ReloadComment(0);
                        }
                        else {
                            mkAlertModal('提交完毕', '您的评论需要审核, 所以目前不会展示在评论区内.<br>一旦评论审核通过, 将会有邮件通知你(前提是你输入了正确的电子邮箱)');
                        }
                        break;
                    default:
                        w.fail(d['data']['msg']);
                        break;
                }
            },
            fail: (t) => {
                mkAlertModal('评论提交错误', '错误信息:' + t, () => { }, true, '确认', 'danger');
            },
            always: () => {
                dismissModal(modal);
            }
        });
    }, '提交评论', 'success');
}
function ReloadComment(page) {
    ajax_get('/api/comments/get', { postid: g_artID, page: page }, w = {
        before: () => {
            $("#comments").empty();
        },
        success: (d) => {
            switch (d['status']) {
                case 201:
                    // Do nothing
                    break;
                case 200:
                    let holder = $('#comments');
                    let pagenation = $('<div class="pagenation"></div>')
                    let navPagenation = $('<nav aria-label="Page navigation example"></nav>')
                    let ulPagenation = $('<ul class="pagination">');

                    let MaxPage = parseInt(d['data']['maxPage']) - 1;
                    let count = parseInt(d['data']['count']);
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

                    holder.append(pagenation); //先添加分页

                    //评论内容
                    if (count < 1) {
                        holder.append("<h2 align=\"center\">空空如也~</h2>");
                    }
                    d['data']['commits'].forEach((x) => {
                        let content = fdbase(x['content']);
                        let repID = parseInt(x['respid']);

                        let commitHolder = $('<div class="commentCard">');
                        let commitHead = $('<div class="commentHead">');
                        commitHead.append('<div><img src="/comment/avator/' + x['email'] + '"></div>');
                        let commitTitle = $('<div></div>');
                        commitTitle.append('<div>' + x['name'] + '</div>');

                        let date = x['time'].split(' ')[0].split('-');
                        let time = x['time'].split(' ')[1].split(':');
                        let finalDateTime = `${date[0]}年${date[1]}月${date[2]}日 ${time[0]}时${time[1]}分`;
                        commitTitle.append('<div><small>' + finalDateTime + '&nbsp;</small></div>');

                        let replyBtn = $('<button class="btn btn-outline-success btn-sm">回复</button>');
                        replyBtn.click(function () {
                            SetReply(x['id'], x['name'], content);
                        });
                        commitTitle.find('small').append(replyBtn);
                        commitHead.append(commitTitle);
                        commitHolder.append(commitHead);

                        let commentBody = $('<div class="commentBody">');
                        //回复的正文
                        if (repID > 0) {
                            let repElement = $('<div class="commentRep"></div>');
                            if (x['repsrc'] != null) repElement.text('@ ' + x['repname'] + ':' + fdbase(x['repsrc']));
                            commentBody.append(repElement);
                        }

                        commentBody.append('<div>' + content + '</div>');
                        commitHolder.append(commentBody);

                        holder.append(commitHolder);
                    });
                    break;
                default:
                    w.fail('请求错误:' + data['data']['msg']);
                    break;

            }
        },
        fail: (t) => {
            let holder = $('#comments');
            let retryBtn = $('<button class="btn btn-warning">重试</button>');
            retryBtn.click(function () {
                ReloadComment(page);
            });
            bsMKalert("错误:" + t, 'danger', '#commits', 60000);
            holder.append(retryBtn);
        },
        always: () => {

        }
    });
}
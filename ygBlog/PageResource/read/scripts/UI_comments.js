function UI_PostComment() {
    let name = $('#comment_name').text();
    let email = $('#comment_email').text();
    let rawcontent = $('#comment_content').text();
    let rep = $('#comment_reply').attr('order');

    //检查输入
    if (email.length < 1 || email.indexOf('@') < 0 || email.indexOf('.') < 0) {
        mkAlertModal('输入错误', '请检查你的电子邮件是否正确', () => { }, true, '好', 'danger');
        return;
    }
    if (rawcontent.length < 1) {
        mkAlertModal('输入错误', '请输入正文...', () => { }, true, '好', 'danger');
        return;
    }
    else if (rawcontent.length > 200) {
        mkAlertModal('输入错误', '正文太多了,请删减一些', () => { }, true, '好', 'danger');
        return;
    }

    //保存Cookie
    $.cookie('comment_name', name, { expires: 3650 });
    $.cookie('comment_email', email, { expires: 3650 });
    PostComment(name, email, rawcontent, rep)
}
function CleanUpInputComment() {
    $('#comment_content').text('');
    ClearReply();
}
function SetReply(orderid, name, content) {
    let replyEl = $('#comment_reply');
    replyEl.attr('order', orderid);
    replyEl.removeAttr('hidden');
    replyEl.text('@ ' + name + ":" + content + " (单击取消)");
}
function ClearReply() {
    let replyEl = $('#comment_reply');
    replyEl.attr('order', 0);
    replyEl.attr('hidden', 'true');
    replyEl.text('');
}

function ReloadAvatarImage() {
    let emailhash = md5($('#comment_email').text());
    $('#comment_imgdisplay').attr('src', '/comment/avator/' + emailhash);
}
function ReadCookieData() {
    $('#comment_name').text($.cookie('comment_name'));
    $('#comment_email').text($.cookie('comment_email'));
}
//每次都要做的事情
$(function () {
    ReloadComment(0);
    ReadCookieData();
    ReloadAvatarImage();
    $('#comment_email').on('keyup', function () {
        if (typeof (avaterReloadTimer) != 'undefined') clearTimeout(avaterReloadTimer);
        avaterReloadTimer = setTimeout(function () {
            ReloadAvatarImage();
        }, 2000);

    });
})
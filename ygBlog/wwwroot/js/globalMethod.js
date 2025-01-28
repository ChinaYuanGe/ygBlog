//展示一个警告框
function bsMKalert(text, type, jqueryselector, timeout = 4000) {
	var item = $('<div class="alert alert-' + type + '" role="alert">').text(text);
	$(jqueryselector).append(item);
	setTimeout(function () {
		item.fadeOut(200);
		setTimeout(function () {
			item.alert('close');
		}, 200)
	}, timeout);
}

//可关闭的警告框
function bsMKdisAlert(text, type, jqueryselector, timeout = 120000) {
	var item = $('<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">');
	item.append(text);
	item.append('<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
	$(jqueryselector).append(item);
	if (timeout == undefined) timeout = 2000;
	if (timeout != null)
		setTimeout(function () {
			item.fadeOut(200);
			setTimeout(function () {
				item.alert('close');
			}, 200)
		}, timeout);
}

//展示一个小弹窗
function mkToast(title, text, jqueryselector, timeout = 4000, subtitle = '') {
	var hold = $('<div class="toast fade" style="pointer-events: auto" role="alert" aria-live="assertive" aria-atomic="true">');
	var head = $('<div class="toast-header">');
	head.append('<strong class="mr-auto">' + title + '</strong>');
	head.append('<small>' + subtitle + '</small>');
	head.append('<button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
	hold.append(head);

	var body = $('<div class="toast-body">');
	body.html(text);
	hold.append(body);
	$(jqueryselector).append(hold);
	hold.toast({
		delay: timeout
	});
	hold.toast('show');
	setTimeout(() => {
		hold.remove();
	}, timeout + 500);
}

function mkConfirmModal(title, contents, callback, closeAfterCallback = true, callback_type = 'success', callbackContent = '确认') {
	var holder = $('<div class="modal fade" id="delDraftModal" tabindex="-1" data-backdrop="static" >');
	var dialog = $('<div class="modal-dialog">');
	var content = $('<div class="modal-content">');

	content.append('<div class="modal-header"><h5 class="modal-title">' + title + '</h5></div>');
	content.append('<div class="modal-body"><p>' + contents + '</p></div>');

	var footer = $('<div class="modal-footer"><div>');

	var cancelBtn = $('<button type="button" id="dismissBtn" class="btn btn-secondary">取消</button>');
	cancelBtn.click(() => {
		dismissConfirmModal(holder);
	})

	footer.append(cancelBtn);

	var cbBtn = $('<button type="button" id="callbackBtn" class="btn btn-' + callback_type + '">' + callbackContent + '</button>');

	cbBtn.click(() => {
		callback();
		if (closeAfterCallback) {
			dismissConfirmModal(holder);
		}
	});

	footer.append(cbBtn);

	content.append(footer);

	dialog.append(content);
	holder.append(dialog);
	$('body').append(holder);
	holder.modal('show');
	return holder;
}

function confirmModal_setProcess(modal) {
	modal.find('#callbackBtn').append('<div class="spinner-border spinner-border-sm">');
	modal.find('button').attr('disabled', true);
}

function confirmModal_unsetProcess(modal) {
	modal.find('.spinner-border').remove();
	modal.find('button').removeAttr('disabled');
}

function mkAlertModal(title, contents, callback = () => {}, closeAfterCallback = true, callback_content = "确认", callback_type = 'success') {
	var holder = $('<div class="modal fade" id="delDraftModal" tabindex="-1" data-backdrop="static" >');
	var dialog = $('<div class="modal-dialog">');
	var content = $('<div class="modal-content">');

	content.append('<div class="modal-header"><h5 class="modal-title">' + title + '</h5></div>');
	content.append('<div class="modal-body"><p>' + contents + '</p></div>');

	var footer = $('<div class="modal-footer"><div>');

	var cbBtn = $('<button type="button" id="callbackBtn" class="btn btn-' + callback_type + '">' + callback_content + '</button>');

	cbBtn.click(() => {
		callback();
		if (closeAfterCallback) {
			dismissConfirmModal(holder);
		}
	});

	footer.append(cbBtn);

	content.append(footer);

	dialog.append(content);
	holder.append(dialog);
	$('body').append(holder);
	holder.modal('show');
	return holder;
}

//自动执行任务的AlertModal
function mkAutoAlertModal(title, contents, callback = () => {}, callback_content = "执行任务", callback_type = 'success') {
	var holder = $('<div class="modal fade" tabindex="-1" data-backdrop="static" >');
	var dialog = $('<div class="modal-dialog">');
	var content = $('<div class="modal-content">');

	content.append('<div class="modal-header"><h5 class="modal-title">' + title + '</h5></div>');
	content.append('<div class="modal-body"><p>' + contents + '</p></div>');

	var footer = $('<div class="modal-footer"><div>');

	var cbBtn = $('<button type="button" id="callbackBtn" class="btn btn-' + callback_type + '" disabled>' + callback_content + '<div class="spinner-border spinner-border-sm"></button>');

	footer.append(cbBtn);

	content.append(footer);

	dialog.append(content);
	holder.append(dialog);
	$('body').append(holder);
	holder.modal('show');
	callback();
	return holder;
}

//一个输入框的Modal
//callback传入一个字符串参数
function mkInputModal(title, placeholder, callback = () => {}, callback_content = "确认", callback_type = 'success', closeAfterCallback = true) {
	var holder = $('<div class="modal fade" tabindex="-1" data-backdrop="static" >');
	var dialog = $('<div class="modal-dialog">');
	var content = $('<div class="modal-content">');

	content.append('<div class="modal-header"><h5 class="modal-title">' + title + '</h5></div>');
	content.append('<div class="modal-body"><p><input id="inputs" type="text" class="form-control" placeholder="' + placeholder + '"/></p></div>');

	

	var footer = $('<div class="modal-footer"><div>');

	footer.append('<button class="btn btn-secondary" data-dismiss="modal">取消</button>');

	var cbBtn = $('<button type="button" id="callbackBtn" class="btn btn-' + callback_type + '">' + callback_content + '</button>');

	footer.append(cbBtn);

	content.append(footer);

	dialog.append(content);
	holder.append(dialog);
	$('body').append(holder);
	holder.modal('show');
	cbBtn.click(() => {
		callback(content.find('input#inputs').val());
		if (closeAfterCallback) {
			dismissConfirmModal(holder);
		}
	});
	content.find('#inputs').on('keydown',function(e){
		if(e.originalEvent.keyCode == 13){
			cbBtn.click();
		}
	});

	return holder;
}

function dismissModal(modal_instance) {
	setTimeout(() => {
		modal_instance.modal('hide');
		setTimeout(() => {
			modal_instance.modal('dispose');
		}, 500);
	}, 1000);
}

function dismissConfirmModal(modal_instance) {
	modal_instance.modal('hide');
	setTimeout(() => {
		modal_instance.modal('dispose');
	}, 500);
}

//获取url的参数
function getQueryVariable(variable) {
	var query = window.location.search.substring(1);
	var vars = query.split("&");
	for (var i = 0; i < vars.length; i++) {
		var pair = vars[i].split("=");
		if (pair[0] == variable) {
			return pair[1];
		}
	}
	return (false);
}


function updateUrl(key, value) {
	var newurl = updateQueryStringParameter(key, value)
		//向当前url添加参数，没有历史记录
	window.history.replaceState({
		path: newurl
	}, '', newurl);
}

function updateQueryStringParameter(key, value) {
	var uri = window.location.href
	if (value!='' && !value) {
		return uri;
	}
	var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
	var separator = uri.indexOf('?') !== -1 ? "&" : "?";
	if (uri.match(re)) {
		return uri.replace(re, '$1' + key + "=" + value + '$2');
	} else {
		return uri + separator + key + "=" + value;
	}
}
//RGBA转hash
function getHexColor(color) {
  var values = color
    .replace(/rgba?\(/, '')
    .replace(/\)/, '')
    .replace(/[\s+]/g, '')
    .split(',')
  var a = parseFloat(values[3] || 1),
    r = Math.floor(a * parseInt(values[0]) + (1 - a) * 255),
    g = Math.floor(a * parseInt(values[1]) + (1 - a) * 255),
    b = Math.floor(a * parseInt(values[2]) + (1 - a) * 255)
  return '#' +
    ('0' + r.toString(16)).slice(-2) +
    ('0' + g.toString(16)).slice(-2) +
    ('0' + b.toString(16)).slice(-2)
}
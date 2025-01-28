//发起一次GET
/*
	url->请求的url
	args->参数
	method: -> 一个Object对象
		method.before -> 可选，请求前要做的事情
		method.success(data) -> 必须, 请求成功时要做的事情
			data -> 接收的数据
		method.fail(textStatus) -> 可选, 请求失败时要做的事情
			textStatus -> 文字状态
		method.always -> 可选，请求完成后总是做时事情
	往下的方法大同小异
*/
function ajax_get(url,args,method,timeout = 6000){
	var isComplete = false;
	var isTimeout = false;
	
	if(typeof(url) != 'string'){
		console.error("fastAjax error:mission url");
	}
	
	if(typeof(method) != 'object') {
		console.error("fastAjax error:missing method object");
		return;
	}
	
	if(typeof(method.before) == 'function') method.before();
	
	if(typeof(method.success) != 'function'){
		console.error('fastAjax error:missing success method.');
		return;
	}
	var req = $.get(url,args,(data) => {
		method.success(data);
		if(typeof(method.always) == 'function') method.always();
		isComplete = true;
	}).fail((jxhr,textStatus) => {
		if(isTimeout) return;
		method.fail(textStatus);
		if(typeof(method.always) == 'function') method.always();
		isComplete = true;
	});
	setTimeout(() => {
		if(isComplete) return;
		isTimeout = true;
		req.abort();
		method.fail('timeout');
		if(typeof(method.always) == 'function') method.always();
	},timeout);
}
//发起一次POST
function ajax_post(url,args,method,timeout = 6000){
	var isComplete = false;
	var isTimeout = false;
	
	if(typeof(url) != 'string'){
		console.error("fastAjax error:mission url");
	}
	
	if(typeof(method) != 'object') {
		console.error("fastAjax error:missing method object");
		return;
	}
	
	if(typeof(method.before) == 'function') method.before();
	
	if(typeof(method.success) != 'function'){
		console.error('fastAjax error:missing success method.');
		return;
	}
	var req = $.post(url,args,(data) => {
		method.success(data);
		if(typeof(method.always) == 'function') method.always();
		isComplete = true;
	}).fail((jxhr,textStatus) => {
		if(isTimeout) return;
		method.fail(textStatus);
		if(typeof(method.always) == 'function') method.always();
		isComplete = true;
	});
	setTimeout(() => {
		if(isComplete) return;
		isTimeout = true;
		req.abort();
		method.fail('timeout');
		if(typeof(method.always) == 'function') method.always();
	},timeout);
}
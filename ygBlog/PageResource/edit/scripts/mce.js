function initMCE() {
	tinymce.init({
		selector: 'textarea#mainEditor',
		skin: (window.isNight ? 'oxide-dark' : 'oxide'),
		menubar: false,
		content_css: window.isNight ? '/css/custom/artView.night.css' : '/css/custom/artView.css',
		toolbar: 'save | undo redo | indent2em | alignleft aligncenter alignright | hr | h2 h3 link | bold italic underline strikethrough | bullist numlist forecolor | codesample code | image',
		language: 'zh-Hans',
		resize: false,
		plugins: 'wordcount save lists image codesample code link indent2em',
		codesample_languages: [{
			text: 'HTML/XML',
			value: 'markup'
		}, {
			text: 'JavaScript',
			value: 'javascript'
		}, {
			text: 'CSS',
			value: 'css'
		}, {
			text: 'PHP',
			value: 'php'
		}, {
			text: 'Python',
			value: 'python'
		}, {
			text: 'Java',
			value: 'java'
		}, {
			text: 'C',
			value: 'c'
		}, {
			text: 'C#',
			value: 'csharp'
		}, {
			text: 'C++',
			value: 'cpp'
		}],
		height: '80vh',
		file_picker_types: "image",
		file_picker_callback: (callback, val, meta) => {
			var input = document.createElement('input');
			$("#for_ios_shit_input").append(input);

			input.setAttribute('type', 'file');
			input.setAttribute('accept', 'image/*');

			input.onchange = () => {
				var modal = mkAutoAlertModal('上传图片', '正在上传，请稍后...', () => {
					uploadImage(0, input.files[0], {
						success: (data) => {
							callback(data['src']);
							//getListImage();
							dismissModal(modal);
							GetServerImages();
						},
						fail: (m) => {
							console.log(m);
							mkAlertModal('发生错误！', '上传图片失败:' + m + ' ...请重试', () => { }, true, '好', 'danger');
						},
						error: (t) => {
							console.log(t);
							mkAlertModal('发生错误！', '上传图片失败:' + t + ' ...请重试', () => { }, true, '好', 'danger');
						},
						always: () => {
							dismissModal(modal);
							input.remove();
						}
					});
				}, '上传...', 'primary');
				modal.css({
					"z-index": "10000",
					"background-color": "rgba(0,0,0,.8)"
				});
			}
			input.click();
		},
		a11y_advanced_options: true,
		init_instance_callback: () => {
			$('#load_warm').remove();
			console.log('tinymce inited');
			$('.artBtn:not([noallow])').removeAttr('disabled');
		},
		nonbreaking_force_tab: true,
		save_enablewhendirty: false,
		save_onsavecallback: () => {
			saveArtContent(() => {
				setTimeout(() => {
					tinymce.activeEditor.focus();
				}, 1000);
			});
		},
		setup: (editor) => {
			editor.on('keydown', (e) => {
				global_saved = false;
				if (e.keyCode == 9) { //tinymce>tab -> 制表符
					e.preventDefault();
					editor.execCommand('indent2em');
				}
			});
		}
	});
}
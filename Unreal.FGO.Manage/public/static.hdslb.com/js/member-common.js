/*! 2013 bilibili.tv */
(function($,e,b){var c="hashchange",h=document,f,g=$.event.special,i=h.documentMode,d="on"+c in e&&(i===b||i>7);function a(j){j=j||location.href;return"#"+j.replace(/^[^#]*#?(.*)$/,"$1")}$.fn[c]=function(j){return j?this.bind(c,j):this.trigger(c)};$.fn[c].delay=50;g[c]=$.extend(g[c],{setup:function(){if(d){return false}$(f.start)},teardown:function(){if(d){return false}$(f.stop)}});f=(function(){var j={},p,m=a(),k=function(q){return q},l=k,o=k;j.start=function(){p||n()};j.stop=function(){p&&clearTimeout(p);p=b};function n(){var r=a(),q=o(m);if(r!==m){l(m=r,q);$(e).trigger(c)}else{if(q!==m){location.href=location.href.replace(/#.*/,"")+q}}p=setTimeout(n,$.fn[c].delay)}(navigator.appName === 'Microsoft Internet Explorer')&&!d&&(function(){var q,r;j.start=function(){if(!q){r=$.fn[c].src;r=r&&r+a();q=$('<iframe tabindex="-1" title="empty"/>').hide().one("load",function(){r||l(a());n()}).attr("src",r||"javascript:0").insertAfter("body")[0].contentWindow;h.onpropertychange=function(){try{if(event.propertyName==="title"){q.document.title=h.title}}catch(s){}}}};j.stop=k;o=function(){return a(q.location.href)};l=function(v,s){var u=q.document,t=$.fn[c].domain;if(v!==s){u.title=h.title;u.open();t&&u.write('<script>document.domain="'+t+'"<\/script>');u.close();q.location.hash=v}}})();return j})()})(jQuery,this);

function zeroFix(n){
	if(n<10) return "0"+n;
	else return n;
}

var Request = {
	get: function(key, str) {
		var url = window.location.href;
		if(str) {
			url = str;
		}
		if(!url.match(/\?.*/)) {
			return false;
		}
		var qs = url.substring(url.search(/\?/) + 1, url.length);
		if(!key) {
			return qs;
		}
		var params = qs.split("&");
		return this.parse(params, key);
	},
	hash: function(key, str) {
		var hash = window.location.hash.substring(1).split("?")[0];
		if(str) {
			hash = str;
			if(hash.charAt(0) == '#') {
				hash = hash.substring(1);
			}
		}
		if(!key) {
			return hash;
		}
		var params = hash.split("&");
		return this.parse(params, key, true);
	},
	root: function(str) {
		var hash = window.location.hash.substr(1);
		if(typeof str != "undefined") {
			hash = str.replace("#","");
		}
		return hash.split("?")[0].split("&")[0];
	},
	parse: function(str, key, byHash) {
		var paraObj = {};
		for (var i = 0; j = str[i]; i++) {
			if(!j.match(/=/) && byHash) {
				paraObj[j.toLowerCase()] = j;
			} else {
				paraObj[j.substring(0, j.search(/=/)).toLowerCase()] = j.substring(j.search(/=/) + 1, j.length);
			}
		}
		var returnValue = paraObj[key.toLowerCase()];
		if (typeof(returnValue) == "undefined") {
			return false;
		} else {
			return returnValue;
		}
	}
};
window.utils.HashManage.prependHash = '';

/*tab绑定*/
function TabManager(options){
	var self = options.obj;
	var id = "#"+self.attr("id");
	var tabParent = self.parent();
	if(typeof(options.page)!="undefined"){
		var page = $(id+options.page);
		var pageParent = page.parent();
		var pages = $(">*",pageParent);
	}
	var callback = self.attr("callback");
	var url = self.attr("url");
	if(self.hasClass("on")){
		if(options.needReload||!self.attr("loaded")){
			if (typeof(callback)!="undefined"){
				eval(callback);
				self.attr("loaded","1");
			}
		}
		return;
	}
	if(typeof(options.iconChange)!="undefined"){
		options.iconChange(self,tabParent);
	}
	$(">.on",tabParent).removeClass("on");
	self.addClass("on");
	var callback_done = false;
	if(typeof(options.switchPage)=="function"){
		callback_done = options.switchPage(page,pageParent,callback,url);
	} else{
		if(typeof(options.page)!="undefined"){
			$(">*",pageParent).hide();
			page.fadeIn(200);
		}
	}
	if (typeof(callback)!="undefined"&&!callback_done&&!self.attr("loaded")){
		eval(callback);
		self.attr("loaded","1");
	}
}

function switchTab(obj,needReload,hashchange){
	if(obj.hasClass("on")&&hashchange||!obj.attr("id")) return;
	var switchPage;
	if(typeof(obj.attr("url"))!="undefined"){
		switchPage = function(page,pageParent,callback,url){
			$.ajax(url, {
				success:function(data)
				{
					$(".main-content-page",pageParent).hide();
					page.html(data);
					page.fadeIn(200);
					if (typeof(callback)!="undefined"){
						eval(callback);
					}
				}
			});
			return true;
		}
	} else{
		switchPage = null;
	}
	TabManager({
		obj:obj,
		needReload:needReload,
		page:"_content",
		switchPage:switchPage
	});
}

function switchContentTab(obj,needReload)
{
	TabManager({
		obj:obj,
		needReload:needReload,
		page:"_content"
	});
}

function switchType(obj)
{
	TabManager({
		obj:obj
	});
}

function switchControlContentTab(obj)
{	
	TabManager({
		obj:obj,
		page:"_page"
	});
}

function switchSubTab(obj)
{
	TabManager({
		obj:obj,
		page:"-page"
	});
}

function switchSubmitTab(obj)
{
	TabManager({
		obj:obj,
		page:"_page",
		iconChange:function(tab,tabParent){
			$(">.on i",tabParent).addClass("gray");
			$(">.on b",tabParent).remove();
			$(">i",tab).removeClass("gray");
			$('<b class="icon-arrow-bottom"></b>').appendTo(tab);
		}
	});
}

function typeSelection(obj,mode){
	if (obj.hasClass("on")) return;
	var parent = obj.parent();
	var listParent = parent.parent();
	$("li.on",parent).removeClass('on');
	obj.addClass("on");
	if(mode=="dyn"){
		var type = obj.attr("type");
		if(type=="all") 
		{ 
			listParent.find(".rencently-list li").show(); 
		}
		else{
			listParent.find(".rencently-list li").hide();
			listParent.find(".rencently-list li."+type).fadeIn(200);
		}
	}
	else if(mode=="stow"){

	}
}

function multiSelect(obj){
	var parent = obj.parent();
	if(!obj.hasClass("on") && $(".on",parent).length>5){
		new MessageBox().showEx(obj,"最多6个风格", 500, "error");
		return false;
	}
	else{
		obj.toggleClass("on");
	}
}

/*小菜单定时关闭事件绑定*/
function closeMenuTimer(menu){
    var timer = setTimeout(function(){
        menu.fadeOut(300,_remove);
    },3000);
    function _bodyClick(){
    	clearTimeout(timer);
        menu.fadeOut(300,_remove);
    }
    function _remove(){
    	$("body").off("click",_bodyClick);
    	menu.remove();
    }
    menu.hover(
        function(){
            clearTimeout(timer);
        },
        function(){
            timer = setTimeout(function(){
                menu.fadeOut(300,_remove);
            },3000);
        }
    );
    $("body").on("click",_bodyClick);
}

/*私信悬浮窗*/
function pm_dialog(){
	var mask = $('<div class="wnd-mask"></div>').appendTo("body");;
	var dlg=$('<div class="pm-wrapper"></div>').appendTo("body");
	var dlgbox = $('<form action="/message.do" method="post"><div class="send-message" id="wnd_msg">'+
	'<div class="title"><i class="icon-halfblock"></i>私信<div class="close"><i class="icon-delete"></i></div></div>'+
	'<div class="cnt">'+
	'<p><span class="txt">对话人：</span><input type="text" id="receiver_id" class="control-inpt-input" style="width:120px;" /><span class="notice">请输入对方的昵称</span></p>'+
	'<p><span class="txt">对话内容：</span><textarea id="msg" class="control-inpt-input"></textarea></p>'+
	'<div class="btn left">发送</div>'+
	'<a href="javascript:void(0);" class="cancel">取消</a>'+
	'</div></div></form>').appendTo(dlg);
	$(".pm-wrapper .cancel,.pm-wrapper .close").click(function(){
		mask.remove();	
		dlg.remove();
	});
	$(".pm-wrapper .btn").click(function(){
		$.ajax({
			type:"POST",
			dataType:"json",
			url:"/message.do",
			data:{
				id:$(".pm-wrapper #receiver_id").val(),
				msg:$(".pm-wrapper #msg").val()
			},
			success:function(data){
				if (data.code == 0)
				{
					new FloatMessageBox().show("message","default","#message");
				}else
				{
					new FloatMessageBox().show(data.msg, 'custom_err');
				}
				mask.remove();
				dlg.remove();
			},
			error:function(data)
			{
				new FloatMessageBox().show('网络错误', 'custom_err');
				mask.remove();
				dlg.remove();
			}
		});		
	});
	dlg.stop().fadeIn(100);
}

/*消息窗*/
function FloatMessageBox(){}

FloatMessageBox.prototype = {
	wnd:null,
	result:"",
	btnText:"[ 点击返回 ]",
	show:function(type,mode,url,timeout){
		$(".container-grid-layout .page-loading").remove();
		$(".wnd-mask").remove();
		$(".messagebox").remove();
		var fmb=this;
		var target = $("body");
		var result_pic="";
		if(mode=="custom_err"||mode=="msg_err"||mode=="error"){ result_pic=" error";fmb.result="error"; }
		else {result_pic=" success";fmb.result="success"}
		var mask = $('<div class="wnd-mask"></div>');
		fmb.wnd = $('<div class="messagebox">'+
						   '<div class="content">'+
						   		'<div class="title">消息框</div>'+
								'<div class="info clearfix">'+
									'<div class="msg-icon'+result_pic+'"></div>'+
									'<div class="text"></div>'+
								'</div>'+
								'<a class="a-btn">'+fmb.btnText+'</a>'+
								'</div></div>');

		fmb.init(type);
		$("a.a-btn",fmb.wnd).click(function(){
			if(typeof(url)!=="undefined"){
				if (url == 'javascript:history.go(-1);'){
					history.go(-1);
				}else if(url=="reload"){
					location.reload();
				} else{
					location.href=url;
				}
			} else if(fmb.result!="error"&&mode!="close"){
				location.reload();
			}
			fmb.close();
		});
		$("a",fmb.wnd).click(function(){
			if($(this).attr("reload")){
				location.reload();
			}
			mask.remove();
			fmb.wnd.remove();
		});
		mask.appendTo(target);
		fmb.startup_date = new Date();
		fmb.wnd.appendTo(target);
		fmb.wnd.css("margin-left",-fmb.wnd.outerWidth()/2);
		window._active_fmb = fmb;
		fmb.close = function()
		{
			mask.remove();
			fmb.wnd.remove();
			window._active_fmb = null;
		};
		/*setTimeout(function(){
			$("a",msg).trigger("click");
		},2000);*/
	},
	init:function(type){
		var fmb = this;
		var text = fmb.wnd.find(".text");
		switch(type){
			case "submit_video":
				text.html("投稿成功<br />查看投稿视频状态，<br />请进入——<a href='#video_manage'>「视频管理」</a>页面");
				if(window.localStorage){
					window.localStorage.setItem("submit_info","null");
					clearInterval(window.rememberTimer);
				}
				break;
			case "edit_video":
				text.html("编辑成功<br />查看视频状态，<br />请进入——<a href='#video_manage'>「视频管理」</a>页面");
				break;
			case "submit_special":
				text.html("创建成功<br />查看创建专题状态，<br />请进入——<a href='#special_manage'>「专题管理」</a>页面");
				break;
			case "edit_special":
				text.html("编辑成功<br />查看创建专题状态，<br />请进入——<a href='#special_manage' reload=1>「专题管理」</a>页面");
				break;
			case "buy":
				text.html("购买成功！");
				break;
			case "save":
				text.html("保存成功！");
				break;
			case "exchange":
				text.html("兑换成功！");
				break;
			case "message": case "message_firend":
				text.html("发送成功！<br />你可以进入消息中心<br /><a href='#message'>「我的私信」</a>栏目中<br />查看对话记录");
				break;
			default:
				text.html(type);
				break;
		}
	},
	setBtnText:function(text){
		this.btnText = text;
	}
};

/*图片上传*/
function pic_upload(mode){
	var mask = $('<div class="wnd-mask"></div>').appendTo("body");
	var swf = "";
	var action = "/upload_api.do?src=web&mode="+mode;
	var size_tips;
	var target_url = encodeURIComponent("/upload_api.do?src=flash&mode="+mode);
	if($("#pic_mode").length){
		$("#pic_mode").val(mode);
	}
	if(mode=="face"){
		swf = "MemberImageUploader.swf";
		size_tips = "请选择jpg,png类图片上传：大小180x180像素";
	} else if(mode=="sp"){
		swf = "SpImageUploader.swf";
		size_tips = "请选择jpg,png类图片上传：大小128x128像素";
	} else if(mode=="sp_index"){
		swf = "SpIndexImageUploader.swf";
		size_tips = "请选择jpg,png类图片上传：大小240x320像素";
	} else{
		swf = "VideoImageUploader.swf";
		size_tips = "请选择jpg,png类图片上传：大小240x150像素";
	}
	var image_url = "";
	if(mode!="sp_index"){
		image_url = !$("#imghead").attr("src")?"":"&image_url="+$("#imghead").attr("src");
	} else{
		image_url = !$("#imghead_index").attr("src")?"":"&image_url="+$("#imghead_index").attr("src");
	}
	var dlg = $('<div class="pic-dialog">'+
		'<iframe name="hiddenFrame" width="0" height="0" style="display:none"></iframe>'+
		'<form enctype="multipart/form-data" method="POST" target="hiddenFrame">'+
		'<div class="title"><div class="left"><i class="icon-halfblock"></i>上传封面</div><div class="right"><a class="close">关闭</a></div></div>'+ 
		/*'<div class="cnt pic-f"><p>无法上传图片？尝试 <a id="pic_n">普通模式</a></p>'+*/
		'<div class="cnt pic-f"><p></p>'+
		'<div id="f_upload" class="m-loading"><div id="flash_content"></div></div></div>'+
		/*'<div class="cnt pic-n" style="display:none;"><p>返回 <a id="pic_f">常规模式</a></p>'+
		'<div id="n_upload"><input name="image" type="file" id="pic_src" size="45"><span>'+size_tips+'</span></div></div>'+*/
		'<div class="btn" id="btn_upload">更新</div></form></div>').appendTo("body").fadeIn(300);
	var params = {
		photoUrl:"http://static.hdslb.com/images/member/noface.gif",
		hasDelete:1,
		target_url:target_url,
		callback:"getFlashPicUrl"
	};
	if(image_url!=""){
		params.image_url = image_url;
	}
	swfobject.embedSWF(
		"http://static.hdslb.com/images/member_v2/"+swf,
		"flash_content",
		"100%",
		"100%",
		"0",
		"",
		params,
		{
			quality:"high",
			allowscriptaccess:"always"
		}
	);
	$("form",dlg).attr("action",action);
	dlg.attr("mode","flash");
	$(".close",dlg).click(function(){
		mask.remove();
		dlg.remove();
	});
	$("#pic_n",dlg).click(function(){
		$(".pic-f").hide();
		$(".pic-n").show();
		$("#btn_upload",dlg).show();
		dlg.attr("mode","normal");
	});
	$("#pic_f",dlg).click(function(){
		$(".pic-n").hide();
		$("#btn_upload",dlg).hide();
		$(".pic-f").show();
		dlg.attr("mode","flash");
	});
	$("#btn_upload",dlg).click(function(){
		if(dlg.attr("mode")=="flash"){
			mask.remove();
			dlg.remove();
			return;
		}
		var btn = $(this);
		if(btn.attr("loading")) return;
		var loading = new MessageBox();
		if(btn.attr("hasMessageBox")!=""){
			btn.attr("hasMessageBox","");
		}
		btn.attr("loading",1);
		loading.showLoading(btn,"上传中",0);
		_rnd_func_name = "_bili_upload_cb_"+(new Date().getTime());
		$("form",dlg).attr("action", action+(action.indexOf("?")==-1?"?callback=" : "&callback=")+_rnd_func_name);
		var timeout;
		timeout = setTimeout(function(){
			loading.close();
			new MessageBox().show(btn,"上传失败，请重试");
			btn.removeAttr("loading");
		},8000);
		
		window[_rnd_func_name] = function(data)
		{
			clearTimeout(timeout);
			loading.close();
			btn.removeAttr("loading");
			if(data.code != 0){
				new MessageBox().show(btn,data.msg);
			} else{
				if(mode!="sp_index"){
					$("#preview img").attr("src",data.preview);
					$("#litpic").val(data.img);
					$("#preview .mask").hide();
				} else{
					$("#preview_index img").attr("src",data.preview);
					$("#litpic_index").val(data.img);
					$("#preview_index .mask").hide();
				}
				mask.remove();
				dlg.remove();
			}
		};
		$("form",dlg).submit();
	});
}
function getFlashPicUrl(pic_url){
	var data = JSON.parse(pic_url);
	var btn = $(".pic-dialog #btn_upload");
	btn.text("完成").show();
	if($("#pic_mode").length==0||$("#pic_mode").val()!="sp_index"){
		$("#preview img").attr("src",data.preview);
		$("#litpic").val(data.img);
		$("#preview .mask").hide();
	} else{
		$("#preview_index img").attr("src",data.preview);
		$("#litpic_index").val(data.img);
		$("#preview_index .mask").hide();
	}
}

function selectInit(){
	if($(".control-btn-select").length>0){
		$(".control-btn-select").each(function(i,e){
			var e = $(e);
			e.find("span").html($("option:selected",e).html());
		});
	}
}
function checkInit(){
	$(".check-box input").each(function(k, v){
		if ($(v).attr("checked"))
		{
			if(!$(v).parent().hasClass("checked")) $(v).parent().addClass("checked");
		}else
		{
			if($(v).parent().hasClass("checked")) $(v).parent().removeClass("checked");
		}
	});
}
var last_mod = "";
var mod_title = [];
window.msg_mode = false;
if (typeof(window.ajaxQuery)!="undefined" && typeof(window.ajaxQuery.abort)=="function") window.ajaxQuery.abort();
window.ajaxQuery = undefined;

//一些例外的hash
var hash_exception = {
	"dm_manage":"/video_manage.html?view=dm_manage",
	"gl_manage":"/video_manage.html?view=gl_manage",
	"sx_manage":"/video_manage.html?view=sx_manage",
	"sp_subscribe":"/favorite_manage.html?act=sp",
	"video_manage_history":"/video_manage.html?act=history",
	"appeal_manage":"/video_manage.html?view=appeal_manage"
};

if(location.hash && location.hash.match(/message\?mid=(\d+)*/)) {
	location.href = 'http://message.bilibili.com/#whisper/mid' + location.hash.match(/message\?mid=(\d+)*/)[1];
} else if(location.hash && location.hash.match(/message/)) {
	location.href = 'http://message.bilibili.com/#whisper';
}

$(function () {
	var url_rewrite = {
		"topic&act=edit":"topic.html?act=edit"
	};
	var qs_exception = {
		"main":"index.html"
	};
	var btn_exception = {
		"weibo_bind":"btn_profile",
		"qq_bind":"btn_profile"
	};
	$("#content").delegate(".box-tab a","click",function(){
		switchTab($(this));
	});
	$("#content").delegate(".main-content-tab a","click",function(){
		switchContentTab($(this));
	});
	$("#content").delegate(".control-content-tab div","click",function(){
		switchControlContentTab($(this));
	});
	$("#content").delegate(".select-type button","click",function(){
		switchType($(this));
	});
	$("#content").delegate(".content-tab div","click",function(){
		switchSubmitTab($(this));
	});
	$("#content").delegate(".subtab-list li","click",function(){
		switchSubTab($(this));
	});
	$("#content").delegate(".search-list li","click",function(){
		typeSelection($(this),$(this).parent().attr("mode"));
	});
	
	$("#content").delegate(".control-btn-select select","change",function(){
		$(this).prev().html($("option:selected",this).html());
	});
	$("#content").delegate("#preview[mode],#preview_index[mode]","click",function(){
		pic_upload($(this).attr("mode"));
	});

	$("#content").on("click","#btn_addFilter",function(){
		var appendTarget = "#filter_str";
		var querySource = "";
		var target_type = 0;
		switch ($("#keyword_tab .control-content-tab-item.on").attr("id")){
			case "keyword_string":
				appendTarget = "#filter_str";
				querySource = "#txt_new_filter_str";
				target_type = 0;
				break;
			case "keyword_account":
				appendTarget = "#filter_user";
				querySource = "#txt_new_filter_user";
				target_type = 2;
				break;
			case "keyword_preg":
				appendTarget = "#filter_regex";
				querySource = "#txt_new_filter_regex";
				target_type = 1;
				break;
		}
		if (!$(querySource).val())
		{
			new MessageBox().show($(querySource), "请输入要添加的过滤信息", 1000, "error");
			setTimeout(function(){
				$(querySource).focus();
			},0);
			return false;
		}
		var filter_data = {
			type:target_type,
			filter:$(querySource).val()
		};
		FilterManage.appendNewFilter(appendTarget, filter_data);
		$(querySource).val("");
	});

	function setFixed()
	{
		var navi = $(".navi");
		var offsetTop =$(".content").offset().top;
		if($(window).scrollTop()>offsetTop && $(window).height()>navi.height() && navi.height()<$(".container-grid-layout").height())
		{
			var off = $(".container-grid-layout").offset();
			navi.css({position:"fixed", left: ( off.left - navi.width() - 13 ) + "px", top: "5px"});
			if ($(".footer").offset().top - navi.offset().top < navi.outerHeight()) {
				navi.css({
					'top': $(".footer").offset().top - $(window).scrollTop() - navi.outerHeight() - 20
				});
			}
		} else {
			navi.css("top", "");
			navi.css("left", "");
			navi.css({position:"relative"});
		}
	}

	$(window)
		.scroll(setFixed)
		.resize(setFixed)
		.trigger("scroll");
	
	
	$("#content").delegate(".check-box","click",function(){
		var check_wrapper = $(this);
		var check = $(":checkbox",check_wrapper);
		if(check_wrapper.hasClass("disabled")){ return; }
		check_wrapper.toggleClass("checked");
		if(check_wrapper.hasClass("checked")){
			check.attr("checked",true);
			check.trigger("change");
		}
		else{
			check.attr("checked",false);
			check.trigger("change");
		}
	});
	
	$("#content").on('change',".check-box input",function(){
		var self = this;
		setTimeout(function(){
			if ($(self).attr("checked"))
			{
				if(!$(self).parent().hasClass("checked")) $(self).parent().addClass("checked");
			}else
			{
				if($(self).parent().hasClass("checked")) $(self).parent().removeClass("checked");
			}
		},0);
	});
	
	
	$(".banner-233").click(function () {$(this).stop().fadeOut('fast', function() {$(this).fadeIn('fast');});});
	

	$(".side-navi a.sub").click(function(){
		$(".side-navi a.sub").removeClass("open").addClass("close");
		$(this).addClass("open");
		$(".side-navi a.sub").each(function(i,e) {
            var e = $(e);
			if(e.hasClass("open")){
				e.next().slideToggle(200);
			}
			//else{
			//	e.next().slideUp(100);
			//}
        });
	});
	$(".side-navi a.n").click(function(){
		//$(".side-navi a.sub").removeClass("open").next().slideUp(100);
	});

	window.tabInsts = [];
	$(window).hashchange(function() {
		if (typeof(window._active_fmb) == "object" && window._active_fmb && new Date().getTime() - window._active_fmb.startup_date.getTime() > 500) {
			window._active_fmb.close();
		}
		window.onbeforeunload = function() {};
		$(".m_layer").remove();
		$(".tool-tip").remove();

		var mod = location.hash.substr(1),
			mod_hash = Request.hash(),
			mod_id = Request.root(),
			mod_qs = Request.get(null, mod),
			mod_sub = Request.hash("sub"),
			mod_tab = Request.hash("tab");

		if (!mod_hash) {
			if (!Request.get()) {
				location.hash = "main";
				return;
			} else {
				return;
			}
		}

		if (!Request.hash("next", last_mod) && Request.get(null) == Request.get(null, last_mod) && (mod_sub && mod_id == Request.root(last_mod) || !mod_sub && mod == Request.root(last_mod) || mod_tab && mod_id == Request.root(last_mod))) {
			last_mod = mod;
			if (mod_tab && !mod_sub) {
				switchContentTab($(".main-content-tab a[tab='" + mod_tab + "']"));
			} else if (mod_tab && mod_sub) {
				switchTab($(".box-tab a[href='#" + mod_hash.split("&tab=")[0] + "']"), false, true);
				switchContentTab($(".main-content-tab a[tab='" + mod_tab + "']"));
			} else {
				switchTab($(".box-tab a[href='#" + mod_hash + "']"), false, true);
			}
			for (var i in window.tabInsts) {
				if (i == mod_id) {
					window.tabInsts[i](mod_tab);
				}
			}
			return;
		}

		if (mod_hash && mod != last_mod) {
			last_mod = mod;
			$("#content").hide();
			$(".side-navi a").removeClass("on");
			$(".side-navi a").find("i").removeClass("on");
			//$(".side-navi a.sub").next().hide();
			var btn = null;
			$(".side-navi a[id]").each(function(i, e) {
				var e = $(e);
				if (mod_id.indexOf(e.attr("id").split("btn_")[1]) >= 0) {
					e.addClass("on").parent().show();
					e.find("i").addClass("on");
					btn = e;
				}
			});
			if (btn == null) {
				if (typeof(btn_exception[mod_id]) != "undefined") btn = $('.side-navi a[id="' + btn_exception[mod_id] + '"]');
			}
			document.title = btn.text() + " - 会员中心 - 哔哩哔哩";
			if (typeof(window.ajaxQuery) != "undefined" && typeof(window.ajaxQuery.abort) == "function") window.ajaxQuery.abort();
			var url = "";
			if (typeof(mod_qs) != "undefined" && mod_qs) {
				if (typeof(hash_exception[mod_id]) != "undefined") {
					if (hash_exception[mod_id].indexOf("?") < 0) {
						url = hash_exception[mod_id].split("?")[0] + "?" + mod_qs;
					} else {
						url = hash_exception[mod_id] + "&" + mod_qs;
					}
				} else {
					if (typeof(qs_exception[mod_id]) != "undefined") url = qs_exception[mod_id].split("?")[0] + "?" + mod_qs;
					else url = "/" + mod_id + ".html?" + mod_qs;
				}
			} else {
				if (typeof(hash_exception[mod_id]) != "undefined") {
					url = hash_exception[mod_id];
				} else url = "/" + mod_id + ".html";
			}
			for (var r in url_rewrite) {
				if (mod.match(r)) {
					url = "/" + mod.replace(mod_hash.match(r)[0], url_rewrite[r]);
				}
			}
			$(".container-grid-layout").append('<div class="page-loading"></div>');

			if (typeof(lazyLoadContents) == "object") {
				while (lazyLoadContents.length > 0) {
					if (typeof(lazyLoadContents[0]) != "undefined") {
						try {
							lazyLoadContents[0].free();
						} catch (s) {

						}
					}
				}
			}
			window.ajaxQuery = $.ajax({
				url: url,
				data: {
					_true_old: 1
				},
				success: function(data) {
					$(".redactor_dropdown").remove();
					var content = $("#content");
					$(".container-grid-layout .page-loading").remove();
					content.hide();
					content.html(data);
					content.fadeIn(200);
					if (mod_tab) {
						switchTab($(".box-tab a[href='#" + mod_sub + "']"), false);
						switchContentTab($(".main-content-tab a[tab='" + mod_tab + "']"));
					} else {
						switchTab($(".box-tab a[href='#" + mod_hash + "']"), false);
					}
					bindToolTips.bind();
				},
				error: function(msg) {
					var content = $("#content");
					$(".container-grid-layout .page-loading").remove();
					last_mod = "";
					content.hide();
					content.html('<div class="load-error"></div>');
					content.fadeIn(200);
				}
			});
		}
	});
	
	var btns = $(".side-navi a");
	btns.each(function(i, e) {
		var e = $(e);
		if (typeof(e.attr("id")) != "undefined" && e.attr("id").match(/^btn_/)) {
			mod_title[e.attr("id").substr(4)] = e.text();
		}
	})

	if (!window.msg_mode) {
		$(window).hashchange();
	} else {
		$(".side-navi a").removeClass("on");
		$("#btn_" + window.current_mod).addClass("on");
	}
	
});

FilterManage = {
	filter_waitAppend:[],
	obj_accept_guest:"#accept_guest",
	obj_accept_advcmt:"#accept_advcmt",
	obj_accept_spcmt:"#accept_spcmt",
	obj_filter_str:"#filter_str",
	obj_filter_regex:"#filter_regex",
	obj_filter_user:"#filter_user",
	classBlockRank:"input_selrank",
	selectRank_Prefix: "selrank",
	appendFilterObject:function(appendTarget, _data)
	{
		$("<a class=\"filter-list-item\" href=\"javascript:void(0);\">"+htmlspecialchars(_data.filter)+"<i class=\"icon-circle-delete\"></i></a>").appendTo(appendTarget)
			.mouseenter(function (e){ $(this).children("i").show();})
			.mouseleave(function (e){ $(this).children("i").hide();})
			.find("i").click(function(){
				var obj = this;
				if (_data.fid)
				{
					var msgbox = new MessageBox().show(obj,'是否确认要删除？此操作不能恢复？','button',function(mobj){
						$.ajax("/video_manage.do?act=dmm&mode=delete_filter&fid="+_data.fid,{
							success:function (data)
							{
								$(obj).parent().fadeOut(200, function(){
									$(obj).parent().remove();
								});
								(new MessageBox()).show(obj.parentNode,data == 'OK' ? '删除成功!' : data,data == 'OK' ? 500 : 2000,data == 'OK' ? 'ok' : 'warning');
							},
							error:function(){
								new MessageBox({ Overlap:true,position:mobj.position}).show(obj,'提交失败，可能权限不足',2000,'error');
							}
						});
					});
				}else
				{
					FilterManage.filter_waitAppend.splice(FilterManage.filter_waitAppend.indexOf(_data),1);
					$(obj).parent().fadeOut(200, function(){
						$(obj).parent().remove();
					});
				}
			})
	},
	loadFilter:function(dm_inid)
	{
		$.getJSON("/video_manage.do?act=filter"+(typeof(dm_inid)!="undefined"?"&dm_inid="+dm_inid:""), function(data){
			$(FilterManage.obj_accept_guest).attr("checked", data.accept_guest ? true : false);
			$(FilterManage.obj_accept_guest).change();
			$(FilterManage.obj_accept_spcmt).attr("checked", data.accept_spcmt ? true : false);
			$(FilterManage.obj_accept_spcmt).change();
			$(FilterManage.obj_accept_advcmt).attr("checked", data.accept_advcmt ? true : false);
			$(FilterManage.obj_accept_advcmt).change();
			$(FilterManage.obj_filter_str).empty();
			$(FilterManage.obj_filter_regex).empty();
			$(FilterManage.obj_filter_user).empty();
			for (var i = 0; i < data.filter_list.length; i++)
			{
				(function(_data)
				{
					if (data.filter_list[i].type_id>=4)
					{
						var group_ids = data.filter_list[i].filter.split(",");
						for (var g = 0; g < group_ids.length; g++)
						{
							$("#"+FilterManage.selectRank_Prefix+"_"+data.filter_list[i].type_id+"_"+group_ids[g]).attr("checked", true);
							$("#"+FilterManage.selectRank_Prefix+"_"+data.filter_list[i].type_id+"_"+group_ids[g]).change();
						}
					}else if (_data.type_id == 0 || _data.type_id == 1 || _data.type_id == 2)
					{
						var appendTarget = FilterManage.obj_filter_str;
						if (_data.type_id==1) appendTarget = FilterManage.obj_filter_regex;
						if (_data.type_id==2) appendTarget = FilterManage.obj_filter_user;
						FilterManage.appendFilterObject(appendTarget, _data);
					}
				})(data.filter_list[i]);
			}
			//checkInit();
		});
	},
	appendNewFilter:function(appendTarget, filter_data)
	{
		FilterManage.filter_waitAppend.push(filter_data);
		FilterManage.appendFilterObject(appendTarget, filter_data);
	},
	submitFilterChange:function(dm_inid)
	{
		var block_values = "";
		$("."+FilterManage.classBlockRank+":checked").each(function(k,v){
			block_values+=(block_values?";":"")+$(v).val();
		});
		var self = this;
		var postData = {
			format:"json",
			accept_guest:$(FilterManage.obj_accept_guest).attr("checked") ? 1 : 0,
			accept_spcmt:$(FilterManage.obj_accept_spcmt).attr("checked") ? 1 : 0,
			accept_advcmt:$(FilterManage.obj_accept_advcmt).attr("checked") ? 1 : 0,
			block_group:block_values,
			new_filters:JSON.stringify(FilterManage.filter_waitAppend)
		};
		if(typeof(dm_inid)!="undefined"){
			postData.cid = dm_inid;
		}
		$.ajax({
			type:"POST",
			dataType:'json',
			url:'/video_manage.do?act=save_filter',
			data:postData,
			success:function(data)
			{
				if (data.code == 0)
				{
					new FloatMessageBox().show('save');
				}else{
					new MessageBox().show(self, data.msg, 2000, "error");
				}
			},
			error:function(){
				new FloatMessageBox().show("网络错误，请稍后重试","error");
			}
		});	
	}
}

function previewImage(file,imgId,type)
{
  var MAXWIDTH  = 128;
  var MAXHEIGHT = 128;
  if(type=="video"){
  	MAXWIDTH  = 120;
  	MAXHEIGHT = 90;
  }
  var div = document.getElementById(imgId);
  if (file.files && file.files[0])
  {
  	var img = document.getElementById('imghead');
  	img.onload = function(){
  	  var rect = clacImgZoomParam(MAXWIDTH, MAXHEIGHT, img.offsetWidth, img.offsetHeight);
  	  img.width = rect.width;
  	  img.height = rect.height;
  	  //img.style.marginLeft = rect.left+'px';
  	  //img.style.marginTop = rect.top+'px';
  	}
  	var reader = new FileReader();
  	reader.onload = function(evt){
  		img.src = evt.target.result;
  	}
  	reader.readAsDataURL(file.files[0]);
  }
  else
  {
    var sFilter='filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale,src="';
    file.select();
    var src = document.selection.createRange().text;
    var img = document.getElementById('imghead');
    img.filters.item('DXImageTransform.Microsoft.AlphaImageLoader').src = src;
    var rect = clacImgZoomParam(MAXWIDTH, MAXHEIGHT, img.offsetWidth, img.offsetHeight);
    status =('rect:'+rect.top+','+rect.left+','+rect.width+','+rect.height);
    div.innerHTML = "<div id=divhead style='width:"+rect.width+"px;height:"+rect.height+"px;margin-top:"+rect.top+"px;margin-left:"+rect.left+"px;"+sFilter+src+"\"'></div>";
  }
}
function clacImgZoomParam( maxWidth, maxHeight, width, height ){
	var param = {
		top:0, 
		left:0, 
		width:width, 
		height:height
	};
	if( width>maxWidth || height>maxHeight )
	{
		rateWidth = width / maxWidth;
		rateHeight = height / maxHeight;
		
		if( rateWidth > rateHeight )
		{
			param.width =  maxWidth;
			param.height = Math.round(height / rateWidth);
		}else
		{
			param.width = Math.round(width / rateHeight);
			param.height = maxHeight;
		}
	}
	
	param.left = Math.round((maxWidth - param.width) / 2);
	param.top = Math.round((maxHeight - param.height) / 2);
	return param;
}

/*绑定说明文字tooltips*/
var bindToolTips = (function(){
	var _animate = function(tips,pos,top,left){
		var aniData = {
			"opacity":1
		};
		var _moveT = 5,_moveL = 5;
		switch(pos){
			case "t":
				aniData["top"] = "-=5";
				_moveL = 0;
				break;
			case "b":
				aniData["top"] = "+=5";
				_moveT = -_moveT;
				_moveL = 0;
				break;
			case "l":
				aniData["left"] = "-=5";
				_moveT = 0;
				break;
			case "r":
				aniData["left"] = "+=5";
				_moveL = -_moveL;
				_moveT = 0;
				break;
			default:
				aniData["top"] = "-=5";
				_moveL = 0;
		}
		tips.css({"top":top+_moveT,"left":left+_moveL,"opacity":0});
		tips.animate(aniData,200);
	};
	var Tip = function(item){
		this.item = item;
	};
	Tip.prototype = {
		item:null,
		tip:null,
		visible:false,
		show : function(text,pos,maxWidth,close){
			var _this = this;
			if(_this.visible){
				return;
			}
			var e = _this.item;
			var top = e.offset().top,left = e.offset().left,offsetTop,offsetLeft;
			var tip = _this.tip = $('<div class="tool-tip"><div class="tip-arrow"></div><div class="tip-text"></div></div>');
			if (close === true) {
				var closeBtn = $('<div>').addClass('close').html('×').on('click', function() {
					tip.fadeOut(200, function() {
						_this.remove();
					});
				});
				tip.append(closeBtn);
			}
			if(typeof(text)!="undefined"){
				$(".tip-text",tip).html(text);
			} else{
				$(".tip-text",tip).html(e.attr("tips"));
			}
			_this.visible = true;
			tip.appendTo("body");
			if(typeof(maxWidth)!="undefined" && maxWidth){
				$(".tip-text",tip).css("max-width",maxWidth);
			}
			var _pos;
			if(typeof(pos)!="undefined"){
				_pos = pos;
			} else{
				_pos = e.attr("tips-pos");
			}
			switch(_pos){
				case "t":
					offsetTop = -tip.outerHeight()-10;
					offsetLeft = tip.outerWidth()>e.outerWidth()?(-(tip.outerWidth()-e.outerWidth())/2):0;
					tip.addClass("tool-tip-t");
					break;
				case "b":
					offsetTop = e.outerHeight()+10;
					offsetLeft = tip.outerWidth()>e.outerWidth()?(-(tip.outerWidth()-e.outerWidth())/2):0;
					tip.addClass("tool-tip-b");
					break;
				case "l":
					offsetTop = tip.outerHeight()>e.outerHeight()?(-(tip.outerHeight()-e.outerHeight())/2):0;
					offsetLeft = -tip.outerWidth()-10;
					tip.addClass("tool-tip-l");
					break;
				case "r":
					offsetTop = tip.outerHeight()>e.outerHeight()?(-(tip.outerHeight()-e.outerHeight())/2):0;
					offsetLeft = e.outerWidth()+10;
					tip.addClass("tool-tip-r");
					break;
				default:
					offsetTop = e.outerHeight()+10;
					offsetLeft = tip.outerWidth()>e.outerWidth()?(-(tip.outerWidth()-e.outerWidth())/2):0;
					tip.addClass("tool-tip-b");
			}
			top += offsetTop;
			left += offsetLeft;
			tip.css({"top":top+5,"left":left,"opacity":0});
			_animate(tip,_pos,top,left);
		},
		remove : function(){
			var _this = this;
			_this.tip.remove();
			_this.visible = false;
		},
		_menterHandler : function(){
			
		},
		_mleaveHandler : function(){
			
		},
		init : function(){
			var _this = this;
			var item = _this.item;
			item.off("mouseenter",_this._menterHandler);
			item.off("mouseleave",_this._mleaveHandler);
			_this._menterHandler = function(){
				_this.show();
			};
			_this._mleaveHandler = function(){
				_this.remove();
			};
			item.mouseenter(_this._menterHandler);
			item.mouseleave(_this._mleaveHandler);
		}
	};
	return {
		tips : [],
		bind : function(parent){
			var _this = this;
			var items;
			if(typeof(parent)!="undefined"){
				if(typeof parent.attr("tips") != "undefined"){
					items = parent;
				} else{
					items = $("[tips]",parent);
				}
			} else{
				items = $("[tips]");
			}
			items.each(function(i,e){
				var e = $(e);
				if(typeof e.attr("initialized") == "undefined"){
					var tip = new Tip(e);
					tip.id = _this.tips.length;
					tip.init();
					e.attr("initialized","true");
					_this.tips.push(tip);
				}
			});
		},
		show : function(item,text,pos,maxWidth,close){
			var _this = this;
			var inTips = 0;
			for(var i=0;i<_this.tips.length;i++){
				var tip = _this.tips[i];
				if(tip.item[0]===item[0]){
					tip.show(text,pos,maxWidth,close);
					inTips++;
				}
			}
			if(inTips==0){
				var tip = new Tip(item);
				tip.show(text,pos,maxWidth,close);
				_this.tips.push(tip);
			}
		},
		remove : function(item){
			var _this = this;
			for(var i=0;i<_this.tips.length;i++){
				var tip = _this.tips[i];
				if(tip.item[0]===item[0]){
					tip.remove();
				}
			}
		}
	};
})();

/*统计字数*/
var bindStrLenCount = (function(){
	var _show = function(elem,maxLen){
		_remove();
		var e = elem;
		var top = e.offset().top,left = e.offset().left,offsetTop,offsetLeft;
		var count = $('<div class="strlen-count"></div>');
		var len = e.val().replace(/\r\n?/, "\n").length;
		count.html(len+"/"+maxLen);
		count.appendTo("body");
		offsetTop = -count.outerHeight()-1;
		offsetLeft = e.outerWidth()-count.outerWidth();
		top += offsetTop;
		left += offsetLeft;
		count.css({"top":top,"left":left});
		_checkLen(e,count,len,maxLen);
	};
	var _change = function(elem,maxLen){
		var e = elem;
		var count = $(".strlen-count");
		var len = e.val().replace(/\r\n?/, "\n").length;
		count.html(len+"/"+maxLen);
		var left = e.offset().left + e.outerWidth()-count.outerWidth();
		count.css({"left":left});
		_checkLen(e,count,len,maxLen);
	};
	var _checkLen = function(e,count,len,max){
		if(len>max){
			count.addClass("error");
			e.addClass("error");
		} else{
			count.removeClass("error");
			e.removeClass("error");
		}
	};
	var _remove = function(){
		$(".strlen-count").remove();
	};
	var _focus = function(){
		var e = $(this);
		var maxLen = parseInt(e.attr("maxLen"));
		_show(e,maxLen);
	};
	var _blur = function(){
		var e = $(this);
		_remove(e);
	};
	var _keyup = function(){
		var e = $(this);
		var maxLen = parseInt(e.attr("maxLen"));
		_change(e,maxLen);
	};
	var _bind = function(elem,max){
		var items = elem;
		items.attr("maxLen",max);
		items.off("focus",_focus);
		items.off("blur",_blur);
		items.focus(_focus);
		items.blur(_blur);
		items.keyup(_keyup)
	};
	return {
		bind:_bind
	};
})();

function getInputSelection(input) {
	var start = 0, end = 0, normalizedValue, range,
        textInputRange, len, endRange;
	input.focus();
	if (document.selection) {
		var range = document.selection.createRange();
        if (range && range.parentElement() == input) {
            len = input.value.length;
            normalizedValue = input.value.replace(/\r\n/g, "\n");

            textInputRange = input.createTextRange();
            textInputRange.moveToBookmark(range.getBookmark());

            endRange = input.createTextRange();
            endRange.collapse(false);

            if (textInputRange.compareEndPoints("StartToEnd", endRange) > -1) {
                start = end = len;
            } else {
                start = -textInputRange.moveStart("character", -len);
                start += normalizedValue.slice(0, start).split("\n").length - 1;

                if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
                    end = len;
                } else {
                    end = -textInputRange.moveEnd("character", -len);
                    end += normalizedValue.slice(0, end).split("\n").length - 1;
                }
            }
        }
		return {
			start: start,
			end: end
		};
	} else if (input.selectionStart || input.selectionStart == '0') {
		start = input.selectionStart;
		end  = input.selectionEnd;
		return {
			start: start,
			end: end
		};
	} else {
		return false;
	}
}

var TagInput = function(container, options) {
	var self = this;
	this.container = $(container);
	this.options = options || {};
	this.input = container.find('input:text');
	this.lastTag = null;
	this._queue = [];
	this._value = [];
	this._disabledValue = [];
	this.maxLen = 10;
	this.maxStrLen = 20;
	this.container.on('click', function() {
		self.input.focus();
		self.container.addClass('focus');
	});
	this.container.on('focusout', function() {
		self.container.removeClass('focus');
	});
	this.input.on('keydown', function(event) {
		if (event.keyCode == 188 && !event.shiftKey || event.keyCode == 13) {
			if (event.keyCode == 13 && self.input.data('bilibiliSuggest') && self.input.data('bilibiliSuggest')._isActive()) {
				return false;
			}
			if (self.check(self.input.val()) === true) {
				self.addToLast();
			}
			return false;
		}
		if (event.keyCode == 8 && !self.input.val()) {
			self.input.val(self.remove());
			self.input.focus();
			return false;
		}
	});
	this.input.on('propertychange input', function(event) {
		if (self.check(self.input.val()) === true && self.needParse()) {
			self.change(event);
		} else if (self.needParse()) {
			self.input.val(self.input.val().replace(/[,，]+/g, ''));
		}
		self.checkLen(self.input.val());
		self.trigger('onInput', self.container, self.input.val());
	});
	this.input.on('blur', function() {
		if (self.check(self.input.val()) === true) {
			self.addToLast();
		}
	});
	this.hiddenInput = this.container.find('input[name="tags"]').length ? this.container.find('input[name="tags"]') : $('<input type="hidden" name="tags" />').appendTo(this.container);
	this.nums = $('<div class="num"></div>').appendTo(this.container);
	this.setNum();
};
TagInput.prototype.trigger = function() {
	var args = Array.prototype.slice.call(arguments, 0);
	var handler = args.shift();
	if (this.options[handler]) {
		return this.options[handler].apply(this, args);
	}
};
TagInput.prototype.change = function(event) {
	var input = this.input.get(0),
		tempTags = $.trim(input.value).replace(/，/g, ',').split(','),
		i, j, k, tags = [];
	for (i = 0; i < tempTags.length; i++) {
		if (this.check(tempTags[i]) !== true && i != tempTags.length - 1) {
			continue;
		}
		tags.push(tempTags[i]);
	}
	if (tags.length > 1) {
		var _last = tags.length <= this.maxLen ? tags.length : this.maxLen + 1;
		for (k = 0; k < tags.length; k++) {
			var tag = tags[k];
			if (!tag || !this.checkDuplicated(tag)) continue;
			this.checkWord(tag, this.add(tag));
			if (k >= this.maxLen) {
				break;
			}
		}
		//input.value = tags[_last - 1];
		input.value = '';
		input.focus();
	} else if (input.value == '，' || input.value == ',') {
		input.value = input.value.replace(/[,，]+/g, '');
	}
};
TagInput.prototype.add = function(tag) {
	if (tag === false) return false;
	var self = this;
	if (this._value.length >= this.maxLen) {
		this.container.addClass('max');
		this.container.on('focusout.maxlen', function() {
			self.container.removeClass('max');
			self.container.off('focusout.maxlen');
		});
		this.trigger('onError', this.container, 'overlimit');
		return false;
	}
	tag = tag.replace(/[,，]+/g, '');
	if (!this.checkLen(tag)) {
		return false;
	}
	var ele = $('<div class="tag-item"><i class="upload-icon tag"></i><span>' + tag + '<div class="del"></div></span></div>').attr('tag', tag).data('tag', tag).insertBefore(this.input);
	ele.on('click', function() {
		self.remove(tag);
	});
	this._value.push(tag);
	this._queue.push(tag);
	this.lastTag = tag;
	this.setValue();
	this.trigger('onAdd', tag);
	return ele;
};
TagInput.prototype.addToLast = function() {
	this.input.focus();
	var tag = this.getTag();
	this.checkWord(tag, this.add(tag));
};
TagInput.prototype.remove = function(tag) {
	if (typeof tag == 'undefined') {
		if (this.lastTag == null) return;
		tag = this.lastTag;
	}
	var ele = this.getTagElement(tag).last();
	ele.remove();
	var index = $.inArray(tag, this._value);
	if (index >= 0) {
		this._value.splice(index, 1);
	}
	var disabledIndex = $.inArray(tag, this._disabledValue);
	if (disabledIndex >= 0) {
		this._disabledValue.splice(disabledIndex, 1);
	}
	var queueIndex = $.inArray(tag, this._queue);
	if (queueIndex >= 0) {
		this._queue.splice(queueIndex, 1);
	}
	this.lastTag = this._queue[this._queue.length - 1] || null;
	this.setValue();
	this.trigger('onRemove', tag);
	return tag;
};
TagInput.prototype.clear = function() {
	this.getTagElement().remove();
	this._value = [];
	this._disabledValue = [];
	this.lastTag = null;
	this.setValue();
};
TagInput.prototype.check = function(value) {
	if (value && value.replace(/[,，]+/g, '').replace(/\s+/g, ' ') != ' ') {
		return true;
	} else {
		return false;
	}
};
TagInput.prototype.checkDuplicated = function(tag) {
	if ($.inArray(tag, this._value) >= 0 || $.inArray(tag, this._disabledValue) >= 0) {
		return false;
	} else {
		return true;
	}
};
TagInput.prototype.checkLen = function(tag) {
	if (tag.length > this.maxStrLen) {
		this.trigger('onError', this.container, 'strOverlimit');
		return false;
	} else {
		return true;
	}
};
TagInput.prototype.checkWord = function(value, el) {
	if (this.options.needCheck === false || el === false) return;
	var self = this;
	$.get('/video_submit_templates.do?act=tagcheck&msg=' + value, function(res) {
		if (res && res['state']) {
			el.addClass('disabled');
			self._disabledValue.push(value);
			self._value.splice($.inArray(value, self._value), 1);
			self.setValue();
			self.trigger('onError', self.container, 'word');
		}
	});
};
TagInput.prototype.needParse = function() {
	return (/[,，]+/g).test(this.input.val());
};
TagInput.prototype.build = function() {
	var self = this;
	this._value = [];
	this.container.find('[tag]').each(function(i, e) {
		var ele = $(e);
		self._value.push(e.attr('tag'));
	});
};
TagInput.prototype.buildByTags = function(tags, needCheck) {
	if ($.trim(tags)) {
		tags = tags.split(',');
		for (var i = 0; i < tags.length; i++) {
			var el = this.add(tags[i]);
			if (needCheck) {
				this.checkWord(tags[i], el);
			}
		}
	}
};
TagInput.prototype.getTagElement = function(tag) {
	if (typeof tag != 'undefined') {
		for (var i = 0; i < this.container.find('[tag]').length; i++) {
			var el = $(this.container.find('[tag]')[i]);
			if (el.data('tag') == tag) {
				return el;
			}
		}
	} else {
		return this.container.find('[tag]');
	}
};
TagInput.prototype.setValue = function() {
	this.hiddenInput.val(this.getValue());
	this.setNum();
};
TagInput.prototype.setNum = function() {
	this.nums.html(this._value.length + '/' + this.maxLen);
};
TagInput.prototype.getValue = function() {
	return this._value.join(',');
};
TagInput.prototype.getTag = function() {
	var input = this.input.get(0),
		tag,
		_val;
	var pos = getInputSelection(input);
	if (pos) {
		tag = input.value.substring(0, pos.start);
		_val = input.value.substring(pos.end, input.value.length);
	} else {
		tag = input.value;
		_val = '';
	}
	if (!this.checkDuplicated(tag)) {
		this.trigger('onError', this.container, 'duplicated');
		return false;
	}
	if (!this.checkLen(tag)) {
		return false;
	}
	input.value = _val;
	return tag;
};

var tagListData = null;
function loadTagRecommendList(target, tid, tagInput, recommendTags) {
	if(tagListData) {
		createTagRecommendList(tagListData[tid], target, tid, tagInput, recommendTags);
	} else {
		window['onRecommendTagsLoaded'] = function(data) {
			if(data) {
				_tagListData = data;
				createTagRecommendList(data[tid], target, tid, tagInput, recommendTags);
			}
		}
		$.ajax({
			url: 'http://api.bilibili.com/tags/hot_tags?type=submission',
			cache: true,
			dataType: 'jsonp',
			jsonp: 'jsonp_callback',
			jsonpCallback: 'onRecommendTagsLoaded'
		});
	}
}

function createTagRecommendList(data, target, tid, tagInput, recommendTags) {
	if(!tid || !data) return;
	var tagRecommend = $('.tag-recommend-list', target);
	if(!tagRecommend.length) {
		tagRecommend = $('<div class="tag-recommend-list"><div class="list-cnt"><span class="t">推荐标签：</span></div></div>').appendTo(target);
		tagInput.options.onAdd = function(tag) {
			tagRecommend.find('[tag="' + tag + '"]').addClass('on');
		};
		tagInput.options.onRemove = function(tag) {
			tagRecommend.find('[tag="' + tag + '"]').removeClass('on');
		};
		tagRecommend.smoothScroll();
	}
	tagRecommend.show();
	recommendTags = recommendTags || {};
	var _exTags = recommendTags[tid] || [];
	var list = tagRecommend.find('.list-cnt');
	list.empty().append('<span class="t">推荐标签：</span>');
	var _data = _exTags.concat([]);
	for (var i = 0; i < data.length; i++) {
		var tag = data[i];
		if($.inArray(tag, _exTags) < 0) {
			_data.push(tag);
		}
	}
	for (var i = 0; i < _data.length; i++) {
		var tag = _data[i];
		if(tag == '其他') continue;
		(function(tag) {
			var tagItem = $('<div class="tag-item" tag="' + tag + '"><i class="upload-icon tag"></i><span>' + tag + '</span></div>').appendTo(list).on('click', function(e) {
				tagInput.input.focus();
				e.stopPropagation();
				if($.inArray(tag, tagInput._value) >= 0) {
					tagInput.remove(tag);
				} else {
					tagInput.add(tag);
				}
			});
			if($.inArray(tag, tagInput._value) >= 0) {
				tagItem.addClass('on');
			}
		})(tag);
	}
}

/*投稿*/
function postVideo(mode,tid,options){
	this.options = {
		arcType: null,
		allowFeed: false
	};
	if (typeof mode == 'object') {
		options = mode;
		mode = null;
		tid = null;
	}
	this.mergeOptions(options);
	this.submitMode = mode || "multi";
	this.loading = 0;
	this.codeMode = false;
	this.stateMsg = new MessageBox();
	this.msgTarget = null;
	this.tid = tid || -1;
	this.allowLocal = typeof(v_local_up)!="undefined"?v_local_up:false;
	this.promote = true;
	this.list = [];
	this.init();
}
postVideo.prototype = {
	strCount:bindStrLenCount,
	tips:bindToolTips,
	emm:new FormModules.ErrManager(),
	upload_query:[],
	TYPES:{
		/*SINA:{
			value:"sina",
			name:"新浪"
		},*/
		/*YOUKU:{
			value:"youku",
			name:"优酷"
		},*/
		/*QQ:{
			value:"qq",
			name:"腾讯"
		},*/
		LETV:{
			value:"letv",
			name:"乐视云"
		},
		VUPLOAD:{
			value:"vupload",
			name:"直传"
		}
	},
	addTemplate:null,
	uploadModuleName:"html5",
	mergeOptions: function (options) {
        if (typeof options != 'object') {
            return;
        }
        for (var k in this.options) {
            if (options.hasOwnProperty(k)) {
                this.options[k] = options[k];
            }
        }
    },
	init:function(){
		var pv = this;
		pv.mode = $("#comefrom").length>0?"submit":"edit";
		pv.addTemplate = '<li class="add-video clearfix">'+
					'<div class="control-btn-select" style="width: 70px;"><span>选择</span><select class="slt_src_type"><option value="0" selected="selected" disabled="disabled">选择</option></select></div>'+
					'<div class="upload-wrapper"><input type="text" class="control-inpt-input url" style="width: 155px;" /></div><input type="text" class="control-inpt-input pagename" style="width: 95px; margin-left:7px;" /><textarea class="control-inpt-input desc auto-size" style="width: 240px; margin-left: 7px;"></textarea><div class="btns-wrapper"><a id="add_video" class="btn simple">添加</a><a id="upload_video" class="btn simple">上传</a></div>'+
				'</li>';
		pv.strCount.bind($("#title"),40);
		pv.strCount.bind($("#description"),200);
		$("#odi_list .add-video").remove();
		pv.initType(pv.tid);

		if($('#tags').length) {
			$('#tags').attr('id', 'tags_input').removeAttr('name').width(120).wrap('<div class="tag-input-wrp"></div>').parents('.item-r').attr('id', 'tag_input_container');
			var tagInputWrp = $('.tag-input-wrp');
			$('<input type="hidden" name="tags" id="tags" />').appendTo(tagInputWrp);
		}
		var tagInput = this.tagInput = new TagInput($('#tags_input').parent(), {
			onError: function(el, error) {
				switch(error) {
					case 'overlimit':
						pv.emm.showErrMsg(el, "最多输入10个tag");
						break;
					case 'duplicated':
						pv.emm.showErrMsg(el, "请勿输入重复的tag");
						break;
					case 'strOverlimit':
						pv.emm.showErrMsg(el, "单个tag最多20字");
						break;
					case 'word':
						pv.emm.showErrMsg(el, "标签无效或含有敏感词", 0, null, 'tags');
						break;
					default:;
				}
				$(document).on('click.tagInputError', function() {
					tagInputWrp.removeClass('error');
					$(document).off('click.tagInputError');
				});
			},
			onInput: function(el, val) {
				if(val.length <= this.maxStrLen) {
					this.input.focus();
				}
			}
		});
		tagInput.buildByTags(tagInput.input.val());
		tagInput.input.val('').removeAttr('value');
		$(document).off('change.recommendTag', '#typeid');
		$(document).on('change.recommendTag', '#typeid', function() {
			loadTagRecommendList('#tag_input_container', $(this).val(), tagInput, pv.recommendTags);
		});

		$("#mission_id").change(function(){
			var target = $(this);
			$.ajax("/video_submit.do?act=mission_help&msid="+target.val(), {
				success:function(data){
					if(data) {
						if(typeof data == 'object') {
							pv.tips.show(target,data.post_help,"r");
						} else {
							pv.tips.show(target,data,"r");
						}
					} else {
						pv.tips.remove(target);
					}
				}
			});
		});

		var suggest_func = {
			positionOffset: {
				top: 3,
				left: 0
			},
			css: {
				minWidth: 240
			},
			source: function( request, response ) {
				var tmp_term_lst = request.term.split(",");
				
				$.getJSON( "/suggest?jsoncallback=?", {
					term: tmp_term_lst[tmp_term_lst.length-1].replace(/　/g,""),
					rnd:Math.random()
				}, response );
			},
			search: function() {
				// custom minLength
				this.value=this.value.replace(/，/g,",");
				var term = this.value;
				var tmp_term_lst = term.split(",");
				if (tmp_term_lst[tmp_term_lst.length-1].length < 1) return false;
				
				if ( term.charCodeAt(0)<255 && term.length < 1 || term.length>40) {
					return false;
				}
			},
			focus: function() {
				// prevent value inserted on focus
				return false;
			},
			select: function( event, ui ) {
				var tmp_term_lst = this.value.split(",");
				_value = "";
				if (tmp_term_lst.length > 1)
				{
					for (i=0;i<tmp_term_lst.length-1;i++)
					{
						_value+=tmp_term_lst[i]+",";
					}
				}
				_value+=ui.item.value;
				this.value = _value;
				tagInput.addToLast();
				return false;
			},
			renderItem: function( ul, item ) {
				return $( "<li></li>" )
				.addClass('suggest-item')
				.data( "item.autocomplete", item )
				.append( "<a style=\"text-align:left\">" + item.value + "<em style=\"float:right;font-size:10px;\""+(item.match ? " title=\"(Match Token: "+item.match+")\"" : "")+">" + (item.desc ? item.desc : item.ref + "个")+"</em></a>" )
				.appendTo( ul );
			}
		};
		if ($("#tags_input").length){
			$("#tags_input").bilibiliSuggestion(suggest_func);
		}
		$( "#multiP_sortable" ).sortable({ 
			items: "li.item",
			cancel:"li.uploading,select,.swfupload,input,textarea,a",
			change:function(e,ui){
				pv.buildList();
			}
		});

		var addControl = pv.initAddControl();

		$('<i class="i"><a href="http://www.bilibili.com/html/help.html#f1_3" target="_blank">点击查看帮助</a></i><br />').insertBefore($("#content_url_page").parents("li").find(".item-l i.i"));
		if(pv.promote){
			var pic = $('<div><a href="video_submit.html?tpl=upload"><img style="display: block;margin: 30px auto 0;" src="http://static.hdslb.com/upload/img/banner.jpg"/></a></div>').insertBefore("#single_item");
			/*pic.click(function(){
				pv.manuallyPost();
				$("#content_url_page").parents("li").prependTo(".form-post > ul");
				if(pv.allowLocal){
					$('[value="'+pv.TYPES.VUPLOAD.value+'"]',addControl).attr("selected",true);
				} else{
					$('[value="'+pv.TYPES.LETV.value+'"]',addControl).attr("selected",true);
				}
				$(".slt_src_type",addControl).change();
			});*/
		}

		function _hashchange(){
			if(pv.queryCheck()){
				$(".side-navi a[href]").off("click",_hashchange);
			} else{
				if(confirm("有视频正在上传")){
					$(".side-navi a[href]").off("click",_hashchange);
					for(var i=0;i<pv.upload_query.length;i++){
						var data = pv.upload_query[i].data('data');
						if(data && data.jqXHR){
							data.abort();
						}
					}
					return true;
				} else{
					return false;
				}
			}
		}
		$(".side-navi a[href]").click(_hashchange);
		if(pv.mode=="submit"){
			pv.infoRecover();
		}

	},
	queryCheck:function(){
		var pv = this;
		for(var i in pv.upload_query){
			if(pv.upload_query[i].is(":visible") && pv.upload_query[i].data('uploading')){
				return false;
			}
		}
		window.onbeforeunload = function(){};
		return true;
	},
	initAddControl:function(){
		var pv = this;
		var add_control = $(pv.addTemplate).appendTo("#multiP_sortable");
		pv.strCount.bind($(".add-video .desc"),200);
		pv.initSelectObj(add_control);
		pv.bindTypeChange(add_control);
		$("#add_video",add_control).off("click");
		$("#add_video",add_control).click(function(){
			var item = $(this).closest('.add-video');
			var info = pv.checkVideoItemInfo(item,"add");
			if(info){
				pv.addP(info.url,info.src_type,info.description,info.pagename);
				item.find("input,textarea").val("");
			}
		});
		return add_control;
	},
	initUploader:function(object,mode){
		var pv = this;
		var status = object.find(".status");
		var uploadWrapper = $(".upload-wrapper",object);
		var uploadContent = $('.file-upload-content',uploadWrapper).remove();
		var infoWrapper = object.find(".info-wrapper").hide();
		var progress = object.find(".progress").hide();
		$(".upload-wrapper .url",object).val("").hide();
		var requesting = uploadWrapper.find(".requesting");
		if(requesting.length==0){
			requesting = $('<span class="requesting">上传组件初始化中...</span>').appendTo(uploadWrapper);
		} else{
			requesting.removeClass("retry").off("click").html("上传组件初始化中...");
		}
		object.off("click",".start");
		object.off("click",".stop");
		object.off("click",".del");

		function _setup(data){
			requesting.remove();
			if(!object.hasClass("add-video") && $(".start",object).length==0){
				$('<a class="start">上传</a><a class="stop">取消</a>').prependTo(object.find(".btns-wrapper")).hide();
			}
			if(mode == "flash"){
				object.data('flashData',data);
				object.addClass("flash");
				uploadContent = $('<div class="file-upload-content"><span id="upload_btn_placeholder"></span><span class="fn">未选择</span></div>').appendTo($(".upload-wrapper",object));
				var settings = getSWFSettings(data,object,pv);
				var swfu = new SWFUpload(settings);
				swfu.settings._create();
			} else{
				object.data('vuploadData',data);
				object.removeClass("flash");
				uploadContent = $('<div class="file-upload-content"><span class="browse-btn">选择文件<input type="file" name="files[]" class="fileinput"></span><span class="fn">未选择</span></div>').appendTo($(".upload-wrapper",object));
				if(typeof pv.allowLocal == "object" && pv.allowLocal.file_types){
					uploadContent.find('input[type="file"]').attr("accept",pv.allowLocal.file_types.join(",").replace(/\*/g,""));
				} else{
					uploadContent.find('input[type="file"]').attr("accept",".flv");
				}
				uploadContent.html5Uploader({
			        url:data.url,
			        postVideoModule: pv,
			        requestData:data,
			        uploadItem:object,
			        dropZone:object
			    });
			}
			uploadContent.attr("mode",mode);
			uploadContent.show();
			$('#add_video',object).hide();
			$('#upload_video',object).css('display',"inline-block");
			if($(".info-wrapper",object).length==0){
				infoWrapper = $('<div class="info-wrapper"><div class="status"></div><div title="剩余时间" class="time-info"><span>00:00:00</span></div><div title="上传速度" class="speed-info"></div></div>').appendTo(object).hide();
				$('<div class="progress"></div>').appendTo(object).hide();
			}
			infoWrapper.find(".time-info").html("");
			infoWrapper.find(".speed-info").html("");
			$(".progress", object).progressbar({
				value: 0
			});
			$(".edit",object).hide();
			$(".start",object).show();
		}

		if(mode == "flash" && typeof object.data('flashData') != "undefined"){
			_setup(object.data('flashData'));
			return;
		} else if(mode == "html5" && typeof object.data('vuploadData') != "undefined"){
			_setup(object.data('vuploadData'));
			return;
		}
		var url = "";
		if(mode == "flash"){
			url = "/get_upload_url";
		} else{
			url = "/get_vupload_url";
		}
		var request = $.getJSON(url, function(data){
			if (data.error_code == 21332){
			}else if (data.error_code == 21000){
				new MessageBox().showEx(uploadWrapper,"请刷新页面重新登录",2000,"error");
			}else if(data.error_code == 20000){
				new MessageBox().showEx(uploadWrapper,"请先登录",2000,"error");
			}else if(data.error_code){
				new MessageBox().showEx(uploadWrapper,data.error_text,2000,"error");
			}else if(data.errno){
				
			}else{
				if(!object.hasClass("add-video") && $(".start",object).length==0){
					$('<a class="start">上传</a><a class="stop">取消</a>').prependTo(object.find(".btns-wrapper")).hide();
				}
				_setup(data);
			}
			if(data.error_code || data.errno){
				requesting.addClass("retry").html("初始化失败,点击重试");
				requesting.click(function(){
					pv.initUploader(object,mode);
				});
			}
		}).error(function(){
			requesting.addClass("retry").html("初始化失败,点击重试");
			requesting.click(function(){
				pv.initUploader(object,mode);
			});
		});
	},
	initSelectObj:function(obj){
		var pv = this;
		for(var t in pv.TYPES){
			if(t=="VUPLOAD"&&!pv.allowLocal || t=="LETV") {
				continue;
			}
			var option = $('<option value="'+pv.TYPES[t].value+'">'+pv.TYPES[t].name+'</option>').appendTo($(".slt_src_type",obj));
		}
	},
	checkTid: function(data,tid, mainTid){
		var mainTid = mainTid || 0;
		for(i in data){
			for(var j = 0;j<data[i].length;j++){
				if(data[i][j].tid==tid&&i!=0){
					mainTid = i;
					return this.checkTid(data,i, mainTid);
				}
			}
		}
		return mainTid;
	},
	selectType: function(data, tid) {
		if(this.tid != -1) {
			tid = this.tid;
		}
		if(tid!=-1 && tid != 0){
			var mTid = this.checkTid(data,tid);
			$("#typeid_main option[value="+mTid+"]").attr("selected",true);
			$("#typeid_main").trigger("change");
			if($.inArray(parseInt(this.typeMap[tid].parentTid), this.mainTypeData) < 0) {
				tid = this.typeMap[tid].parentTid;
			}
			$("#typeid option[value="+tid+"]").attr("selected",true);
			$("#typeid").trigger("change");
		}
	},
	initType:function(tid){
		var pv = this;
		var container = $("#lanmu").empty();
		if(!container.length) {
			return;
		}
		var selectMain = $('<div class="control-btn-select" style="width: 120px;clear: both; ">'+
										'<span>请选择</span><select id="typeid_main"><option value=0 selected disabled="disabled">请选择</option></select></div>');
		var selectSub = $('<div class="control-btn-select" style="width: 150px;clear: both; margin-left:15px;">'+
								'<span>请选择</span><select name="typeid" id="typeid"><option value=0 selected disabled="disabled">请选择</option></select></div>');
		selectMain.appendTo($("#lanmu"));
		selectSub.appendTo($("#lanmu"));
		var exception = [32,33];
		pv.typeMap = {};

		pv.recommendTags = {};

		function createSub(sel,data){
			$("select",selectSub).empty();
			$("span",selectSub).html("请选择");
			var def_opt = $('<option value=0 selected>请选择</option>').appendTo($("select",selectSub));
			for(var j = 0;j<data[sel.val()].length;j++){
				var opt,
					type = data[sel.val()][j];
					type.parentTid = sel.val();
				pv.typeMap[type.tid] = type;
				pv.recommendTags[type.tid] = [];
				if(data[type.tid]){
					opt = $('<option value='+type.tid+'>'+type.typename+'</option>').appendTo($("select",selectSub));
					if($.inArray(type.tid,exception)>=0){
						opt.removeAttr("disabled");
					}
					for(var k = 0;k<data[type.tid].length;k++){
						/*var opt_sub = $('<option value='+data[data[sel.val()][j].tid][k].tid+'>　├'+data[data[sel.val()][j].tid][k].typename+'</option>').appendTo($("select",selectSub));*/
						data[type.tid][k].parentTid = type.tid;
						pv.typeMap[data[type.tid][k].tid] = data[type.tid][k];
						pv.recommendTags[type.tid].push(data[type.tid][k].typename);
					}
				} else{
					opt = $('<option value='+type.tid+'>'+type.typename+'</option>').appendTo($("select",selectSub));
				}
			}
			def_opt.attr("disabled",true);
			selectInit();
		}
		var mainTid = 0;
		function checkTid(data,tid){
			for(i in data){
				for(var j = 0;j<data[i].length;j++){
					if(data[i][j].tid==tid&&i!=0){
						mainTid = i;
						checkTid(data,i);
					}
				}
			}
			return mainTid;
		}
		$.ajax({
			url:"/js/types.json",
			dataType:"json",
			success:function(data){
				pv.mainTypeData = [];
				pv.typeData = data;
				for(var i = 0;i<data[0].length;i++){
					var opt = $('<option value='+data[0][i].tid+'>'+data[0][i].typename+'</option>').appendTo($("select",selectMain));
					pv.mainTypeData.push(data[0][i].tid);
				}

				$("select",selectMain).change(function(){
					var sel = $(this);
					createSub(sel,data);
				});
				var _timer;
				$("#typeid").change(function(){
					var vt_id = parseInt($(this).val()),
						_tInfo = pv.typeMap[vt_id],
						$this = $(this),
						maxWidth = 320;
					pv.tips.remove($this);
					clearInterval(_timer);
					_timer = setInterval(function() {
						if(pv.noTagRecommend || $('.tag-recommend-list').length) {
							clearInterval(_timer);
							switch(vt_id){
								case 20:
									pv.tips.show($this,"以ACG相关音乐为BGM的翻跳、原创作品，标题建议注明舞者名和歌曲名。（ACG相关音乐包括VOCALOID、UTAU、同人音乐、动漫用曲、游戏用曲）","r",maxWidth,true);
									break;
								case 154:
									pv.tips.show($this,"一切以三次元音乐作为BGM的舞蹈，包括偶像系舞蹈","r",maxWidth,true);
									break;
								case 33: case 32: case 94: case 120:
									pv.tips.show($this,_tInfo.abstract || "新番请依照 <b>【XX月/生肉/OVA等】新番名字 XX【字幕组】</b> 填写标题","r",maxWidth,true);
									break;
								case 40:
									pv.tips.show($this,_tInfo.abstract || "<b>视频带有一定理论基础相关的自主研究，制作，开发，改造等原创视频。</b>","r",maxWidth,true);
									break;
								case 83:
								case 145:
								case 146:
								case 147:
									pv.tips.show($this,_tInfo.abstract || "请使用“【类型】电影名称 年份【其他信息】”的格式填写标题。【合集投稿将不予审核通过】","r",maxWidth,true);
									break;
								case 24:
									pv.tips.show($this,_tInfo.abstract || "具有一定制作程度的动画或静画的二次创作（使用官方素材（包括但不限于PV、动画、游戏OP）自主再创作）视频，不适合三次创作（使用二次创作作品再创作）投稿。<br />转载稿件请在简介中注明原作者、转载地址和相关的简介内容。<br />自制稿件请写明使用的音声素材。","r",maxWidth,true);
									break;
								case 43: case 44: case 45: case 46:
									pv.tips.show($this,_tInfo.abstract || "使用MMD（MikuMikuDance）和其他3D建模类软件制作的视频投稿。<br />转载稿件请在简介中注明原作者、转载地址和相关的简介内容。<br />自制稿件请写明使用的模型、动作、镜头设定和音声素材来源。若未使用素材也请一并说明。","r",maxWidth,true);
									break;
								case 49:
									pv.tips.show($this,_tInfo.abstract || "配音作品请使用【配音】作品名 的类似格式。建议简介注明参与者信息。","r",maxWidth,true);
									break;
								case 50:
									pv.tips.show($this,_tInfo.abstract || "手书类动画作品投稿至本区。","r",maxWidth,true);
									break;
								case 51:
									pv.tips.show($this,_tInfo.abstract || "各类动画咨询、PV预告和专辑CM投稿至本区。","r",maxWidth,true);
									break;
								case 52:
									pv.tips.show($this,_tInfo.abstract || "各类动画杂谈作品投稿至本区。","r",maxWidth,true);
									break;
								case 53:
									pv.tips.show($this,_tInfo.abstract || "恶搞向作品、OP/ED替换作品、相关排行榜等投稿至本区。","r",maxWidth,true);
									break;
								case 34:
								case 15:
									pv.tips.show($this,"暂不接受国产电视剧投稿","r",maxWidth,true);
									break;
								default:
									if(_tInfo && _tInfo.abstract) {
										pv.tips.show($this,_tInfo.abstract,"r",maxWidth,true);
									} else {
										pv.tips.remove($this);
									}
							}
						}
					}, 100);
				});
				$("#typeid_main").change(function(){
					$('.tag-recommend-list').hide();
					pv.tips.remove($("#typeid"));
				});
				pv.selectType(data, tid);
			}
		});
	},
	changeArcType: function(type) {
		var frmItem = $('#arctype'),
			comefrom = $('#block_videoFrom #comefrom'),
			recovery = $('#block_recovery'),
			btn = $("#select_arctype button#" + type);
		$("#select_arctype button").removeClass("on");
		btn.addClass("on");
		if(type=="select_type_ref"){
			frmItem.val('Copy');
			comefrom.val("");
			recovery.hide();
		}
		else if(type=="select_type_src"){
			frmItem.val('Original');
			comefrom.val("自制");
			recovery.hide();
		}
		else{
			frmItem.val('Copy');
			comefrom.val("");
			recovery.show();
		}
		var planBlock = $("#block_plan");
		if(planBlock.length) {
			if(type=="select_type_src"){
				planBlock.show();
			} else {
				planBlock.hide();
				$("#allow_feed", planBlock).attr('checked', false);
			}
		}
	},
	fastPost:function(item){
		var pv = this;
		pv.submitMode="multi";
		var url = $("input.url",item).val();
		var src_type = pv.checkUrl(url,item);
		if(!src_type) return false;
		pv.msgTarget = $("input.url",item);
		pv.getVideoInfo(url);
		return true;
	},
	manuallyPost:function(){
		$(".m_layer").remove();
		$("#step_1").hide();
		$("#step_2").show();
		pv.submitMode="multi";
		$("#content_url_page").show();
		$("#code_mode").show();
		location.hash = "video_submit&next&sub";
	},
	checkUrl:function(url,item,emm){
		var pv = this;
		var src_type = "";
		if(url.indexOf("video.sina.com.cn")>=0){
			src_type=pv.TYPES.SINA.value;
		} else if(url.indexOf("v.qq.com")>=0){
			src_type=pv.TYPES.QQ.value;
		} else{
			if(typeof(emm)!="undefined"){
				emm.showErrMsg($("input.url",item),"请填写正确的地址");
			} else{
				new MessageBox().showEx($("input.url",item),"请填写正确的地址",0,"error");
			}
			return false;
		}
		return src_type;
	},
	getVideoInfo:function(url){
		var pv = this;
		if(pv.stateMsg.bindobj!=null&&pv.stateMsg.bindobj.attr("hasmessagebox")!="") {
			//pv.stateMsg.close();
			return;
		}
		pv.stateMsg = new MessageBox();
		pv.stateMsg.showLoading(pv.msgTarget,"加载中",0);
		pv.loading = 1;
		var timeout = setTimeout(function(){
			if(pv.loading==1){
				pv.stateMsg.close();
				pv.stateMsg.show(pv.msgTarget,"网络超时");
			}
		},10000);
		$.ajax({
            url:"/getinfo",
            data:{ url:url },
            success:function(data){
            	clearTimeout(timeout);
                if(!data.error_code&&data.video_url){
                	pv.stateMsg.close();
                    var info = {
                        url:data.video_url,
	                    src_type:data.video_type,
	                    title:data.title,
	                    cover:data.cover,
	                    tags:data.keywords,
	                    description:data.description
                    };
                    pv.fillVideoInfo(info);
                } else{
                	pv.stateMsg.close();
                	pv.stateMsg = new MessageBox();
                	pv.stateMsg.show(pv.msgTarget,"信息获取失败");
                	//pv.stateMsg.change("信息获取失败",2000);
                }
                pv.loading = 0;
            },
			error:function() {
				clearTimeout(timeout);
				pv.stateMsg.close();
            	pv.stateMsg = new MessageBox();
            	pv.stateMsg.show(pv.msgTarget,"信息加载失败");
                pv.loading = 0;
			}
		});
	},
	getVideoTypeName:function(video_type){
		if(video_type=="youku"){
			return "优酷";
		} else if(video_type=="sina"){
			return "新浪";
		} else if(video_type=="qq"){
			return "腾讯";
		} else{
			return "";
		}
	},
	fillVideoInfo:function(info,mode){
		var pv = this;
		$("#step_1").hide();
		$("#step_2").show();
		location.hash = "video_submit&next&sub";
		var target = $("#content_url_page");
		target.attr("video-type",info.src_type);
		target.attr("video-link",info.url);
		$("#title").val(info.title);
		$("#imghead").attr("src",info.cover);
		$("#litpic").val(info.cover);
		$("#tags").val(info.tags);
		$("#description").val(info.description);
		pv.addP(info.url,info.src_type,"","");
		target.show();
	},
	checkVideoItemInfo:function(item,mode,emm){
		var pv = this;
		var url = $("input.url",item).val();
		var slt_src_type = $(".slt_src_type",item);
		var src_type = slt_src_type.val();
		var repeat = 0;
		$("#multiP_sortable .item").each(function(i,e){
			var e = $(e);
			if (i!=item.index() && src_type != pv.TYPES.VUPLOAD.value && src_type != pv.TYPES.LETV.value && src_type == e.find(".slt_src_type").val() && url==e.find(".url").val()){
				if(typeof(emm)!="undefined"){
					emm.showErrMsg($("input.url",item),"视频源重复");
				} else{
					new MessageBox().showEx($("input.url",item),"视频源重复",0,"error");
				}
				repeat++;
				return false;
			}
		});
		if(repeat>0) return false;
		if(src_type=="0"){
			if(typeof(emm)!="undefined"){
				emm.showErrMsg(slt_src_type,"请选择视频源类别");
			} else{
				new MessageBox().showEx(slt_src_type,"请选择视频源类别",0,"error");
			}
			return false;
		}
		if((src_type!=pv.TYPES.VUPLOAD.value&&src_type!=pv.TYPES.LETV.value)&&!url||(src_type==pv.TYPES.VUPLOAD.value||src_type==pv.TYPES.LETV.value)&&($(".browse-btn",item).length>0&&!$('.file-upload-content[mode="html5"] .fn',item).attr("selected")||$(".swfupload",item).length>0&&!$('.file-upload-content[mode="flash"] .fn',item).attr("selected"))){
			var url_target = $("input.url",item),timeout = 0,url_msg = "请填写视频源地址";
			if(src_type==pv.TYPES.VUPLOAD.value || src_type==pv.TYPES.LETV.value){
				url_target = $(".upload-wrapper",item);
				url_msg = "请上传文件";
				timeout = 1500;
			}
			if(typeof(emm)!="undefined"){
				emm.showErrMsg(url_target,url_msg,timeout);
			} else{
				new MessageBox().showEx(url_target,url_msg,timeout,"error");
			}
			return false;
		}
		if(pv.submitMode=="single") {
			return {
				url:url,
				src_type:src_type
			};
		}
		var length = $("#multiP_sortable li.item").length;
		var pagename = $("input.pagename",item).val();
		if(pagename==""&&length>0){
			if(mode=="add"&&length>=1){
				$("#multiP_sortable li.item").each(function(i,e){
					var e = $(e);
					if(!$(".pagename",e).val()){
						$(".pagename",e).val("P"+(i+1));
					}
				});
			}
            pagename = mode=="add"?"P"+(length+1):(length>1?"P"+(item.index()+1):"");
        }
		var description = $("textarea.desc",item).val();

		if(description.length>200){
			if(typeof(emm)!="undefined"){
				emm.showErrMsg($("textarea.desc",item),"简介请不要超过200字");
			} else{
				new MessageBox().showEx($("textarea.desc",item),"简介请不要超过200字",0,"error");
			}
			return false;
		}
		var info = {
			url:url,
			src_type:src_type,
			pagename:pagename,
			description:description
		};
		return info;
	},
	checkForm:function(){
		var pv = this;
		var emm = pv.emm;
		var avType;
		emm.init();
		var topIndex = 0;
		function _checkPicSrc(src){
			return true;
		}
		function _checkYouku(){
			var tid = $("#typeid_main").val();
			if(tid!=4&&tid!=36){
				return false;
			}
			return true;
		}
		function _checkQQ(){
			var tid = $("#typeid_main").val();
			var tid_s = $("#typeid").val();
			if(tid!=13&&tid!=36&&tid!=11&&tid_s!=80&&tid_s!=81){
				return false;
			}
			return true;
		}
		function _typeChange(){
			if($(this).hasClass("on")) return;
			for(var i=0;i<emm.msgArray.length;i++){
				if(emm.msgArray[i].needRemove){
					emm.msgArray[i].bindobj.removeClass("error");
					emm.msgArray[i].close();
				} else{
					emm.msgArray[i].resetPos();
				}
			}
		}
		function _setTopIndex(){
			if($("#multiP_sortable").closest("li").index()==0){
				topIndex = emm.error;
			}
		}

		$("#select_arctype button").off("click",_typeChange);
		$("#select_arctype button").on("click",_typeChange);
		if(!$("#select_type_fix").hasClass("on")){
			$("#orig_av").val("");
		}
		if($("#imghead").val()!=""&&!_checkPicSrc($("#imghead").val())){
			emm.showErrMsg($("#imghead"),"图片地址非法");
		} 
		if($("#title").val()==""){
			emm.showErrMsg($("#title"),"标题不能为空");
		} 
		if($("#title").val().length>40){
			emm.showErrMsg($("#title"),"标题字数请控制在40字以内");
		}
		if($("#tags").val()==""){
			emm.showErrMsg($(".tag-input-wrp"),"tag不能为空");
		}
		var tagInput = this.tagInput;
		if(tagInput._disabledValue.length) {
			emm.showErrMsg($("#tag_input_container .tag-input-wrp"), "请删除无效tag，下次别再填了哦(´・ω・｀)", 0, null, 'tags');
		}
		if($("#typeid").val()==0){
			emm.showErrMsg($("#typeid"),"请选择隶属栏目");
		} else {
			avType = $("#typeid").val();
			if ((avType == 83 || avType == 145 || avType == 146 || avType == 147) && (location.hash.indexOf('video_submit') > -1)) {
				if (!pv.codeMode && $("#multiP_sortable li.item").length > 1) {
					emm.showErrMsg($("#typeid"), "在欧美电影、日本电影、国产电影和其他电影分区中不可使用分P投稿", 0, null, 'typeid');
				} else if (pv.codeMode && ($("#body").val().match(/\[vupload\]/g) || []).length > 1) {
					emm.showErrMsg($("#typeid"), "在欧美电影、日本电影、国产电影和其他电影分区中不可使用分P投稿", 0, null, 'typeid');
				}
			}
		}
		if($("#comefrom").val()==""&&!$("#select_type_src").hasClass("on")){
			emm.showErrMsg($("#comefrom"),"请填写视频出处",0,function(){
				emm.removeErr(emm.error);
			});
		} 
		if($("#select_type_fix").hasClass("on")&&$("#orig_av").val()==""){
			emm.showErrMsg($("#orig_av"),"补档请填写补档AV号",0,function(){
				emm.removeErr(emm.error);
			});
		}
		if($("#description").val()==""){
			emm.showErrMsg($("#description"),"请填写简介");
		}
		if($("#description").val().length>200){
			emm.showErrMsg($("#description"),"简介字数不要超过200");
		}
		if(!pv.codeMode&&pv.submitMode=="single"){
			pv.checkVideoItemInfo($("#content_url_1p_page"),"add",emm);
		}
		if(!pv.codeMode&&pv.submitMode=="multi"&&$("#multiP_sortable li.item").length==0){
			_setTopIndex();
			emm.showErrMsg($(".add-video"),"请添加视频源信息",1500);
		}
		if(!pv.codeMode&&pv.submitMode=="multi"&&$("#multiP_sortable li.item.lock").length!=$("#multiP_sortable li.item").length){
			_setTopIndex();
			emm.showErrMsg($("#multiP_sortable li").not(".lock"),"你还有未保存的更改",2000);
		}
		/*if(!pv.codeMode&&$("#lanmu").length>0&&$("#typeid").val()!=0&&$('.slt_src_type option[value="youku"]:selected').length>0&&!_checkYouku()){
			emm.showErrMsg($("#typeid"),"优酷源只允许投游戏区和科技区");
		}*/
		/*if(!pv.codeMode&&$("#lanmu").length>0&&$("#typeid").val()!=0&&$('.slt_src_type option[value="qq"]:selected').length>0&&!_checkQQ()){
			emm.showErrMsg($("#typeid"),"腾讯源只允许投新番区，科技区，影视区和娱乐区的美食分区");
		}*/
		if(!pv.codeMode&&!pv.queryCheck()){
			_setTopIndex();
			emm.showErrMsg($("#multiP_sortable li.uploading"),"有视频正在上传",1500);
		}
		if(!pv.codeMode&&pv.queryCheck()&&$("#multiP_sortable li.uploading").length>0){
			_setTopIndex();
			emm.showErrMsg($("#multiP_sortable li.uploading"),"有未成功上传的视频",2000);
		}
		if(pv.codeMode&&!$("#body").val()){
			emm.showErrMsg($("#body"),"请填写投稿代码");
		}
		if(emm.error!=0){
			emm.msgArray[topIndex].scrollToMsg();
			return false;
		} else{
			if ($("#mission_id").length&&parseInt($("#mission_id").val()) > 0){
				var ms = "";
				var obj = $("#mission_id").get(0);
				for (var i=0;i<obj.options.length;i++)
				{
					if (obj.options[i].selected){
						ms = obj.options[i].text;
						break;
					}
				}
				return confirm("您的投稿将要参加活动："+ms+"，请确认！");
			} else{
				return true;
			}
		}
	},
	bindTypeChange:function(object){
		var pv = this;
		$(".slt_src_type",object).off("change");
		$(".slt_src_type",object).change(function(){
			var sel = $(this);
			object.removeClass("flash");
			if(sel.val()==pv.TYPES.VUPLOAD.value){
				pv.initUploader(object,"html5");
			} else if(sel.val()==pv.TYPES.LETV.value){
				pv.initUploader(object,"flash");
			} else{
				object.removeClass("uploading error");
				$(".upload-wrapper .url",object).show();
				$('.upload-wrapper .file-upload-content',object).remove();
				$('#add_video',object).show();
				$('#upload_video',object).hide();
				if(sel.val()==object.attr("temp_type")){
					$(".url",object).val(object.attr("temp_url"));
				} else{
					$(".url",object).val("");
				}
				$(".edit,.del",object).show();
				$(".start",object).hide();
				$(".info-wrapper",object).remove();
				$(".progress",object).remove();
			}
		});
	},
	buildBodyFromList:function(mode,submit){
		var pv = this;
		var body_msg = "";
		var items;
		if(mode=="multi"||typeof(mode)=="undefined"){
			items = $("#multiP_sortable li.item");
		} else{
			items = $("#content_url_1p_page");
		}
		if(submit){
			var from = "";
			if($("#comefrom").length>0&&$("#description").val().indexOf($("#comefrom").val())!=0){
				from = $("#comefrom").val()+" "; 
			}
			$("#description").val(from+$("#description").val());
		}
		for (var i = 0; i < items.length; i++){
			var vType = $(".slt_src_type",items[i]).val();
			var names = $(".pagename", items[i]).val() ? $(".pagename", items[i]).val() : "P"+(i+1);
			var desc = $(".desc", items[i]).val() ? "[desc]"+$(".desc", items[i]).val()+"[/desc]" : "";
			body_msg+="["+vType+"]"+$(".url",items[i]).val()+"[/"+vType+"]"+desc.replace(/(\n|\r\n)/g," ")+(items.length > 1 ? "#p#"+names+"#e#" : "")+"\n";
		}
		$("#body").val(body_msg);
	},
	buildList:function(){
		var pv = this;
		pv.list = [];
		var items = $("#multiP_sortable li.item");
		for (var i = 0; i < items.length; i++){
			var vType = $(".slt_src_type",items[i]).val();
			var names = $(".pagename", items[i]).val() ? $(".pagename", items[i]).val() : "P"+(i+1);
			var desc = $(".desc", items[i]).val();
			var link = $(".url",items[i]).val();
			pv.list.push({
				desc:desc,
				link:link,
				page:i+1,
				part:names,
				type:vType,
				saved:$(items[i]).data('saved')
			});
		}
	},
	_setLock:function(btn,item){
		var pv = this;
		var uploaded = item.find(".control-btn-select").length == 0;
		var video_type = $(".slt_src_type",item).val();
		if((video_type == pv.TYPES.VUPLOAD.value || video_type == pv.TYPES.LETV.value) && !uploaded){
			//btn.text("取消");
		} else{
			btn.text("编辑");
		}
		item.data('saved',true);
		item.addClass("lock");
		item.find("input,textarea,select").attr("disabled",true);
	},
	_setEdit:function(btn,item){
		var pv = this;
		var uploaded = item.find(".control-btn-select").length == 0;
		var video_type = $(".slt_src_type",item).val();
		if((video_type == pv.TYPES.VUPLOAD.value || video_type == pv.TYPES.LETV.value) && !uploaded){
			//btn.text("上传");
		} else{
			btn.text("保存");
		}
		item.data('saved',false);
		item.removeClass("lock");
		item.find("input,textarea,select").attr("disabled",false);
	},
	_editHandler:function(btn,mode,callback){
		var pv = this;
		var item = btn.closest(".item");
		if(mode=="edit"){
			item.attr("temp_type",$(".slt_src_type",item).val());
			item.attr("temp_url",$(".url",item).val());
			pv._setEdit(btn,item);
			if(callback !== undefined){
				callback(btn);
			}
		} else{
			var checked = pv.checkVideoItemInfo(item,"edit");
			if(checked){
				if($(".pagename",item).val()=="") $(".pagename",item).val(checked.pagename);
				item.attr("temp_type",checked.src_type);
				item.attr("temp_url",checked.url);
				pv._setLock(btn,item);
				if(callback !== undefined){
					callback(btn);
				}
			}
		}
	},
	addP:function(video_link,video_type, description,pagename,saved) {
		var pv = this;	
		var object = null;
		if(typeof(video_link)=="object"){
			object = video_link;
			object.attr("class","item clearfix clickable uploading");
			$("#add_video",object).replaceWith('<a class="start">上传</a><a class="stop">取消</a><a class="edit">保存</a><a class="del">删除</a>');
			$(".edit",object).hide();
			$(".stop",object).show();
			pv.initAddControl();
		} else{
			object = $('<li class="item clearfix clickable">' + 
							'<div class="control-btn-select" style="width: 70px;"><span></span><select class="slt_src_type"><option value="0" disabled="disabled">选择</option></select></div>'+
							'<div class="upload-wrapper"><input class="control-inpt-input url" style="width: 155px;" value="'+video_link+'"/></div>' + 
							'<input class="control-inpt-input pagename" style="width: 95px; margin-left:7px;" value="'+pagename+'"/>' + 
							'<textarea class="control-inpt-input desc auto-size" style="width: 240px; margin-left:7px;">'+(typeof(description)!="undefined"?description:"")+'</textarea>' + 
							'<div class="btns-wrapper"><a class="edit">编辑</a><a class="del">删除</a></div>' + 
						'</li>').insertBefore('#multiP_sortable .add-video');
			pv.initSelectObj(object);
			var type_option = $('.slt_src_type option[value="'+video_type+'"]',object);
			if(type_option.length>0&&(video_type!=pv.TYPES.VUPLOAD.value&&video_type!=pv.TYPES.LETV.value||(video_type == pv.TYPES.VUPLOAD.value ||video_type == pv.TYPES.LETV.value) && !video_link)){
				type_option.attr("selected",true);
				$(".slt_src_type",object).change();
				object.find(".url").val(video_link);
			} else{
				var typeName = video_type;
				if(video_type==pv.TYPES.VUPLOAD.value){
					typeName = pv.TYPES.VUPLOAD.name;
				} else if(video_type==pv.TYPES.LETV.value){
					typeName = pv.TYPES.LETV.name;
				}
				$(".control-btn-select",object).replaceWith('<div class="slt_type_local"><input type="hidden" class="slt_src_type" value="'+video_type+'" />'+typeName+'</div>');
				if(video_type==pv.TYPES.VUPLOAD.value || video_type==pv.TYPES.LETV.value){
					$(".url",object).replaceWith('<div class="url_local" title="'+video_link+'"><input type="hidden" class="url" value="'+video_link+'" />'+video_link+'</div>');
				}
			}
		}
		pv.strCount.bind($(".desc",object),200);
		pv.bindTypeChange(object);

		var editBtn = $(".edit", object);

		if(typeof saved !="undefined"){
			object.data("saved",saved);
			if(!saved || video_type == pv.TYPES.VUPLOAD.value && !video_link){
				$('[value="'+video_type+'"]',object).attr("selected",true);
				$(".slt_src_type",object).change();
				object.find(".url").val(video_link);
				/*if(video_type == pv.TYPES.VUPLOAD.value && pv.uploadModuleName == "html5"){
					var uploadTips = new MessageBox();
					uploadTips.showEx($(".upload-wrapper",object),"重新选择上次的文件，可继续上传",0,"tips");
					$(".upload-wrapper",object).mouseover(function(){
						uploadTips.close();
					});
				}*/
				pv._setEdit(editBtn,object);
			} else{
				pv._setLock(editBtn,object);
			}
		} else{
			pv._setLock(editBtn,object);
		}

		editBtn.click(function(){
			var self = $(this);
			var item = self.closest(".item");
			if(item.hasClass("lock")){
				pv._editHandler(self,"edit");
			} else{
				pv._editHandler(self,"lock");
			}
		});	
		$(".del", object).click(function(){
			object.slideUp(200,function(){
				object.remove();
				pv.buildList();
			});
		});
		$(".control-btn-select",object).find("span").html($("option:selected",object).html());
		return object;
	},
	initVideoList:function(v_list){
		var pv = this;
		for(var i=0;i<v_list.length;i++){
			pv.addP(v_list[i].link,v_list[i].type,v_list[i].desc,v_list[i].part,v_list[i].saved);
		}
		//console.log(v_list);
	},
	infoRecover:function(){
		var pv = this;
		var timer = setInterval(function(){
			clearInterval(timer);
			if(window.localStorage){
				if (window.localStorage.getItem("submit_info") && window.localStorage.getItem("submit_info") != "null" && window.localStorage.getItem("submit_info")!==undefined){
					var info = JSON.parse(window.localStorage.getItem("submit_info"));
					var hasHistory = false;
					for(var i in info){
						if(info[i]){
							if (i != "list" && i != "arctype" && i != "keywords_change" && i != "mission_id" && i != "typeid" || i == "list" && info[i].length != 0) {
								hasHistory = true;
								break;
							}
						}
					}
					if(!hasHistory) return;
					if(confirm("有保存未投稿的数据，是否载入？")){
						pv.manuallyPost();
						for(var k in info) {
							if(k == "list") continue;
							var item = $('[name="'+k+'"]'),
								value = info[k];
							switch(k) {
								case "litpic":
									if(info[k]) {
										$("#imghead").attr("src", info.cover || "http://i0.hdslb.com" + value);
										item.val(value);
									}
									break;
								case "typeid":
									pv.tid = value;
									if(pv.typeData) {
										pv.selectType(pv.typeData, value);
									}
									break;
								case "_arctype":
									if(value) {
										pv.changeArcType(value);
									}
									break;
								case "allow_feed":
									item.attr("checked", value);
									break;
								case "tags":
									pv.tagInput.buildByTags(value);
									break;
								default:
									item.val(value);
							}
						}
						if(!info["keywords_change"]) {
							$("#keywords_change").attr("checked", false);
							$("#keywords_change").change();
						}
						if (info.list){
							pv.initVideoList(info.list);
						}
					}
				}
			}
		}, 100);
		window.rememberTimer = setInterval(function(){
			if(location.hash.indexOf("video_submit")<0){
				clearInterval(rememberTimer);
				return;
			}
			if(window.localStorage){
				pv.buildList();
				var submitInfo = {
					list: pv.list,
					cover: $("#imghead").attr("src"),
					_arctype: $("#select_arctype button.on").attr("id") != "select_type_ref" ? $("#select_arctype button.on").attr("id") : null
				};
				var frmData = $("#frm_SubmitVideo").serializeArray();
				for (var i = 0; i < frmData.length; i++) {
					submitInfo[frmData[i].name] = frmData[i].value;
				}
				//console.log(submitInfo);
				window.localStorage.setItem("submit_info", JSON.stringify(submitInfo));
			}
		}, 1000);
	}
};

function ajaxFrmSubmit(frm){
	if(frm.hasClass("loading")) return;
	var url = frm.attr("action");
	if(url.indexOf("?")>=0) url += "&output=json";
	else url += "?output=json";
	frm.removeAttr("hasMessageBox");
	var loading = new MessageBox({
		focusShowPos: 'down',
		center: false
	});
	loading.showEx(frm,"提交中",0,"loading");
	$.ajax({
		url:url,
		type:"POST",
		dataType:"json",
		data:frm.serialize()+"&sid="+__GetCookie("sid"),
		success:function(data)
		{
			loading.close();
			frm.removeClass("loading");
			if(data==null){
				new FloatMessageBox().show("未知的错误","msg_err");
				return;
			}
			if(data.code==-1){
				new FloatMessageBox().show(data.msg,"msg_err");
			} else if(data.code==0){
				new FloatMessageBox().show(data.msg);
			} else{
				new FloatMessageBox().show(data);
			}
		},
		error:function(){
			loading.close();
			frm.removeClass("loading");
			new FloatMessageBox().show("网络错误,请稍后重试","msg_err");
		}
	});
}

//专题订阅
function spSubscribe(){
	var spParams = {
		url: "/favorite_manage.do?act=spdata&page=",
		target: "#sp_sbs_list",
		render: function(data) {
			return '<li class="item" dyn=' + data.attention + '>' +
				'<a href="http://www.bilibili.com/sp/' + data.title + '" target="_blank">' +
				'<div class="preview"><img src="' + data.cover + '" /></div>' +
				'<div class="t" title="' + data.title + '">' + data.title + '</div>' +
				'</a>' +
				'<a class="ctrl clickable" spid=' + data.spid + '><i class="icon-arrow-down"></i></a>' +
				'</li>';
		},
		renderCallback: function(obj, data) {
			$(".ctrl", obj).click(function(e) {
				e.stopPropagation();
				var btn = $(this);
				var target = btn.parent();
				var spid = btn.attr("spid");
				var dyn = target.attr("dyn");
				if (target.find(".menu").length == 0) {
					var menu = _toggleMenu(target, true, dyn);
					$("li.cancel", menu).click(function(e) {
						e.stopPropagation();
						_toggleMenu(menu, false, dyn);
						new MessageBox().show(btn, "是否确定取消订阅？", "button", function() {
							_postCommand(target, btn, "/favorite_manage.do?act=delsp", {
								spid: spid
							}, function(res) {
								target.slideUp(200, function() {
									target.remove();
								});
							});
						});
						return false;
					});
					$("li.dyn", menu).click(function(e) {
						e.stopPropagation();
						_toggleMenu(menu, false, dyn);
						if (dyn == 1) {
							new MessageBox().show(btn, "是否确定取消推送？", "button", function() {
								_postCommand(target, btn, "/favorite_manage.do?act=inattention_sp", {
									spid: spid
								}, function(res) {
									new MessageBox().show(btn, "已关闭推送功能");
									obj.attr("dyn", 0);
								});
							});
						} else {
							_postCommand(target, btn, "/favorite_manage.do?act=attention_sp", {
								spid: spid
							}, function(res) {
								new MessageBox().show(btn, "已开启推送功能");
								obj.attr("dyn", 1);
							});
						}
						return false;
					});
					closeMenuTimer(menu);
				} else {
					_toggleMenu(null, false, dyn);
				}
			});
		},
		onScroll: function() {
			return _getView('sp').is(':visible');
		},
		showPageAfter: 2,
		pageContainer: "#sp_page",
		state: "#sp_state"
	};
	var lazySpObj = lazyLoadContent(spParams);
	var filter = function(isAttention, isBangumi) {
		if (typeof(isAttention) != "undefined") {
			var isAttention = "&attention=" + isAttention;
		} else {
			var isAttention = "";
		}
		if (typeof(isBangumi) != "undefined") {
			var isBangumi = "&is_bangumi=" + isBangumi + "&bgm_end=0";
		} else {
			var isBangumi = "";
		}
		spParams.url = "/favorite_manage.do?act=spdata" + isAttention + isBangumi + "&page=";
		var ul = $("#sp_sbs_list");
		ul.empty();
		$("#sp_page").empty();
		lazySpObj.abort();
		lazySpObj.page = 0;
		lazySpObj.load();
	};

	var _enableTag = true;
	var _BASE_URL = 'http://www.bilibili.com';
	var $wrp = $('<div class="subscribe-wrp"></div>').appendTo('#main_content');
	var $tabContainer = $('<div class="subscribe-head">'+
		    '<ul class="subscribe-tab">'+
		        '<li class="tab-i" val="tag">标签</li>'+
		        '<li class="tab-i" val="bangumi">剧集</li>'+
		        '<li class="tab-i" val="sp">专题（暂留）</li>'+
		    '</ul>'+
		'</div>').prependTo($wrp);
	var $containers = $('<div class="subscribe-container" view="tag"><ul class="subscribe-list tag clearfix"></ul></div><div class="subscribe-container" view="bangumi"><ul class="subscribe-bangumi-list clearfix"></ul><span id="bangumi_state" class="loading-state"></span></div><div class="subscribe-container" view="sp"></div>').appendTo($wrp).hide();
	$('.top-search, #spForm').appendTo($containers.filter('[view=sp]'));

	var viewTab = TabModule.bind($tabContainer.find('>ul'), {
		onChange: function(item) {
			var view = item.attr('val');
			_switchView(view);
		}
	});

	var _currentView;

	window.tabInsts['sp_subscribe'] = function(view) {
		if(view != _currentView) {
			loadPage(true);
		}
	};

	function _switchView(view, history) {
		$containers.hide();
		_currentView = view;
		var $view = _getView(view);
		if(!$view.length) {
			$view = _getView('tag');
			_currentView = 'tag';
		} else if(history !== true) {
			location.hash = 'sp_subscribe&tab='+view;
		}
		$view.fadeIn(200);
		switch(view) {
			case 'sp':
				if (lazySpObj.options.onData()) {
					lazySpObj.load();
				}
				break;
			case 'bangumi':
				if (lazyBangumiObj.options.onData()) {
					lazyBangumiObj.load();
				}
				break;
			default:
				TagList.load();
		}
	}

	function _getView(view) {
		return $containers.filter('[view=' + view + ']');
	}

	function _toggleMenu(target, toggle, dyn) {
		if(toggle) {
			$(".subscribe-wrp .menu").remove();
			var menu = $('<ul class="menu">' +
				'<li class="cancel">取消订阅</li>' +
				'<li class="dyn' + (dyn === 1 ? "" : " a") + '">' + (dyn === 1 ? "取消实时推送" : "开启实时推送") + '</li>' +
				'</ul>').appendTo(target).hide().fadeIn(300);
			return menu;
		} else {
			if(target) {
				target.fadeOut(300, function() {
					$(this).remove();
				});
			} else {
				$(".subscribe-wrp .menu").fadeOut(300, function() {
					$(this).remove();
				});
			}
		}
	}

	function _command(type, target, btn, url, data, successCallback, errorCallback) {
		$.ajax({
			type: type,
			dataType: "json",
			url: url,
			data: data || {},
			success: function(res) {
				if (res.code == 0) {
					if(successCallback) {
						successCallback(res);
					}
				} else {
					new MessageBox().show(btn, "操作失败", 2000);
				}
			},
			error: function() {
				if(errorCallback) {
					errorCallback();
				} else {
					new MessageBox().show(btn, "网络错误");
				}
			}
		});
	}

	function _postCommand(target, btn, url, data, successCallback, errorCallback) {
		_command("post", target, btn, url, data, successCallback, errorCallback);
	}

	function _getCommand(target, btn, url, data, successCallback, errorCallback) {
		_command("get", target, btn, url, data, successCallback, errorCallback);
	}

	var TagList = (function() {
		var _api = '/api_proxy?app=tag&action=/tags/subscribe';
		var loading = $('<span class="loading-state"><span class="loading-spinner"></span><b>初始化中...</b></span>');
		var _loaded = false;
		var $container = _getView('tag').find('>ul'),
			$nomore = $('<div class="nomore"></div>');
		function load() {
			if(_loaded) {
				return;
			}
			_loaded = true;
			loading.appendTo($container);
			onLoginInfoLoaded(function(info) {
				$.post(_api + '_list/', {}, function(res) {
					loading.remove();
					if (res.code == 0) {
						_render(_initData(res.result));
					} else {
						$nomore.appendTo($container);
					}
				}).error(function() {
					loading.remove();
					$nomore.appendTo($container);
				});
			});
		}

		function _initData(data) {
			data.sort(function(a, b) {
				return b.notify - a.notify;
			});
			return data;
		}

		function _render(data) {
			for(var i=0;i<data.length;i++) {
				var item = _renderItem(data[i]).appendTo($containers.filter('[view=tag]').find('>ul'));
				_bindEvent(item);
			}
		}
		function _renderItem(dataItem) {
			var item = $('<li class="item" dyn=' + dataItem.notify + '>' +
				'<a href="' + _BASE_URL + '/tag/' + dataItem.name + '/" target="_blank">' +
				'<i class="tag-icon"></i><span class="t" title="' + dataItem.name + '">' + dataItem.name + '<div class="notify-on">实时订阅中</div></span>' +
				'</a>' +
				'<a class="ctrl clickable" tagid=' + dataItem.tag_id + '><i class="icon-arrow-down"></i></a>' +
				'</li>');
			if(dataItem.notify) {
				item.addClass('dyn');
			}
			return item;
		}

		function _bindEvent(obj) {
			$(".ctrl", obj).click(function(e) {
				e.stopPropagation();
				_createMenu(obj, $(this));
			});
		}

		function _createMenu(obj, btn) {
			var target = btn.parent();
			var dyn = parseInt(target.attr("dyn"));
			var tagid = btn.attr("tagid");
			if (target.find(".menu").length == 0) {
				var menu = _toggleMenu(target, true, dyn);
				$("li.cancel", menu).click(function(e) {
					e.stopPropagation();
					_toggleMenu(menu, false, dyn);
					new MessageBox().show(btn, "是否确定取消订阅？", "button", function() {
						_postCommand(target, btn, _api + "_cancel/", {
							tag_id: tagid
						}, function(res) {
							target.slideUp(200, function() {
								target.remove();
							});
						});
					});
					return false;
				});
				$("li.dyn", menu).click(function(e) {
					e.stopPropagation();
					_toggleMenu(menu, false, dyn);
					if (dyn == 1) {
						new MessageBox().show(btn, "是否确定取消推送？", "button", function() {
							_postCommand(target, btn, _api + "_add/", {
								tag_id: tagid,
								notify: 0
							}, function(res) {
								new MessageBox().show(btn, "已关闭推送功能");
								obj.attr("dyn", 0);
								obj.removeClass('dyn');
							});
						});
					} else {
						_postCommand(target, btn, _api + "_add/", {
							tag_id: tagid,
							notify: 1
						}, function(res) {
							new MessageBox().show(btn, "已开启推送功能");
							obj.attr("dyn", 1);
							obj.addClass('dyn');
						});
					}
					return false;
				});
				closeMenuTimer(menu);
			} else {
				_toggleMenu(null, false, dyn);
			}
		}

		return {
			load: load
		};
	})();

	var bangumiList = _getView('bangumi').find('>ul');
	function _bangumiErrorHandler() {
		$('#bangumi_state').hide();
		$('.nomore', bangumiList).remove();
		$('<div class="nomore"></div>').appendTo(bangumiList);
	}
	var bangumiParams = {
		url: "/api_proxy?app=bangumi&action=get_concerned_season&pagesize=10&page=",
		target: bangumiList,
		render: function(data) {
			var status = '';
			if(data.is_finish == '0') {
				status = '最近更新至' + data.newest_ep_index + '集';
			} else {
				status = '共' + data.total_count + '集完';
			}
			return '<li class="item">' +
				'<a href="' + _BASE_URL + '/bangumi/i/' + data.season_id + '/" target="_blank">' +
				'<div class="preview"><img src="' + data.cover + '" /></div>' +
				'</a>' +
				'<a href="' + _BASE_URL + '/bangumi/i/' + data.season_id + '/" target="_blank">' +
				'<div class="t" title="' + data.title + '">' + data.title + '</div>' +
				'</a>' +
				'<div class="status">' + status + '</div>' +
				'<a class="del-btn clickable" seasonid=' + data.season_id + '><i class="icon-delete"></i></a>' +
				'</li>';
		},
		renderCallback: function(obj, data) {
			$(".del-btn", obj).click(function(e) {
				e.stopPropagation();
				var btn = $(this);
				var target = btn.parent();
				var seasonid = btn.attr("seasonid");
				new MessageBox().show(btn, "是否确定取消订阅？", "button", function() {
					_getCommand(target, btn, "/api_proxy?app=bangumi&action=/unconcern_season", {
						season_id: seasonid
					}, function(res) {
						target.slideUp(200, function() {
							target.remove();
						});
					});
				});
			});
		},
		onScroll: function() {
			return _getView('bangumi').is(':visible');
		},
		showPageAfter: 2,
		pageContainer: "#bangumi_page",
		state: "#bangumi_state"
	};
	var lazyBangumiObj = lazyLoadContent(bangumiParams);
	lazyBangumiObj.xhrParams = function() {
		var obj = this;
		var options = obj.options;
		return {
			url: options.url + obj.page,
			dataType: 'json',
			success: function(data) {
				if(data.code != 0) {
					return _bangumiErrorHandler();
				}
				obj.render(data.result);
				obj.totalPage = parseInt(data.pages);
				obj.totalResults = parseInt(data.count);
				obj.showPages = typeof(options.showPageAfter) != "undefined" ? Math.ceil(obj.totalPage / options.showPageAfter) : false;

				if (obj._debug) {
					console.log("loaded page: " + obj.page + " Total: " + obj.totalPage + " (URL: " + options.url + obj.page + ")");
				}
				if (obj.page >= obj.totalPage) {
					options.state.find("> .loading-spinner").hide();
					if (options.noDataPrompt) {
						options.state.find("> b").html(options.noDataPrompt);
					} else {
						options.state.hide();
					}
				} else {
					options.state.show();
					options.state.find("> .loading-spinner").show();
					options.state.find("> b").html("查看更多...");

					setTimeout(function() {
						if ($(options.target).height() == 0) return;
						obj.scroll();
					}, 0);
				}
				if ((obj.page % options.showPageAfter == 0 || options.showPageAfter === 1) && !obj.manualLoad) {
					obj.waitManualOperate = true;
					options.state.hide();
				} else {
					$(options.pageContainer).hide();
				}
				if ((obj.page % options.showPageAfter == 0 || obj.page >= obj.totalPage || options.showPageAfter === 1) && obj.showPages > 1) {
					$(options.pageContainer).show();
					obj.showPage();
				}
			},
			error: function() {
				_bangumiErrorHandler()
			}
		};
	};

	function loadPage(history) {
		if(_enableTag) {
			if(Request.hash('view')) {
				viewTab.value('val', Request.hash('view'));
				_switchView(Request.hash('view'), history);
			} else if(Request.hash('tab')) {
				viewTab.value('val', Request.hash('tab'));
				_switchView(Request.hash('tab'), history);
			} else {
				viewTab.value('val', 'tag');
				_switchView('tag', history);
			}
		} else {
			$tabContainer.hide();
			_switchView('sp', history);
		}
	}

	return {
		lazyObj: {
			load: loadPage
		},
		filter:filter
	};
}

window.ErrMsgManage = FormModules.ErrManager;

function checkSpForm(editor_mode,emm){
	emm.init();
	function _checkPicSrc(src){
		return true;
	}
	function _modeChange(){
		if($(this).hasClass("on")) return;
		for(var i=0;i<emm.msgArray.length;i++){
			if(emm.msgArray[i].needRemove){
				emm.msgArray[i].bindobj.removeClass("error");
				emm.msgArray[i].close();
			} else{
				emm.msgArray[i].resetPos();
			}
		}
	}
	function _styleChange(){
		for(var i=0;i<emm.msgArray.length;i++){
			if(emm.msgArray[i].needRemove&&emm.msgArray[i].bindobj.attr("id")== "bangumi_types"){
				emm.msgArray[i].bindobj.removeClass("error");
				emm.msgArray[i].close();
			}
		}
	}
	$(".select-type button").off("click",_modeChange);
	$(".select-type button").on("click",_modeChange);
	$("#bangumi_types button").off("click",_styleChange);
	$("#bangumi_types button").on("click",_styleChange);
	if(!$("#sp_title").val()){
		emm.showErrMsg($("#sp_title"),"标题不能为空");
	} 
	if ($("#is_bangumi").val()==1 && editor_mode=="new"){
		if(!$("#areaid").val()){
			emm.showErrMsg($("#areaid"),"请选择地区",0,emm.removeErr(emm.error));
		}
		if(!$("#bangumi_date").val()){
			emm.showErrMsg($("#bangumi_date"),"请选择播放时间",0,function(){
				emm.removeErr(emm.error);
			});
		}
		if(!$("#id_styleid").val()){
			emm.showErrMsg($("#bangumi_types"),"请选择风格",2000,function(){
				emm.removeErr(emm.error);
			});
		}
	}
	if (editor_mode=="edit"){
		if(!$("#edit_reason").val()){
			emm.showErrMsg($("#edit_reason"),"请填写修改理由");
		}
	}
	if(emm.error!=0){
		emm.msgArray[0].scrollToMsg(emm.firstErr);
		return false;
	}
	return true;
}

function getSWFSettings(data,append_row,pv)
{
	var filename = data.file_name;
	var c_btn = append_row.find(".upload-wrapper");
	var server_ip = data.server_ip;
	var url_data = null;
	
	var last_date = new Date();
	var last_bytes = 0;
	var u_sp = "";
	var request;

	var settings = {
		upload_data:data,
		item:append_row,
		postVideoModule:pv,
		flash_url : "http://static.hdslb.com/images/swfupload/swfupload.swf",
		upload_url: "http://pl.bilibili.tv/",
		file_post_name: "file",
		file_size_limit : "2000 MB",
		file_types : [
			"*.mp4"
		],
		file_types_description : [
			"MP4 files"
		],
		file_upload_limit : 2000,
		file_queue_limit : 0,
		custom_settings : {
			progressTarget : "fsUploadProgress",
			cancelButtonId : "btnCancel"
		},
		debug: false,

		// Button settings
		button_action : SWFUpload.BUTTON_ACTION.SELECT_FILE,
		button_image_url: "http://static.hdslb.com/images/btn/upload_zh_cn_66x20.png",
		button_width: "66",
		button_height: "20",
		button_placeholder_id: "upload_btn_placeholder",
		

		_create:function(){
			var self = this;
			self.item.find("#upload_video").off("click");
			self.item.find("#upload_video").click(function(){
				var mode = "edit";
				if(append_row.hasClass("add-video")){
					mode = "add";
				}
				var checked = self.postVideoModule.checkVideoItemInfo(self.item,mode);
			});
			self.item.find(".start").click(function(){
				var mode = "edit";
				if(append_row.hasClass("add-video")){
					mode = "add";
				}
				var checked = self.postVideoModule.checkVideoItemInfo(self.item,mode);
			});
			self.postVideoModule.upload_query.push(append_row);
		},
		_initEvent:function(self){
			//var self = this;
			self.setUploadURL(data.url);
			self.settings.item.find("#upload_video").off("click");
			self.settings.item.find("#upload_video").click(function(){
				if(self.getStats().files_queued==0){
					new MessageBox().showEx($(self.movieElement),"由于上传组件被重置，请重新选择文件",1500,"error");
					return;
				}
				self.settings._initAddTemplate(self);
			});
			self.settings._initUploadItem(self);
		},
		_initAddTemplate:function(self){
			var mode = "edit";
			if(append_row.hasClass("add-video")){
				mode = "add";
			}
			var checked = pv.checkVideoItemInfo(append_row,mode);
			if (!checked) return;
			append_row.find("#upload_video").remove();
			if(append_row.hasClass("add-video")){
				pv.addP(append_row,pv.TYPES.LETV.value);
				self.startUpload();
				self.settings._setStatus(self,"start");
			}
			append_row.find(".del").off("click");
		},
		_setStatus:function(self,status,text){
			var item = self.settings.item;
			var startBtn = item.find(".start");
			var stopBtn = item.find(".stop");
			var info = item.find(".info-wrapper").show();
			var progressDiv = item.find(".progress");
			var statusDiv = item.find(".status");
			switch(status){
                case "start":
                	self.setButtonDimensions(0, 0);
                    progressDiv.show();
                    stopBtn.show();
                    startBtn.hide();
                    item.addClass("uploading");
                    item.data('uploading',true);
                    statusDiv.text("正在上传...");
                    break;
                case "stop": case "error":
                	self.setButtonDimensions(66, 20);
                    stopBtn.hide();
                    startBtn.show();
                    item.removeClass("uploading");
                    item.data('uploading',false);
                    if(status == "stop"){
                    	statusDiv.text("已暂停");
                    } else{
                    	if(typeof text != "undefined"){
                    		statusDiv.text(text);
                    	} else{
                    		statusDiv.text("上传出错");
                    	}
                    	pv._editHandler(stopBtn,"edit");
                    }
                    break;
                case "done":
                    stopBtn.remove();
                    startBtn.remove();
                    $(".swfupload",item).remove();
                    $(".edit",item).html("编辑").show();
                    $(".control-btn-select",item).replaceWith('<div class="slt_type_local"><input type="hidden" class="slt_src_type" value="'+pv.TYPES.LETV.value+'" />'+pv.TYPES.LETV.name+'</div>');
                    $(".fn",item).addClass("done");
                    item.removeClass("uploading");
                    item.data('uploading',false);
                    item.data('done',true);
                    statusDiv.text("上传成功");
                    break;
                case "delete":
                    break;
                default:;
            }
			pv.queryCheck();
		},
		_initUploadItem:function(self){
			//var self = this;
			append_row.on("click",".del",function(){
				append_row.slideUp(300,function(){
					if(append_row.data('uploading')){
						self.stopUpload();
					}
					$(this).remove();
					pv.buildList();
				});
			});
			append_row.find(".start").off("click");
			append_row.on("click",".start",function(){
				var startBtn = $(this);
				if(self.getStats().files_queued==0){
					new MessageBox().showEx($(self.movieElement),"由于上传组件被重置，请重新选择文件",1500,"error");
					return;
				}
				pv._editHandler(startBtn,"lock",function(){
					self.startUpload();
					self.settings._setStatus(self,"start");
				});
			});
			append_row.on("click",".stop",function(){
				var stopBtn = $(this);
				self.stopUpload();
				pv._editHandler(stopBtn,"edit");
				self.settings._setStatus(self,"stop");
			});
		},
		// The event handler functions are defined in handlers.js
		file_queued_handler : function(file){
			var self = this;
			if(self.getStats().files_queued>1){
				self.cancelUpload();
			}
			$(".fn",append_row).html(file.name).attr("selected",1).attr('title',file.name);
			self.settings._initEvent(self);
		},
		file_queue_error_handler : function(file, errorCode, message) {
			var status = this.settings.item.find(".status");
			try {
				switch (errorCode) {
				case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
					status.html("未选择文件");
					(new MessageBox()).show(c_btn,"文件大小过大，请重新选择",2000,"warning");
					break;
				case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
					status.html("未选择文件");
					(new MessageBox()).show(c_btn,"不允许上传空文件，请重新选择",2000,"warning");
					break;
				case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
					status.html("未选择文件");
					(new MessageBox()).show(c_btn,"文件类型错误，请重新选择",2000,"warning");
					break;
				default:
					(new MessageBox()).show(c_btn,"未知错误："+errorCode+" "+message,2000,"warning");
					break;
				}
			} catch (ex) {
				this.debug(ex);
			}
		},
		file_dialog_complete_handler: function(numFilesSelected, numFilesQueued) {
			if(numFilesSelected>=1){
				//append_row.addClass("uploading");
				new MessageBox().showEx(append_row,"选择文件后以及上传中的列表项会暂时禁用拖动功能",1500,"tips");
			}
		},
		upload_start_handler : function(file){
			var self = this;
			self.settings._setStatus(self,"start");
			window.onbeforeunload = function(){  
			    return "有文件正在上传";     
			};
		},
		upload_progress_handler : function(file, bytesLoaded, bytesTotal) {
			var self = this;
			try {
				var percent = Math.ceil((bytesLoaded / bytesTotal) * 100);

				if (new Date().getTime() - last_date.getTime() > 1000)
				{
					u_sp = (bytesLoaded - last_bytes)/((new Date().getTime() - last_date.getTime())/1000);
					if (u_sp > 1048576)
					{
						u_sp = Math.floor(u_sp / 1048576)+"MB/s";
					}else if (u_sp > 1024)
					{
						u_sp = Math.floor(u_sp / 1024)+"KB/s";
					}else
					{
						u_sp = Math.floor(u_sp)+"B/s";
					}
					last_bytes = bytesLoaded;
					last_date = new Date();
				}
				self.settings.item.find(".progress").progressbar({
					value: percent
				});
				self.settings.item.find(".speed-info").html(u_sp ? " "+u_sp : "");
				self.settings.item.find(".time-info").html(percent+"%").removeAttr("title");
			} catch (ex) {
				this.debug(ex);
			}
		},
		upload_success_handler : function(file) {
			var self = this;
			$.get("/video?act=upload_success&fn="+encodeURIComponent(filename), function(data){
				$(".url",append_row).val(filename+";"+encodeURIComponent(file.name)+";"+server_ip+";");
                $(".url",append_row).attr("fn",file.name);
				self.settings._setStatus(self,"done");
			});
		},
		upload_error_handler : function(file, errorCode, message) {
			var self = this;
			var status = self.settings.item.find(".status");
			try {
				if(errorCode == -280){
					return;
				}
				$.get("/video?act=upload_fail&fn="+encodeURIComponent(filename)+"&code="+errorCode, function(data){
				});
				switch (errorCode) {
					case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
						self.settings._setStatus(self,"error","上传错误: "+message);
						break;
					case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED:
						self.settings._setStatus(self,"error","上传失败");
						break;
					case SWFUpload.UPLOAD_ERROR.IO_ERROR:
						self.settings._setStatus(self,"error","服务器IO错误");
						break;
					case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR:
						self.settings._setStatus(self,"error","安全沙箱错误");
						break;
					case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
						self.settings._setStatus(self,"error","上传限制错误");
						break;
					case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED:
						self.settings._setStatus(self,"error","校验错误");
						break;
					case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
						//self.settings._setStatus(self,"error","上传取消");
						break;
					case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
						self.settings._setStatus(self,"error","上传停止");
						break;
					default:
						self.settings._setStatus(self,"error");
						if(errorCode==-260){
							status.html("请重新选择文件");
							(new MessageBox()).show(c_btn,"请重新选择文件",2000,"warning");
						} else{	
							status.html("未知错误  "+errorCode+" "+message);
							(new MessageBox()).show(c_btn,"未知错误  "+errorCode+" "+message,2000,"warning");
						}
						break;
				}
			} catch (ex) {
				this.debug(ex);
			}
		}
	};
	return settings;
}

define(function (require, exports, module) {
    window.$ = require("jquery");
    var popup = require("popup");
    var mng = require("mng");
    var crypt = require("jsencrypt");
    require("jvalidate")($);
    var captchaUrl = '/captcha';

    _initPage();
    _initEvent();
    
    window.checkPhone = function(item) {
    	var tel = item.val();
    	var country_code = $('#country_code').val();
    	var checked = false;
    	mng.doPost('/register/checkTelFormat', {tel:tel, country_code:country_code}, function (result) {
    		if (result.status == false) {
    			item.attr('jverrortip', result.message);
    		} else {
    			checked = true;
    		}
        });
    	if (!checked) {
    		return false;
    	}
    	return true;
    }
    
    window.checkAgree = function(item) {
    	if (item.is(':checked')) {
    		$('#regSubmit').removeClass('ys-a');
    		return true;
    	} else {
    		$('#regSubmit').addClass('ys-a');
    		return false;
    	}
    }
    
    window.checkCaptch = function(item) {
    	var code = item.val();
    	var tel = $('#new_phone').val();
    	var _return = true;
    	mng.doPost('/register/checkCaptcha', {tel:tel, code:code}, function(result) {
    		if (!result.status) {
    			item.attr('jverrortip', result.message);
    			$('#captchCheck').hide();
    			_return = false;
    		} else {
    			$('#captchCheck').show();
    			$('#captchTip').text('');
    		}
    	});
    	return _return;
    }
    
    window.checkUname = function(item) {
    	var uname = item.val();
    	var _return = true;
    	mng.doPost('/register/checkUname', {uname:uname}, function(result) {
    		if (!result.status) {
    			item.attr('jverrortip', result.message);
    			_return = false;
    		}
    	});
    	return _return;
    }
    
    $("#password").focus(function(){
        $("#safe_window").fadeIn();
        $('#password').keyup();
   });

    //判断密码强度
   $('#password').keyup(function () {
       var __th = $('#password');
       var _r = checkPassword(__th);
       if (_r < 1) {
           Weak();
           return;
       }
       if (_r > 0 && _r < 2) {
           Weak();
       } else if (_r == 2) {
           Medium1();
       }else if (_r == 3) {
           Medium2();
       }else if (_r == 4) {
           Medium3();
       }else if (_r == 5) {
           Tough();
       }    
   });
   
   window.checkPwd = function(item) {
	   var len = item.val().length;
	   if (len < 6) {
			$('#userpwdTip').html('密码不能小于6个字符');
		} else if (len > 16) {
			$('#userpwdTip').html('密码不能大于16个字符');
		} else {
			$('#userpwdTip').html('');
			return true;
		}
	   return false;
   }
   
   //强度函数
   function Weak() {
	   $(".a_pw").empty();
	   $(".a_pw").append("<div class='safe_line bc0001'></div><div class='safe_line e7e7e7e'></div><div class='safe_line e7e7e7e'></div><div class='safe_line e7e7e7e'></div><div class='safe_line e7e7e7e'></div><span style='color:#bc0001'>弱</span>");
	}
	function Medium1() {
		$(".a_pw").empty();
		$(".a_pw").append("<div class='safe_line bc0001'></div><div class='safe_line ff9537'></div><div class='safe_line e7e7e7e'></div><div class='safe_line e7e7e7e'></div><div class='safe_line e7e7e7e'></div><span style='color:#ff9537'>中</span>");
	}
	function Medium2() {
		$(".a_pw").empty();
		$(".a_pw").append("<div class='safe_line bc0001'></div><div class='safe_line ff9537'></div><div class='safe_line ffd800'></div><div class='safe_line e7e7e7e'></div><div class='safe_line e7e7e7e'></div><span style='color:#ffd800'>中</span>");
	}
	function Medium3() {
		$(".a_pw").empty();
		$(".a_pw").append("<div class='safe_line bc0001'></div><div class='safe_line ff9537'></div><div class='safe_line ffd800'></div><div class='safe_line b5dc05'></div><div class='safe_line e7e7e7e'></div><span style='color:#ffd800'>中</span>");
	}
	function Tough() {
		$(".a_pw").empty();
		$(".a_pw").append("<div class='safe_line bc0001'></div><div class='safe_line ff9537'></div><div class='safe_line ffd800'></div><div class='safe_line b5dc05'></div><div class='safe_line c519'></div><span>安全</span>");
	}
	//检测密码内部强度
	function checkPassword(pwdinput) {
		var maths, smalls, bigs, corps, cat, num;
		var str = $(pwdinput).val()
		var len = str.length;
		var cat = /.{16}/g
		if (len == 0) return 1;
		if (len < 6) {
			$('#userpwdTip').html('密码不能小于6个字符');
		} else if (len > 16) {
			$('#userpwdTip').html('密码不能大于16个字符');
		} else {
			$('#userpwdTip').html('');
		}
		cat = /.*[\u4e00-\u9fa5]+.*$/
		if (cat.test(str)) {
			return -1;
		}
		cat = /\d/;
		var maths = cat.test(str);
		cat = /[a-z]/;
		var smalls = cat.test(str);
		cat = /[A-Z]/;
		var bigs = cat.test(str);
		var corps = corpses(pwdinput);
		var num = maths + smalls + bigs + corps;
		if (len < 6) { return 1; }
		if (num == 1) {
			return 2;
		}
		if (num >= 3 && (corps && maths && (smalls || bigs))) {
			return 5;
		}
		if (maths && (smalls || bigs)) {
			return 3;
		} else if (corps && (maths || smalls || bigs)) {
			return 4;
		}
		return;
		if (len >= 6 && len <= 8) {
			if (num == 1) return 1;
			if (num == 2 || num == 3) return 2;
			if (num == 4) return 3;
		}
		if (len > 8 && len <= 11) {
			if (num == 1) return 2;
			if (num == 2) return 3;
			if (num == 3) return 4;
			if (num == 4) return 5;
		}
		if (len > 11) {
			if (num == 1) return 3;
			if (num == 2) return 4;
			if (num > 2) return 5;
		}
	}
   function corpses(pwdinput) {
         var cat = /./g
         var str = $(pwdinput).val();
         var sz = str.match(cat)
         for (var i = 0; i < sz.length; i++) {
             cat = /\d/;
             maths_01 = cat.test(sz[i]);
             cat = /[a-z]/;
             smalls_01 = cat.test(sz[i]);
             cat = /[A-Z]/;
             bigs_01 = cat.test(sz[i]);
             if (!maths_01 && !smalls_01 && !bigs_01) { return true; }
         }
         return false;
     }
   
   var checkSmsCaptcha = function() {
	   $('#captchTip').html('');
	   var _status;
	   mng.doPost('/register/checkSmsCaptcha', {yzm:$('#yzm').val()}, function(rs){
		   if (!rs.status) {
			   $('#yzm').css('color', 'red');
			   _status = false;
			   $('#captchTip').html('验证码错误');
		   } else {
			   $('#yzm').css('color', '');
			   _status = true;
		   }
	   });
	   return _status;
   }
   
    function _initPage() {
        //初始化表单验证组件
        $('form').jValidate({
            blurvalidate: true,
            isbubble: true,
            emptytip: true,
            oncompleted: function (form) {
            	$('#regSubmit').removeClass('ys-a');
                mng.doPost('/register/phone', $(form).serialize(), function(result) {
            		if (!result.status) {
            			popup.alert.show('系统提示', result.message);
            		} else {
            			location.href = 'http://www.bilibili.com/account/register_success';
            		}
                });
            }
        });
        $('#yzm').live('focus', function(){
        	$('#yzm').css('color', '');
        });
        return true;
    }
    
    var refreshCaptcha = function() {
        var rdn = Math.random();
        var tempCaptchaUrl = $("#captchaImg").attr('src');
        var index = tempCaptchaUrl.lastIndexOf('?');
        if (index != -1) {
            tempCaptchaUrl = tempCaptchaUrl.substr(0, index);
        }
        $("#captchaImg").attr("src", tempCaptchaUrl + "?t=" + rdn);
    }
    
    window.canGetCaptchInterval = false;
    window.canGetCaptch = 60;
    
    function _initEvent() {
    	$('#regSubmit').click(function(){
    		if ($(this).hasClass('ys-a')) {
    			return false;
    		}
    	});
    	$('#captchaImg, #changeCaptcha').live('click', function(){
    		refreshCaptcha();
    	});
    	$('#getCaptch').live('click', function() {
    		if (canGetCaptchInterval) {
    			return false;
    		}
    		if (checkPhone($('#new_phone'))) {
				var src = '/captcha?' + Math.random();
				popup.confirm.show('输入验证码', '<img id="captchaImg" src="' + src + '"  style="width:110px;height:30px;margin-bottom:15px;margin-left:10px;"><a href="javascript:void(0);" style="display:block;width:100%;text-align:center;padding-bottom:10px" id="changeCaptcha">换一张</a><input id="yzm" placeholder="请输入图片中的内容" type="text" style="border: 1px solid #DDDDDD;text-transform:uppercase;width:110px;height:20px;">', function(){
    				return sendSmsCaptcha();
    			});    					
    		}
    		return false;
    	});
    }
    
    function sendSmsCaptcha() {
		if (!checkSmsCaptcha()) {
    		return false;
    	}
    	mng.doPost('/register/sendSms', {yzm:$('#yzm').val(), tel:$('#new_phone').val(), country_code:$('#country_code').val()}, function (result) {
			if (!result.status) {
				if ($('#captchaImg').length == 0) {
					$('#getCaptch').click();
					return false;
				}
				refreshCaptcha();
				$('#captchTip').text(result.message);
				return false;
			}
			$('#captchTip').text(result.data);
			$('#getCaptch').removeClass('ys-b').addClass('ys-a');
			canGetCaptchInterval = setInterval(function(){
    			canGetCaptch--;
    			if (canGetCaptch <= 0) {
    				canGetCaptchInterval = clearInterval(canGetCaptchInterval);
    				$('#getCaptch').html('重新获取验证码').removeClass('ys-a');
    				canGetCaptch = 60;
    				return false;
    			}
    			$('#getCaptch').html(canGetCaptch + '秒后重新获取');
    		}, 1000);
		});
    }
});

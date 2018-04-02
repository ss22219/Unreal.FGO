define(function (require, exports, module) {
    window.$ = require("jquery");
    require("jquery-autocomplete");
    var popup = require("popup");
    var qrcode = require('qrcode');
    var mng = require("mng");
    var crypt = require("jsencrypt");
    require("jvalidate")($);
    var captchaUrl = '/captcha';

    var QR = {
        refer: null,
        node: null,
        tick: null,
        show_app_link_tick: null,
        loop: null,
        key: null,
        init: function() {
            // 初始化
            QR.node = $('.content-l');
            // 小电视动画
            var tv = $('#tv'), tick, x = 0;
            $('#app-link').hover(function() {
                clearInterval(tick);
                tick = setInterval(function() {
                    if( x > -720 ) {
                        x -= 80;
                    } else {
                        x = -720;
                        clearInterval(tick);
                    }
                    tv.css('background-position-x', x);
                }, 50);
                $('#download-app').stop(true, true).fadeIn();
            }, function() {
                clearInterval(tick);
                tick = setInterval(function() {
                    if( x < 0 ) {
                        x += 80;
                    } else {
                        x = 0;
                        clearInterval(tick);
                    }
                    tv.css('background-position-x', x);
                }, 50);
                QR.node.attr('status', QR.node.attr('origin-status'));
                clearTimeout(QR.show_app_link_tick);
                QR.show_app_link_tick = setTimeout(function() {
                    $('#download-app').stop(true, true).fadeOut();
                }, 500);
            });
            // 悬浮显示扫码帮助
            $('#login-qrcode').on('mouseenter', function() {
                $('#qr-tips').fadeIn();
                tv.fadeOut();
            });
            $('#qr-tips').on('mouseout', function() {
                $(this).fadeOut();
                tv.fadeIn();
            })
            // 刷新二维码
            $('#qr-reload').on('click', '.reload-btn', function() {
                QR.load();
            });
            // go
            QR.load();
        },
        load: function() {
            // 获取二维码
            $.getJSON('/qrcode/getLoginUrl', function(rs) {
                if( !rs.status ) {
                    return;
                }
                QR.node.attr('status', 'available');
                if( QR.refer ) {
                    QR.refer.clear();
                    QR.refer.makeCode(rs.data.url);
                } else {
                    // 生成二维码
                    QR.refer = new qrcode.QRCode($('#login-qrcode')[0], {
                       'text': rs.data.url,
                       'width': 140,
                       'height': 140
                    });
                }
                // 更新key
                QR.key = rs.data.oauthKey;
                // 检查是否包含gourl
                var redirect = /gourl=([\w\:\/\.]+)/.exec(window.location.search);
                // 过期倒计时
                clearTimeout(QR.tick);
                QR.tick = setTimeout(QR.expire, 1000 * 60 * 3);
                // 轮询
                clearInterval(QR.loop);
                QR.loop = setInterval(function() {
                    $.post('/qrcode/getLoginInfo', {'oauthKey': QR.key, 'gourl': redirect ? redirect[1] : ''}, function(rs) {
                        if( rs.status ) {
                            // 登录完成，跳转
                            window.location.href = rs.data.url;
                        } else if( !rs.status && rs.message == -2 ) {
                            // 当前二维码已过期
                            QR.expire();
                        }
                    }, 'json');
                }, 3000);
            });
        },
        expire: function() {
            clearInterval(QR.loop);
            $('#qr-tips').hide();
            QR.node.attr('status', 'expired');
        }
    }

    _initPage();
    _initEvent();

    function _initPage() {
        //初始化表单验证组件
        $('form').jValidate({
            blurvalidate: true,
            isbubble: true,
            emptytip: true,
            oncompleted: function (form) {
                var passwd = $("#passwdTxt").val();
                passwd = encryptPassword(passwd);
                if (passwd == null) {
                    return false;
                }
                $("#passwdTxt").val(passwd);
                $('#userIdTxt').val($('#userIdTxt').val().trim());
                form.submit();
                return true;
            }
        });

        // 把.footer顶到下面去
        var sh = $(window).height(), footer = $('.footer'), h = footer.outerHeight(), y = footer.offset().top,
            delta = sh - h - y;
        if( delta > 0 ) {
            footer.css('margin-top', delta);
        }
    }

    function _initEvent() {
        //换一换链接
        $("#refreshCaptchaAch").click(refreshCaptcha);

        $("#captchaImg").click(refreshCaptcha);

        //验证码输入框获得焦点时显示验证码
        $("#vdCodeTxt").focus(function () {
            if ($('#captchaImg')[0].style.display == 'none') {
                $("#captchaImg").attr("src", captchaUrl);
                $('#captchaImg').show();
            }
            if ($('#refreshCaptchaAch')[0].style.display == 'none') {
                $('#refreshCaptchaAch').show();
            }
        });
        window._origin_source = ['qq.com', '163.com', '126.com', 'gmail.com', 'sina.com', 'hotmail.com', 'vip.qq.com', 'foxmail.com', 'sina.cn', 'yeah.net', 'sohu.com', 'live.cn', 'outlook.com', 'aliyun.com', 'yahoo.com'];
        window._source =  [];
        $('#userIdTxt').autocomplete({
        	delay : 100,
			search : function(event, ui){
				var _val = $(this).val();
				if (_val.search('@') <= 0) {
					setTimeout(function(){
						$('ul.ui-autocomplete').css('display', 'none');						
					}, 50);
				}
				if (_val.search('@') > 0 && _val.split('@').length == 2) {
					for (var i = 0;i < _origin_source.length;i++) {
						_source[i] = _val.split('@')[0] + '@' +  _origin_source[i];
					}
				}
			},
			autoFocus : true,
			source : _source
		});
        // 初始化二维码
        QR.init();

        $('a[data-sns]').click(function() {
            $.post('/login/' + $(this).data('sns'), {'csrf' : $(this).attr('csrf')}, function(rs) {
                if( !rs.status ) {
                    return popup.alert.show('操作失败', rs.message);
                } else {
                    window.location.href = rs.data;
                }
            }, 'json');
            return false;
        });

        $('input[remember-me]').click(function() {
            if($(this).attr("checked")){
                $("#keeptime").attr("value", $(this).attr("remember-me"));
            }else{
                $("#keeptime").attr("value", $(this).attr("no-remember-me"));
            }
        });

    }

    function refreshCaptcha() {
        var rdn = Math.random();
        var tempCaptchaUrl = $("#captchaImg").attr('src');
        var index = tempCaptchaUrl.lastIndexOf('?');
        if (index != -1) {
            tempCaptchaUrl = tempCaptchaUrl.substr(0, index);
        }
        $("#captchaImg").attr("src", tempCaptchaUrl + "?t=" + rdn);
    }

    /**
     * 加密passwd,返回加密后的passwd，加密失败返回null
     * @param passwd
     * @returns {*}
     */
    function encryptPassword(passwd) {
        if (passwd.length >= 88) {
            return passwd;
        }
        var getKeyUrl = '/login?act=getkey';
        mng.doGet(getKeyUrl, null, function (result) {
            if (result && result.error) {
                popup.alert.show('系统提示', '登录失败，服务端出现异常。');
                passwd = null;
            } else {
                var jscrypt = new crypt.JSEncrypt();
                jscrypt.setPublicKey(result.key);
                var _encPwd = jscrypt.encrypt(result.hash + passwd);
                passwd = _encPwd;
            }
        });
        return passwd;
    }
});

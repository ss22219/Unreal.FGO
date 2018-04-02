define(function (require, exports, modules) {
    require('utils');
    var popup = require('popup');
    var
        $ = require('jquery'),//引入jquery
        mng = {},
    /*默认设置*/
        _defaultAjaxOpt = {
            timeout: 1000 * 60 * 10,//超时
            encodeURI: true,//Unicode编码  get请求默认转换
            cache: true,//缓存
            async: false,//异步
            dataType: "json"//返回值类型
        },
    /*ajax状态码提示*/
        _defaultAjaxMsg = {
            title: "提示",
            sysError: "系统异常",
            permission: "访问被拒绝",
            timeOut: "访问超时",
            notFound: "该访问不存在",
            logTimeOut: "登录超时，请重新登录"
        },
        _defaultDate = {
            format: 'yyyy-MM-dd'
        };

    //公共方法定义区
    $.extend(mng, {
        /*ajax post请求*/
        doPost: function (url, params, callback, options) {
            _defaultAjaxOpt = $.extend(_defaultAjaxOpt, options);
            _ajax("POST", url, params, callback);
        },
        /*ajax get请求*/
        doGet: function (url, params, callback, options) {
            _defaultAjaxOpt = $.extend(_defaultAjaxOpt, options);
            if (_defaultAjaxOpt.encodeURI === true && params != null && params != undefined) {
                params = _encodeParams(params);
            }
            _ajax("GET", url, params, callback);
        },
        /*ajax全局设置*/
        ajaxSet: function (cache) {
            cache = (cache === false ? false : true);
            $.ajaxSetup({
                contentType: "application/x-www-form-urlencoded;charset=utf-8",
                cache: cache,
                complete: function (xhr, ts) {
                    _showAjaxGlobal(xhr);
                }
            });
        },
        /*全选checkbox allButton : 支持传jquery对象 或者 name属性*/
        /*需要被选中的checkbox checkedButton name属性*/
        checked: function (allButton, checkedButton) {
            if (_isBlank(allButton)) {
                throw("allButton can't be undefined or null");
            }
            if (_isBlank(checkedButton)) {
                throw("checkedButton can't be undefined or null");
            }
            if (_getObjType(allButton) === "string") {
                var _allButton$ = $("input[name=" + allButton + "]");
            } else {
                var _allButton$ = $(allButton);
            }

            if (_allButton$.is("checkbox")) {
                _allButton$.bind('click', function () {
                    var column = $("input[name=" + checkedButton + "][type=checkbox]");
                    if (this.checked) {
                        column.each(function () {
                            this.checked = true;
                        });
                    } else {
                        column.each(function () {
                            this.checked = false;
                        });
                    }
                });
            } else {
                var checked = false;
                _allButton$.bind('click', function () {
                    var column = $("input[name=" + checkedButton + "][type=checkbox]");
                    if (checked) {
                        column.each(function () {
                            this.checked = false;
                        });
                        checked = false;
                    } else {
                        column.each(function () {
                            this.checked = true;
                        });
                        checked = true;
                    }
                });
            }
        },
        /*判断 是否为null、空白、undefined*/
        isBlank: function (value) {
            return _isBlank(value);
        },
        /*判断 是否为null、空白、undefined*/
        isNotBlank: function (value) {
            return _isNotBlank(value);
        },
        /*校验是否为email格式*/
        checkEmail: function (email) {
            var emailReg = /^[a-zA-Z0-9_@\.-]+@([a-zA-Z0-9_-]+\.)+[a-zA-Z0-9_-]{2,3}$/;
            return emailReg.test(item.val());
        },
        /*校验是否为手机号码*/
        checkMobile: function (mobile) {
            var mobileReg = /^1[3,5,8]{1}[0-9]{1}[0-9]{7}[0-9]$/;
            return mobileReg.test(item.val());
        },
        checkIE678: function () {
            return _checkI678();
        },
        //将yyyy-MM-dd格式的时间字符串格式化成日期
        getDate: function (dateStr) {
            if (_checkIE678()) {
                //兼容IE678
                if (value.indexOf("-") > 0) {
                    var vals = value.split("-");
                    var year = vals[0];
                    var month = vals[1] - 1;
                    var day = vals[2];
                    return new Date(year, month, day);
                }
            } else {
                return new Date(value);
            }
        }

    });

    //内部方法定义区
    var
        _ajax = function (type, url, params, callback) {
            $.ajax({
                type: type,
                url: url,
                data: params,
                dataType: _defaultAjaxOpt.dataType,
                async: _defaultAjaxOpt.async,
                cache: _defaultAjaxOpt.cache,
                timeout: _defaultAjaxOpt.timeout,
                success: function (obj) {
                    if (obj && obj.error) {
                        $.popup.alert.show(_defaultAjaxMsg.title, obj.error, null);
                        return;
                    }
                    if (callback) {
                        callback(obj);
                    }
                }
            });
        },
    /*ajax登录超时和异常提示*/
        _showAjaxGlobal = function (xhr) {
            if (xhr.responseText == undefined && xhr.status == undefined) {
                popup.alert.show(_defaultAjaxMsg.title, _defaultAjaxMsg.sysError, null);
            }
            if (xhr.status == 302) {
                popup.alert.show(_defaultAjaxMsg.title, _defaultAjaxMsg.logTimeOut, function () {
                    var url = location.href;
                    location.href = url;
                });
            }
            if (xhr.status == 500) {
                popup.alert.show(_defaultAjaxMsg.title, _defaultAjaxMsg.sysError, null);
            }
            if (xhr.status == 404) {
                popup.alert.show(_defaultAjaxMsg.title, _defaultAjaxMsg.notFound, null);
            }
            if (xhr.status == 403) {
                popup.alert.show(_defaultAjaxMsg.title, _defaultAjaxMsg.permission, null);
            }
            if (xhr.status == 408) {
                popup.alert.show(_defaultAjaxMsg.title, _defaultAjaxMsg.timeOut, null);
            }
        },
        _isBlank = function (value) {
            if (value == undefined) {
                return true;
            }
            if (value == null) {
                return true;
            }
            if ($.trim(value).length <= 0) {
                return true;
            }
            return false;
        },
        _isNotBlank = function (value) {
            return !_isBlank(value);
        },
        _encodeParams = function (params) {
            if (_isNotBlank(params) && (typeof params).toLowerCase() === 'string') {
                if (params.indexOf("&") != -1) {
                    var paramsArray = params.split("&");
                    params = "";
                    for (var i = 0; i < paramsArray.length; i++) {
                        if (_isNotBlank(paramsArray[i])) {
                            params += _encodeParam(paramsArray[i]);
                            if (i < paramsArray.length - 1) {
                                params += "&";
                            }
                        }
                    }
                } else {
                    params = _encodeParam(params);

                }
            }
            return params;
        },
        _encodeParam = function (params) {
            var rst = "";
            rst = params.substring(0, params.indexOf("="));
            rst += "=";
            rst += encodeURIComponent(params.substring(params.indexOf("=") + 1));
            return rst;
        },

        _checkI678 = function () {
            var ieMode = document.documentMode;//IE独有的属性 文档模式
            var isIE = !!window.ActiveXObject;
            var isIE6 = isIE && !window.XMLHttpRequest;
            var isIE7 = isIE && !isIE6 && !ieMode || ieMode == 7;
            var isIE8 = isIE && ieMode == 8;
            return isIE6 || isIE7 || isIE8;
        },
    //获取obj对象的类型 返回类型小写字符串
        _getObjType = function (obj) {
            return (typeof obj).toLowerCase();
        },
    //校验obj是否是string or date
        _isStrOrDate = function (obj) {
            return _getObjType(obj) === "string" || _getObjType(obj) === "date";
        },
    //将字符串或者日期格式化成yyyy-MM-dd类型
        _formatDate = function (date) {
            if (_getObjType(date) === "string") {
                return date;
            }
            if (_getObjType(date) === "date") {
                return date.format(_defaultDate.format);
            }
        };

    modules.exports = mng;
    mng.ajaxSet();//默认调用全局ajax设置
});

define(function(){
    return function(jQuery) {

/**
Name    : jValidate
Author  : kingthy
Email   : kingthy@gmail.com
Blog    : http://www.cnblogs.com/kingthy/
Version : 1.0.3
License : MIT,GPL licenses.

Sample:
---------------------------------------------
sample 1 :
    jQuery('form').jValidate();

sample 2 :
     jQuery('form').jValidate({
		 blurvalidate : true,
		 isbubble : false,
		 emptytip : true
	 });
sample 2 :
     jQuery('form').jValidate({
		 blurvalidate : false,
		 isbubble : false,
		 emptytip : true,
		 onerror  : function(item){
			 alert(item.attr('jverrortip'));
		 },
		 oncompleted : function(form){
			 //submit form
			 return true;
		 }
	 });
**/
if(typeof(jQuery) != 'undefined')(function($){
	$.fn.extend(
		{
			jValidate : function(settings){
				settings = $.extend({
					isbubble : false,
					blurvalidate : false,
					emptytip : false,
					validation : false,
                    validationGroup : '',
					validationOnSubmit : true,
					focusOnError : true,
					focusOnBlurError : false,
                    autotrim : true,
					oncorrect : function(item, form){
						return _setTip(item, form, true);
					},
					onerror : function(item, form){
						return _setTip(item, form, false);
					},
					oncompleted : null
				},settings);

				var _needFocus = false;
				var _isFocusOn = false;
				this.each(function(){
                    var formObj = $(this);
                    if(formObj.attr('jveventinit') != '1'){
                        formObj.attr('jveventinit', '1');
                        var controls = $(':input', formObj);
                        controls.filter('[jvdefault]').focus(function(){
                            var item = $(this);
                            var defaultValue = item.attr('jvdefault');
                            if(defaultValue && defaultValue == item.val()){
                                item.val('');
                                item.removeClass('jv-box-default');
                            }
                        }).blur(function(){
                            var item = $(this);
                            var defaultValue = item.attr('jvdefault');
                            if(defaultValue && item.val() == ''){
                                item.val(defaultValue);
                                item.addClass('jv-box-default');
                            }
                        }).each(function(){
                            var item = $(this);
                            var defaultValue = item.attr('jvdefault');
                            if(defaultValue && item.val() == defaultValue){
                                item.addClass('jv-box-default');
                            }                            
                        });
                        controls.filter('[jvtipid]').keypress(function(){
                            var item = $(this);
                            var tipid = item.attr('jvtipid');
                            var errorclass = item.attr('jverrorclass');
                            if(errorclass)item.removeClass(errorclass);

                            if(tipid){
                                var tipobj = $('#' + tipid, this.form);
                                if(tipobj && tipobj.attr('jvnormaltip') != undefined){
                                    var normalclass = tipobj.attr('jvnormalclass');
                                    var correctclass = tipobj.attr('jvcorrectclass');
                                    var errorclass = tipobj.attr('jverrorclass');
                                    if(normalclass)tipobj.addClass(normalclass);
                                    if(errorclass)tipobj.removeClass(errorclass);
                                    if(correctclass)tipobj.removeClass(correctclass);
                                    tipobj.html(tipobj.attr('jvnormaltip'));
                                }
                            }
                        });
                        if(settings.blurvalidate){
                            controls.change(function(){
                                _needFocus = settings.focusOnBlurError;
                                _isFocusOn = false;
                                _isCorrect($(this), this.form);
                            });
                        }
                    }
				});

				function _setTip(item, form, isCorrect){
					var tipid = item.attr('jvtipid');
					var errortip = item.attr('jverrortip');
					var correcttip = item.attr('jvcorrecttip');
					var unfocuson = item.attr('jvfocuson');

                    var errorclass = item.attr('jverrorclass');
                    if(errorclass){
                        if(isCorrect){
                            item.removeClass(errorclass);
                        }else{
                            item.addClass(errorclass);
                        }
                    }

					var tip = isCorrect ? correcttip : errortip;
					unfocuson = (unfocuson && (unfocuson.toLowerCase() == 'false' || unfocuson == '0'));
					if(tipid){
						var tipobj = $('#' + tipid, form);
						if(tipobj.length > 0){
							var normalclass = tipobj.attr('jvnormalclass');
							var correctclass = tipobj.attr('jvcorrectclass');
							var errorclass = tipobj.attr('jverrorclass');
							if(tipobj.attr('jvnormaltip') == undefined){
								tipobj.attr('jvnormaltip', tipobj.html())
							}
							if(normalclass)tipobj.removeClass(normalclass);
							if(isCorrect){
								if(errorclass)tipobj.removeClass(errorclass);
								if(correctclass && (settings.emptytip || tip))tipobj.addClass(correctclass);
							}else{
								if(errorclass && (settings.emptytip || tip))tipobj.addClass(errorclass);
								if(correctclass)tipobj.removeClass(correctclass);
							}
							if(tip){
								tipobj.html(tip);
							}else{
								tipobj.html(tipobj.attr('jvnormaltip'));
							}
						}else if(tip){
							alert(tip);
						}
					}else if(tip){
						alert(tip);
					}
					if(!unfocuson && _needFocus && !isCorrect && !_isFocusOn){
						item.focus();
						_isFocusOn = true;
					}
					return true;
				}
				function _isCorrect(item, form){
					var jvpattern = item.attr('jvpattern');
					var compareid = item.attr('jvcompareid');
					var method = item.attr('jvmethod');
					var required = item.attr('jvrequired');
                    var defaultValue = item.attr('jvdefault');

					if(!(jvpattern || compareid || method || defaultValue))return true;

					var f = false, p = false;
					var val = item.val();
                    if(settings.autotrim && "false" != item.attr('jvtrim')){
                        if(!this.disabled && val != undefined && val.length > 0){
                            if("TEXTAREA" == item[0].tagName || ("INPUT" == item[0].tagName && item.attr('type') == 'text')){
                                var trimVal = val.replace(/^\s+|\s+$/g,"");
                                if(trimVal.length != val.length){
                                    val = trimVal;
                                    item.val(trimVal);
                                }
                            }
                        }
                    }
                    if(defaultValue){
                        if(val == defaultValue){
                            val = '';
                            item.val('');
                        }
                    }
					if(required && (required.toLowerCase() == 'false' || required == '0')){
						if(val == '')f = true;
					}else if(this.disabled || val == undefined){
						p = true;
						f = false;
					}
					if(!p && !f){
						f = true;
						if(jvpattern && f){
							if(("TEXTAREA" == item[0].tagName || ("INPUT" == item[0].tagName && item.attr('type') == 'hidden')) && jvpattern.indexOf('.') != -1){
								jvpattern = jvpattern.replace(/\./g,function(item, index, text){
									if(index == 0 || text.charAt(index - 1) != '\\'){
										return "(?:.|\\r|\\n)";
									}else{
										return item;
									}
								});
							}
							var r = new RegExp(jvpattern,"i");
							if(!r.test(val))f = false;
						}
						if(compareid && f){
							var co = $('#' + compareid, form);
							if(co.length > 0 && co.val() != val)f = false;
						}
						if(method && f){
							f = window[method](item);
							//eval('f=' + method + '(item);');
							if(typeof(f) == 'object' && f.message){
								item.attr('jverrormessage', item.attr('jverrortip'));
								item.attr('jverrortip', f.message)
								f = f.result;
							}else{
								if(item.attr('jverrormessage'))
									item.attr('jverrortip',item.attr('jverrormessage'));
							}
						}
					}
					if(f){
						if(val != '' && val != undefined){
							settings.oncorrect(item,form);
						}
					}else{
						settings.onerror(item,form);
					}
					return f;
				}
				function _validate(form){
					var success = true;
					_isFocusOn = false;
					_needFocus = settings.focusOnError;
					$(':input', form).each(function(){
                        var o = $(this);
                        if(settings.validationGroup){
                            if(settings.validationGroup != o.attr('jvgroup')) return;
                        }
						if(!_isCorrect(o, form)){
							success = false;
							if(!settings.isbubble)return false;
						}
					});
					if(success && settings.oncompleted)success = settings.oncompleted(form);
					return success;
				}
				if(settings.validationOnSubmit){
					this.submit(function(){
						return _validate(this);
					});
				}

				if(!settings.validation){
					return this;
				}else{
					var f = true;
					this.each(function(){
						if(!_validate(this))f = false;
					});
					return f;
				}
			}
		}
	)
})(jQuery);

    }
});
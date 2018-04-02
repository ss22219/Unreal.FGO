String.prototype.trim = function (){
    if(this.length == 0) return this;
    return this.replace(/^\s+|\s+$/g,"");
}

String.prototype.replaceAll = function(reallyDo, replaceWith, ignoreCase){
    if (!RegExp.prototype.isPrototypeOf(reallyDo)) {
        return this.replace(new RegExp(reallyDo, (ignoreCase ? "gi" : "g")), replaceWith);
    } else {
        return this.replace(reallyDo, replaceWith);
    }
}

String.prototype.htmlEncode = function (){
	if(this.length == 0)return this;
	var span = jQuery('<span></span>');
    span.text(this.toString());
    var text = span.html();
    span = null;
    return text;
}

String.prototype.javaStringEncode = function (){
	if(this.length == 0)return this;
	var chars = null;
	for(var i=0; i<this.length; i++){
		var c = this.charAt(i);
		if(c == '"' || c == "'" || c == '\\' || c == '\r' || c == '\n'){
			if(chars == null){
				chars = [];
				if(i > 0)chars.push(this.substring(0, i));
			}
			if(c == '\r'){
				chars.push('\\r');
			}else if(c == '\n'){
				chars.push('\\n');
			}else{
				chars.push('\\');
				chars.push(c);
			}
		}else if(chars != null){
			chars.push(c);
		}
	}
	return chars == null ? this : chars.join('');
}

function StringBuffer(){
    this.__strings__ = new Array;
}

StringBuffer.prototype.append = function(str){
    this.__strings__.push(str);
    return this;
}

StringBuffer.prototype.toString = function(){
    return this.__strings__.join("");
}

Array.prototype.pushArray = function (addArray){
    if(addArray == null || addArray.length == 0) return this;

    for(var index = 0; index < addArray.length; index ++){
        this.push(addArray[index]);
    }
    return this;
}

Date.prototype.format = function(format){
    var o = {
        "M+" : this.getMonth()+1, //month
        "d+" : this.getDate(), //day
        "h+" : this.getHours(), //hour
        "m+" : this.getMinutes(), //minute
        "s+" : this.getSeconds(), //second
        "q+" : Math.floor((this.getMonth()+3)/3), //quarter
        "S" : this.getMilliseconds() //millisecond
    }

    if(/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear()+"").substr(4 - RegExp.$1.length));
    }

    for(var k in o) {
        if(new RegExp("("+ k +")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length==1 ? o[k] : ("00"+ o[k]).substr((""+ o[k]).length));
        }
    }
    return format;
}
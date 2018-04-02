/*
<div id="list">
  <div class="nodate" style="display:none">
    没有数据
  </div>
  <div>
    name:{name}
  </div>
</div>
<script>
//POST /member/list -> {code:'00', message:'success', list:[name:'...']}

api.postBind({active : 1},'/member/list','list',function(){
  console.log('success');
},function(){
  console.log('nodata');
});
</script>
gool 2016.5.7
*/
var api = {
    templates: {},
    baseUrl: 'http://localhost:22080/'
};
/**
*初始化模版
*/
api.initTemplate = function (id) {
    if (!id)
        $('.template').each(function () {
            $this = $(this), id = this.id;
            if (!api.templates[id])
                api.initTemplate(id);
        });
    else {
        $this = $('#' + id);
        var nodata = $('.nodata', $this).prop('outerHTML');
        $('.nodata', $this).remove();
        api.templates[id] = {
            nodata: nodata,
            content: $('#' + id).html()
        };
        $this.empty();
        return api.templates[id];
    }
}
$(function () {
    api.initTemplate();
});

/**
*绑定列表 id:模版id
*/
api.bindList = function (id, list) {
    var template = api.templates[id], result = '';
    if (!template)
        template = api.initTemplate(id);
    for (var i = 0; i < list.length ; i++) {
        result += api.format(template.content, list[i]);
    }
    $('#' + id).html(result);
}

/**
*占位符替换
*/
api.format = function (template, model) {
    for (var k in model) {
        template = template.replace(new RegExp("\\{" + k + "\\}", 'g'), model[k]);
    }
    return template;
}
api.submit = function (callback, selector) {
    var form = $(selector ? selector : 'form');
    var parms = $(selector ? selector : 'form').controlJson();
    api.post(parms, form.attr('action'), callback);
}
/**
*请求api url:相对路径
*/
api.post = function (parms, url, success, nodata) {
    $('.api_mode').remove();
    $('body').append('<div class="api_mode" style="z-index: 99999;background: rgba(0,0,0,0.5);position: fixed;top: 0;left: 0;width: 100%;height: 100%;"><p style="position: fixed;top: 50%;left: 50%;color: white;line-height: 0; ">loading...</p></div>')
    $.ajax({
        type: "POST",
        async: true,
        cache: false,
        url: api.baseUrl + url,
        data: parms,
        success: function (res) {
            $('.api_mode').remove();
            var resObj = typeof res == "string" ? JSON.parse(res) : res;
            if (resObj.code != "00") {
                if (resObj.code == "02" && nodata) {
                    nodata();
                    return;
                }
                alert(resObj.msg || resObj.message);
            } else {
                if (success)
                    success(resObj);
            }
        },
        error: function () {
            $('.api_mode').remove();
            alert("网络异常，请稍后再试");
        }
    });
}

/**
*   请求api并且绑定到html模板中 dataCallback是对数据的加工处理,nodata是列表为空的时候处理
*/
api.postBind = function (parms, url, id, dataCallback, nodata) {
    api.post(parms, url, function (resObj) {
        if (dataCallback)
            dataCallback(resObj);
        api.bindList(id, resObj.list);
    }, function () {
        $('#' + id).html(api.templates[id].nodata);
    });
}

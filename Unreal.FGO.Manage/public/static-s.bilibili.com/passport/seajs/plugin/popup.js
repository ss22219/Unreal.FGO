define(function(require){
    var art = require("artDialog");
    //default setting
    (function (d) {
        d['okValue'] = '确定';
        d['cancelValue'] = '取消';
        d['title'] = '消息';
        d['lock'] = true;
        d['esc'] = false;
        d['focus'] = false;
    })(art.dialog.defaults);
    //export popup for seajs
    return {
        loading : {
            show : function(text){
                text = '<div><img src="/public/seajs/plugin/dialog/skins/loading.gif" align="absmiddle">' + (text || '正在处理，请稍候..') + '</div>';
                if(art.dialog.list['art_loading']){
                    art.dialog.list['art_loading'].visible();
                    art.dialog.list['art_loading'].content(text);
                }else{
                    art.dialog({id:'art_loading', title:false, cancel:false, content:text, height: '30px',padding:'5px'});
                }
            },
            hide : function(){
                if(art.dialog.list['art_loading']){
                    art.dialog.list['art_loading'].hidden();
                }
            }
        },
        alert : {
            show : function(title, content, ok){
                art.dialog({title : '<i class="topIcon"></i>' + title,
                    cancel : false,
                    content : content,
                    ok : function(){
                        if(ok)return ok();
                    }
                });
            }
        },
        confirm : {
            show : function(title, content, ok, cancel, okvalue, cancelvalue){
                art.dialog({title : '<i class="topIcon"></i>' + title,
                    cancel : function(){
                        if(cancel)return cancel();
                    },
                    content : content,
                    ok : function(){
                        if(ok)return ok();
                    },
                    okValue : okvalue ? okvalue : '确定',
            		cancelValue : cancelvalue ? cancelvalue : '取消'
                });
            }
        }
    }
});
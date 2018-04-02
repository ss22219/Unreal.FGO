var alertTemplate = '<div id="alert_mod"><div style="display: block;position: fixed;visibility: visible;z-index: 1991;left: 50%;top: 50%;transform: translate(-50%,-50%);"><div class="d-outer   d-state-focus d-state-lock d-state-visible"><table class="d-border"><tbody><tr><td class="d-nw"></td><td class="d-n"></td><td class="d-ne"></td></tr><tr><td class="d-w"></td><td class="d-c"><div class="d-inner"><table class="d-dialog"><tbody><tr><td class="d-header"><div class="d-titleBar"><div class="d-title" style="display: block;"><i class="topIcon"></i>提示</div><a class="d-close" href="javascript:/*artDialog*/;" style="display: none;">×</a></div></td></tr><tr><td class="d-main" style="width: auto; height: auto;"><div class="d-content" style="padding: 20px 25px;">该访问不存在</div></td></tr><tr><td class="d-footer"><div class="d-buttons"><input type="button" class="d-button d-state-highlight" value="确定"></div></td></tr></tbody></table></div></td><td class="d-e"></td></tr><tr><td class="d-sw"></td><td class="d-s"></td><td class="d-se"></td></tr></tbody></table></div></div><div d-mask"="" style="z-index: 1990;position: fixed;left: 0px;top: 0px;width: 100%;height: 100%;overflow: hidden;display: block;background: rgba(1, 1, 1, 0.5);"></div></div>';
window.alert = function (msg) {
    if (!window.alert.dom) {
        window.alert.dom = $(alertTemplate)
        $('body').append(window.alert.dom)
        window.alert.dom.find('.d-button').click(function () {
            window.alert.dom.css('display', 'none')
        })
    }
    window.alert.dom.find('.d-content').text(msg)
    window.alert.dom.css('display', 'block')
}

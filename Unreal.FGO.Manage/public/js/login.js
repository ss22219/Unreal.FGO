$.fn.validate.errorTemplate = '<p class="yzm_x mar_t_56" id="userpwdTip">{msg}</p>';
var handler = function () {
    if ($('from').isValidate())
        $('#regSubmit').removeClass("ys-a")
    else
        $('#regSubmit').addClass("ys-a")
};
$('[data-rule]').keyup(handler);
$('[data-rule]').blur(handler);
$(':submit').mouseover(handler);
$.validate.validateHandler = function (msg) {
    if (msg.isValidate)
        $(this).data('errorEl') && $(this).data('errorEl').remove();
    else
        $(msg.messages).each(function () {
            var el = this.element;
            el.data('errorEl') && el.data('errorEl').remove();
            el.data('errorEl', $($.validate.errorTemplate.replace(/\{msg\}/, this.message)));
            el.parent().after(el.data('errorEl'));
            return false;
        });
};
$('form').submit(function () {
    if ($(this).validate().isValidate)
        api.submit(function (res) {
            sessionStorage.setItem('token', res.data)
            location.href = "/"
        })
})
/*
 * HTML5Uploader
 * https://github.com/filad/html5Uploader
 *
 * Copyright 2014, Adam Filkor
 * http://filkor.org
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint nomen: true, regexp: true */
/*global define, window, URL, webkitURL, FileReader */


(function (factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // Register as an anonymous AMD module:
        define([
            'jquery',
            'tmpl',
            './jquery.fileupload-resize',
            './jquery.fileupload-validate'
        ], factory);
    } else {
        // Browser globals:
        factory(
            window.jQuery,
            window.Handlebars
        );
    }
}(function ($, Handlebars) {
    'use strict';

    $.blueimp.fileupload.prototype._specialOptions.push(
        'filesContainer',
        'postVideoModule',
        'requestData',
        'uploadItem'
    );

    // The UI version extends the file upload widget
    // and adds complete user interface interaction:
    $.widget('filkor.html5Uploader', $.blueimp.fileupload, {

        options: {
            // By default, files added to the widget are uploaded as soon
            // as the user clicks on the start buttons. To enable automatic
            // uploads, set the following option to true:
            autoUpload: false,
            
            maxFileSize: 2048*1024*1024,
            //chunking uploads by default
            maxChunkSize: 1024*512, // 0.5MB
            // The container for the list of files. If undefined, it is set to
            // an element with class "files" inside of the widget element:
            filesContainer: '#multiP_sortable',
            //
            postVideoModule: null,
            //
            requestData: null,
            //
            uploadItem:null,
            // The expected data type of the upload response, sets the dataType
            // option of the $.ajax upload requests:
            dataType: 'json',

            // Function returning the current number of files,
            // used by the maxNumberOfFiles validation:
            getNumberOfFiles: function () {
                return this.filesContainer.children().not(".add-video").length;
            },

            // The add callback is invoked as soon as files are added to the fileupload
            // widget (via file input selection, drag & drop or add API call).
            add: function (e, data) {
                var $this = $(this),
                    that = $this.data('filkor-html5Uploader'),
                    options = that.options,
                    files = data.files,
                    file = files[0],
                    existingFiles = options.existingFiles || [];
                if (typeof options.postVideoModule.allowLocal == "object") {
                    var support = false;
                    for (var i = 0; i < options.postVideoModule.allowLocal.file_types.length; i++) {
                        var type = options.postVideoModule.allowLocal.file_types[i].replace("*", "").toLowerCase();
                        var typeReg = new RegExp(type + "$","i");
                        if (files[0].name.match(typeReg) != null) {
                            support = true;
                            break;
                        }
                    }
                    if (!support) {
                        new MessageBox().showEx($this.find(".fn"), "不支持该文件格式，请选择指定的格式", 2000, "error");
                        return;
                    }
                } else {
                    if (files[0].name.match(/.flv$/i) == null) {
                        new MessageBox().showEx($this.find(".fn"), "不支持该文件格式，请选择flv格式", 2000, "error");
                        return;
                    }
                }
                if (file.size > options.maxFileSize) {
                    new MessageBox().showEx($this.find(".fn"), "文件大小超过限制( 2G )", 2000, "error");
                    return;
                }
                data.process(function () {
                    return $this.html5Uploader('process', data);
                }).always(function () {
                    $(".fn",$this).text(files[0].name).attr("selected",1).attr("title",files[0].name);
                    options.uploadItem.data('data',data);
                    if(!options.uploadItem.hasClass("add-video")){
                        that._addTemplate(e,data);
                    }
                });
            },

            // Callback for the start of each file upload request:
            send: function (e, data) {
                var that = $(this).data('filkor-html5Uploader');
                data.context.find('.resumed-upload-note').fadeOut();
                if (!$.support.transition) {
                    that.options.uploadItem.find('.progress').hide();
                }
                return that._trigger('sent', e, data);
            },

            // Callback for successful uploads:
            done: function (e, data) {
                var that = $(this).data('filkor-html5Uploader'),
                getFilesFromResponse = data.getFilesFromResponse ||
                    that.options.getFilesFromResponse,
                files = getFilesFromResponse(data),
                progressbar = that.options.uploadItem.find('.progress'),
                file = files[0] || {error: 'Empty file upload result'};
                
                $(".url",that.options.uploadItem).val(that.options.requestData.file_name+";"+encodeURIComponent(file.name)+";"+that.options.requestData.server_ip+";");
                $(".url",that.options.uploadItem).attr("fn",file.name);
                that._setStatus(e, "done");

                that.options.uploadItem.data('uploading',false);
                if (!$.support.transition) {
                    that.options.uploadItem.find('.progress').show()
                        .progressbar({
                            value: 100
                        });
                }
                /*var request_encode = setInterval(function(){
                    $.getJSON("encode-status.php?hash="+file.hash, function(status)
                    {
                        if (status.encoded == -1)
                        {
                            clearInterval(request_encode);
                            $(".encode-status", data.context).html("failed");
                            $(".encode-status", data.context).css("color","red");
                            return;
                        }
                        if (status.encoded == 100)
                        {
                            clearInterval(request_encode);
                            $(".encode-status", data.context).html("Done");
                            $(".encode-status", data.context).css("color","green");
                        }

                        data.context.find('.encoding-progress')
                            .progressbar({
                                value: status.encoded
                            });
                    });
                }, 1000);*/
                //could have used _transition, but its buggy for some reason..
            },
            chunkdone: function(e, data) {
                /*if (data.context) {
                    var progress = 0;
                    try{
                        progress = data.result.files[0].encoded;
                    }catch(err)
                    {

                    }
                    data.context.find('.encoding-progress')
                        .progressbar({
                            value: progress
                        });
                }*/
                var that = $(this).data('filkor-html5Uploader');
                that._retryTime = 0;
            },
            fail: function (e, data) {
                var that = $(this).data('filkor-html5Uploader');
                data.context.each(function (index) {
                    var file = data.files[index];
                    file.error = data.errorThrown;
                    that._setStatus(e, "error", file.error);
                });
            },

            // Callback for upload progress events:
            progress: function (e, data) {
                var that = $(this).data('filkor-html5Uploader');
                if (data.context) {
                    var progress = Math.floor(data.loaded / data.total * 100);
                    that.options.uploadItem.find('.progress')
                        .progressbar({
                            value: progress
                        });
                }
            },

            // Callback for global upload progress events:
            progressall: function (e, data) {
                var $this = $(this),
                    that = $this.data('filkor-html5Uploader'),
                    timeInfo = that.options.uploadItem.find('.time-info'),
                    bitrateInfo = that.options.uploadItem.find('.speed-info');

                timeInfo.html(
                    $this.data('filkor-html5Uploader')._renderTimeInfo(data)
                );

                bitrateInfo.html(
                    $this.data('filkor-html5Uploader')._renderBitrateInfo(data)
                );
            },

            processstart: function () {
                //console.log('processstart..');
            },

            destroy: function (e, data) {
                //destroy file.
                //By default when you click on the cancel btn you only abort the jqXHR, it doesn't deletes the file 
                //(If you want to deletion  you can implement it here)
            },

            // Callback to retrieve the list of files from the server response:
            getFilesFromResponse: function (data) {
                if (data.result && $.isArray(data.result.files)) {
                    return data.result.files;
                }
                return [];
            }
        },

        _addTemplate:function(e,data){
            //data.context is represents a single li.file-item (if fact, it's attached as an object to it's 'data' attribute)
            var _this = this;
            var pv = this.options.postVideoModule;
            var uploadItem = this.options.uploadItem;
            var mode = "edit";
            if(uploadItem.hasClass("add-video")){
                mode = "add";
            }
            var data = uploadItem.data('data') || {};
            var checked = pv.checkVideoItemInfo(uploadItem,mode);
            if (!checked) return;
            if(mode == "add"){
                data.context = pv.addP(uploadItem,pv.TYPES.VUPLOAD.value).data('data',data);
            } else{
                data.context = uploadItem.data('data',data);
            }
            uploadItem.find("#upload_video").remove();
            data.context.find(".del").off("click");

            this._forceReflow(data.context);
            this._transition(data.context).done(
                function () {
                    if ((_this._trigger('added', e, data) !== false) &&
                            (_this.options.autoUpload || data.autoUpload) &&
                            data.autoUpload !== false && !data.files.error) {
                        data.submit();
                    }
                }
            );
            if(mode == "add"){
                this._startHandler(e);
            }
        },

        _initUploadItem:function(data){
            var _this = this;
            var pv = _this.options.postVideoModule;
            var uploadItem = this.options.uploadItem;
            uploadItem.on("click",".start",function(e){
                var startBtn = $(this);
                _this._retryTime = 0;
                pv._editHandler(startBtn,"lock",function(){
                    _this._startHandler(e);
                });
            });
            uploadItem.on("click",".stop",function(e){
                var stopBtn = $(this);
                _this._stopHandler(e,function(){
                    pv._editHandler(stopBtn,"edit");
                });
            });
        },

        _getInfo:function(){
            var wrapper = this.options.uploadItem.find(".info-wrapper");
            return {
                wrapper:wrapper,
                speed:wrapper.find(".speed-info"),
                time:wrapper.find(".time-info")
            };
        },

        _getStatus:function(){
            return this.options.uploadItem.find(".status");
        },

        _setStatus:function(e, status, errText){
            var _this = this;
            var item = this.options.uploadItem;
            var stopBtn = item.find(".stop");
            var startBtn = item.find(".start");
            var info = this._getInfo().wrapper.show();
            var statusDiv = this._getStatus();
            var pv = _this.options.postVideoModule;
            switch(status){
                case "start":
                    item.find(".progress").show();
                    stopBtn.show();
                    startBtn.hide();
                    item.addClass("uploading");
                    item.data('uploading',true);
                    statusDiv.text("正在上传...");
                    break;
                case "stop": case "error":
                    stopBtn.hide();
                    startBtn.show();
                    item.removeClass("uploading");
                    item.data('uploading',false);
                    if(status == "stop"){
                        statusDiv.text("已暂停");
                    } else{
                        if (errText == 'abort') {
                            return;
                        }
                        var errorMsg = "上传出错：" + errText;
                        statusDiv.text(errorMsg);
                        pv._editHandler(stopBtn,"edit");
                        this._retry(e, item, errorMsg);
                    }
                    break;
                case "done":
                    stopBtn.remove();
                    startBtn.remove();
                    $(".browse-btn",item).remove();
                    $(".edit",item).html("编辑").show();
                    $(".control-btn-select",item).replaceWith('<div class="slt_type_local"><input type="hidden" class="slt_src_type" value="'+pv.TYPES.VUPLOAD.value+'" />'+pv.TYPES.VUPLOAD.name+'</div>');
                    $(".fn",item).addClass("done");
                    item.removeClass("uploading");
                    item.data('uploading',false);
                    item.data('done',true);
                    statusDiv.text("上传成功");
                    break;
                case "delete":
                    pv.buildList();
                    break;
                default:;
            }
            this.options.postVideoModule.queryCheck();
        },

        _startHandler: function (e) {
            var _this = this;
            var data;
            e.preventDefault();
            var item = this.options.uploadItem;
            var progress = item.find(".progress").show();
            clearInterval(this._retryTimer);
            //item.data('saved',false);
            item.each(function(index, fileItem) {
                data = $(fileItem).data('data');
                if (data && data.submit && !data.files.error && data.submit()) {
                    //show pause btn for exmaple
                    _this._setStatus(e, "start");
                    window.onbeforeunload = function(){  
                        return "有文件正在上传";     
                    };
                }
            });
        },

        _cancelHandler: function (e) {
            var template = $(e.currentTarget).closest('.item'),
                data = template.data('data') || {},
                that = this;
                
            template.slideUp('fast', function () {
                if (data.jqXHR) {
                    data.jqXHR.abort();
                    
                    //we may also delete the file, even when it's partially uploaded
                    //that._trigger('destroy', e, data);
                }
                template.remove();
                that._setStatus(e, "delete");
            });

        },

        _stopHandler:function(e,callback){
            var _this = this;
            var template = $(e.currentTarget).closest('.item'),
                data = template.data('data') || {},
                that = this;
            if (data.jqXHR) {
                data.jqXHR.abort();
                _this._setStatus(e, "stop");
                if(callback !== undefined){
                    callback();
                }
            }
        },

        _retryMaxTime: 5,
        _retryTime: 0,
        _retryTimer: null,
        _retry: function(e, item, errText, timeout) {
            var _this = this;
            var tick = 1000,
                timeout = timeout || 5000;
            this._retryCountdown(errText, timeout);
            if(++this._retryTime > this._retryMaxTime) {
                this._getStatus().show().text(errText + '， 自动重连' + this._retryMaxTime + '次后失败，请稍后手动重新开始');
                return;
            }
            this._retryTimer = setInterval(function() {
                timeout -= tick;
                _this._retryCountdown(errText, timeout);
                if (timeout <= 0) {
                    clearInterval(_this._retryTimer);
                    _this._startHandler(e, item);
                }
            }, tick);
        },
        _retryCountdown: function(errText, time) {
            this._getStatus().show().text(errText + '， ' + parseInt(time / 1000) + '秒后尝试重连');
        },

        _renderTemplate: function (func, files) {
            if (!func) {
                return $();
            }
            var result = func({
                files: files,
                options: this.options
            });
            if (result instanceof $) {
                return result;
            }

            return $(this.options.templatesContainer).html(result).children();
        },

        _renderUpload: function(files) {
            return this._renderTemplate(
                this.options.uploadTemplate,
                files
            );
        },

        // http://stackoverflow.com/questions/9016307/force-reflow-in-css-transitions-in-bootstrap
        _forceReflow: function (node) {
            return $.support.transition && node.length &&
                node[0].offsetWidth;
        },

        _transition: function (node) {
            var dfd = $.Deferred();
            if ($.support.transition) {
                node.on(
                    $.support.transition.end,
                    function (e) {
                        // Make sure we don't respond to other transitions events
                        // in the container element, e.g. from button elements:
                        if (e.target === node[0]) {
                            node.unbind($.support.transition.end);
                            dfd.resolveWith(node);
                        }
                    }
                );
            } else {
                dfd.resolveWith(node);
            }
            return dfd;
        },

        _formatBitrate: function (bits) {
            if (typeof bits !== 'number') {
                return '';
            }
            if (bits >= 8589934592) {
                return (bits / 1073741824 / 8).toFixed(2) + ' GB/s';
            }
            //1MB would be 8388608
            if (bits >= 12388608) {
                return (bits / 1048576 / 8).toFixed(1) + ' MB/s';
            }
            if (bits >= 8192) {
                return (bits / 1024 / 8).toFixed(0) + ' KB/s';
            }
            if (bits < 0) return 0;

            return (bits / 8).toFixed(2) + ' byte/s';
        },

        _formatTime: function (seconds) {
            if (seconds < 0) seconds = 0;

            var date = new Date(seconds * 1000),
                days = Math.floor(seconds / 86400);
            days = days ? days + 'd ' : '';
            return days +
                ('0' + date.getUTCHours()).slice(-2) + ':' +
                ('0' + date.getUTCMinutes()).slice(-2) + ':' +
                ('0' + date.getUTCSeconds()).slice(-2);
        },

        _renderTimeInfo: function (data) {
            return this._formatTime(
                (data.total - data.loaded) * 8 / data.bitrate
            );
        },

        _renderBitrateInfo: function (data) {
            return this._formatBitrate(data.bitrate);
        },

        _initTemplates: function () {
            var options = this.options;
            options.templatesContainer = this.document[0].createElement(
                options.filesContainer.prop('nodeName')
            );
            if (Handlebars) {
                if (options.uploadTemplateId) {
                    var source = $('#' + options.uploadTemplateId).html();
                    options.uploadTemplate = Handlebars.compile(source);
                }
            }
        },

        _initFilesContainer: function () {
            var options = this.options;
            if (options.filesContainer === undefined) {
                options.filesContainer = this.options.uploadItem.find('.files');
            } else if (!(options.filesContainer instanceof $)) {
                options.filesContainer = $(options.filesContainer);
            }
        },

        _initHandlebarHelpers: function () {
            //debug, usage {{debug}} or {{debug someValue}}
            Handlebars.registerHelper("debug", function (optionalValue) {
                    console.log("Current Context");
              console.log("====================");
              console.log(this);
             
              if (optionalValue) {
                console.log("Value");
                console.log("====================");
                console.log(optionalValue);
              }
            });

            //format File size,
            Handlebars.registerHelper("formatFileSize", function (bytes) {
                if (typeof bytes !== 'number') {
                    return '';
                }
                if (bytes >= 1073741824) {
                    return (bytes / 1073741824).toFixed(1) + ' GB';
                }
                if (bytes >= 1048576) {
                    return (bytes / 1048576).toFixed(1) + ' MB';
                }
                return (bytes / 1024).toFixed(0) + ' KB';
            });

            Handlebars.registerHelper("shortenName", function (name) {
                if (name.length > 45) {
                    name = ' ' + name.substring(0, 45) + '...';
                }
                return name;
            });

        },

        _initEventHandlers: function () {
            this._super();
            this.options.uploadItem.removeData('data');
            var uploadBtn = $("#upload_video",this.options.uploadItem);
            var pv = this.options.postVideoModule;
            pv.upload_query.push(this.options.uploadItem);
            uploadBtn.off("click");
            this._on(uploadBtn, {'click' : this._addTemplate});
            this._initUploadItem();
            this._on(this.options.uploadItem, {
                'click .del': this._cancelHandler
            });
        },

        _initSpecialOptions: function () {
            this._super();
            this._initFilesContainer();
            //this._initTemplates();
        },

        _create: function () {
            this._super();
            //this._initHandlebarHelpers();
        }

    });
}));

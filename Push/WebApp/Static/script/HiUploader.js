function HiUploader() {
    var _options = {
        /*
            form:form表单的名称
            name:file控件的名称
            onview:显示图片
                src:图片的地址
            onprogress:图片上传的进度事件
            onupload:图片上传成功之后触发
                responseText这里是

        */

        form: "",
        name: "",
        url: "",
        onview: function (src) {

        },
        onprogress: function (evt) {
            _options.onprogress(evt);
            if (evt.lengthComputable) {
                var percentComplete = evt.loaded / evt.total;
                //$('#progressbar').progressbar('setValue', percentComplete * 100);
            }
        },
        onupload: function (responseText) {
            console.log(responseText);
        },
    };

    var __reqopt = {
        type: "post",                            //xmlHttpRequest的请求的类型
        mimeType: "application/octet-stream",
        //mimeType: "image/jpeg",
    };

    var __init_formdata = function (file) {
        var reader = new FileReader();
        reader.onload = function (evt) {

        };
        reader.onloadstart = function (evt) {

        };
        reader.onloadend = function (evt) {
            //$("#context").text("加载图片完毕.");



            var img = new Image();
            var url = URL.createObjectURL(file);
            _options.onview(url);
            if (evt.target.readyState != FileReader.DONE) {
                console.log(reader.error);
            }
            else {
                __sendfile(evt, file.name);
            }

        };
        reader.onprogress = function (evt) {

        };
        reader.readAsArrayBuffer(file);
    }
    var __sendfile = function (evt, filename) {

        var formdata = new FormData();
        var blob = new Blob([evt.target.result]);
        formdata.append("file", blob, filename);

        var xhr = new XMLHttpRequest();
        xhr.open(__reqopt.type, _options.url);
        xhr.overrideMimeType(__reqopt.mimeType);

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    _options.onupload(xhr.responseText);
                }
            }
        };
        xhr.upload.onprogress = function (evt) {
            _options.onprogress(evt);
        };
        xhr.send(formdata);
    }

    /*
        /@ 初始化文件集合 @/
    */
    var _initfiles = function () {
        if (_options.name) {
            var files = document.forms[_options.form | 0][_options.name].files;
            if (files && files.length > 0) {
                return files;
            }
        }
        return null;
    }

    /*
        /@ 初始化参数 @/
    */
    var _initOptions = function (options) {
        if (options)
            return extend(_options, options);
        return null
    }

    /*
        /@ 上传 @/
    */
    var _upload = function (options) {
        _initOptions(options);
        var files = _initfiles();
        if (files) {
            for (var i = 0; i < files.length; i++) {
                __init_formdata(files[i]);
            }
        }
    }

    /*
    
    */
    this.upload = function (options) {
        _upload(options);
    }
}

///复制opt对象
function extend(destination, source) {
    for (var property in source)
        destination[property] = source[property];
    return destination;
}
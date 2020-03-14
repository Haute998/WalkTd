/**
 * 客户端接口公共方法
 *
 */
(function ($, app) {

    //接口服务器url前缀
    app.server_url_prefix ="";
    //app.server_url_prefix = 'http://localhost:8088/mingpian/index.php/wx/';

    //图片长宽比
    app.radio = 1 / 0.618;

    //文件上传类型
    app.qiniu = {
        domain: 'http://qiniu.sxvt58.com/',
        type: {
            article: 'article',
            card: 'card',
            greetingcard: 'greetingcard',
            adver: 'adver',
            enterprise: 'enterprise',
            me: 'me'
        }
    }

    //分页每页显示数量
    app.size_per_page = 10;

    /**
     * api request请求
     * @param url  url,不需要包含域名，如account/register
     * @param data 数据
     * @param success 请求成功时触发的回调函数
     * @param error 请求失败时触发的回调函数
     **/
    app.request = function (url, data, success, error) {

        //拼装ajax请求url
        url = app.server_url_prefix + url;
        //ajax请求
        $.ajax(url, {
            data: data,
            dataType: 'json', //服务器返回json格式数据
            type: 'post', //HTTP请求类型
            timeout: 30000, //超时时间设置为30秒；
            success: function (data) {

                if (success && success(data)) {
                    return;
                }

                //根据服务器返回数据，判断接口请求是否合法
                if (!data || !data.code || data.status != 1) {
                    layer.open({ content: data.message, skin: 'msg', time: 2 });
                    $.closeWaiting();
                    return;
                }
            },
            error: function (xhr, type, errorThrown) {

                $.closeWaiting();

                if (xhr.status == 401) {
                    layer.open({ content: '登录失效，请重新登录！', skin: 'msg', time: 2 });
                    location.href = "/";
                    return;
                }
                if (error && error(xhr, type, errorThrown)) {
                    return;
                } else {
                    if (type == 'timeout') {
                        layer.open({ content: '请求超时，可能是由于您的网络不太稳定，请重试！', skin: 'msg', time: 2 });
                    } else {
                        layer.open({ content: '网络异常， 请检查您的网络是否正常！', skin: 'msg', time: 2 });
                    }
                }
            }
        });
    };

}(mui, window.api = {}));

(function ($, window) {
    $.urlparams = function (name) {
        var url = window.location.href;
        var res = [];
        url.replace(new RegExp("[&?]" + name + "=([^&#]*)", "ig"), function ($, $1) {
            res.push(decodeURIComponent($1));
        });
        return res.join(",");
    };

    $.layerIndex = 0;
    $.showWaiting = function () {
        $.layerIndex = layer.open({
            type: 2,
            shadeClose: false
        });
    };
    $.closeWaiting = function () {
        layer.close($.layerIndex);
    };

})(mui, window);
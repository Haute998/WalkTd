function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)","i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

// �ж��Ƿ�Ϊ��(null)
function IsNullOrEmpty(str) {
    if (str == null || str == undefined || typeof str == undefined || str == "null" || str == "undefined" || str == "") {
        return true;
    }
    else {
        return false;
    }
}

//дcookies
function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}
//��ȡcookies
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}
//ɾ��cookies
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

//��ҳ��δ�������֮ǰ��ʾ��loading Html�Զ�������
var _LoadingHtml = '<div id="loadingDiv" style="display: none;"><div id="over" style=" position: absolute;top: 0;left: 0; width: 100%;height: 100%; background-color: #f5f5f5;opacity:0.5;z-index: 1000;"></div><div id="layout" style="position: absolute;top: 40%; left: 40%;width: 20%; height: 20%;  z-index: 1001;text-align:center;"><img src="images/loading.gif" /></div></div>';
//����loadingЧ��
document.write(_LoadingHtml);
//�Ƴ�loadingЧ��
function clearLoading() {
    document.getElementById("loadingDiv").style.display = "none";
}
//չʾloadingЧ��
function showLoading() {
    document.getElementById("loadingDiv").style.display = "block";
}
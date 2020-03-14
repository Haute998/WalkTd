var HeadCode;

function ScrollNotice() {
    var str = location.href;
    var arr = str.split("/");
    HeadCode = arr[arr.length - 2];

    setInterval(GetNoticeInfo, 3000);
    GetNoticeInfo();
}

function GetNoticeInfo() {
    $.ajax({
        type: "post",
        contentType: "application/json",
        url: "../../Chinese/Notice/GetCustNotice",
        data: "{HeadCode:'" + HeadCode + "'}",
        dataType: 'json',
        success: function(data) {
            if (data != null) {
                DealNoticeHTML(data);
            }
        }, error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.status + " | " + XMLHttpRequest.readyState + " | " + textStatus);
        }
    });
}

function DealNoticeHTML(JsonData) {
    var htmlStr = "";
    if (JsonData) {
        for (var i = 0; i < JsonData.length; i++) {
            htmlStr += JsonData[i].Content;
            htmlStr += " ";
        }

        $("#NoticeInfo").html(htmlStr);
    }
}

function QuerySecuritCode(SecuritCode, AddressJson, WxOpenId) {

    if (AddressJson == null || AddressJson == undefined || typeof AddressJson == undefined || AddressJson == "null" || AddressJson == "undefined") AddressJson = "";
    if (WxOpenId == null || WxOpenId == undefined || typeof WxOpenId == undefined || WxOpenId == "null" || WxOpenId == "undefined") WxOpenId = "";

    if (!ValidCode(SecuritCode)) {
        $(".spa2").text("防伪码输入错误！");
        $(".spa2").show();
    }
    else {
        $(".PressMask").show();
        $(".BtnQuery").hide();
        $(".Text1").attr("disabled");

        $.ajax({
            type: "post",
            contentType: "application/json",
            url: "../../Chinese/Home/GetQueryCode",
            data: "{QueryCode:'" + SecuritCode + "',Address:" + AddressJson + ",OpenId:'" + WxOpenId + "'}",
            dataType: 'json',
            success: function (data) {
                console.log(data);
                if (data != null) {
                    DealQueryResult(data, SecuritCode);
                } else {
                    $("#Label3").html("查询编码错误.");
                }
            }, error: function(XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.status + " | " + XMLHttpRequest.readyState + " | " + textStatus);
            }
        });
    }
}

function DealQueryResult(JsonData, SCode) {
    if (JsonData) {
        var iNo = 1;
        $("#div1").hide();
        $("#div2").show();
        $(".gonggao").hide();

        if (JsonData.State == 1 || JsonData.State == 3) {
            $("#CODE p").html("防伪码有效");
            $("#img").attr("src", "images/success.png");
            $(".verify,.jiaxi,.qiye,.hongbao,.footgs").show();
            $(".p2,.p3,.ewm,.zhichi1,.fwcx,.record").hide();
            $('#judge').html('验证成功');
            $("#Label3").html(JsonData.QueryContent);
            $('.p1').css('text-align', 'center');
        }
        else if (JsonData.State == 2) {
            $("#CODE p").html("防伪码已被查询");
            $("#img").attr("src", "images/caveat.png");
            $(".p2,.p3,.jianyi,.record").show();
            $(".d8,.d9,.hongbao,.zhichi1,.fwcx,.footgs").hide();
            $('#judge').html('验证警告');
            $("#Label3").html('首次查询：' + JsonData.FirstDate + '</br>查询密码 : ' + JsonData.BarCode + '</br>如有疑问,请联系我们！');
            $('.p1').css('text-align', 'left');
        }
        else {
            $("#CODE p").html("防伪码无效");
            $("#img").attr("src", "images/error.png");
            $(".p2,.p3,.jianyi,.fwcx").show();
            $(".d8,.d9,.hongbao,.zhichi1,.footgs,.record").hide();
            $('#judge').html('验证失败');
            $("#Label3").html(JsonData.QueryContent);
            $('.p1').css('text-align', 'center');
        }

        $("#Label2").html(SCode);
      
        $("#company").html(JsonData.CompanyName);
        $("#tel").html(JsonData.Tel);
        $("#website").html(JsonData.WebSite);


        //动态添加li 同时动态添加数据
        var str = "";
        for (var i = 0; i < JsonData.RecordList.length; i++) {
            $("#parkNumber").append("<li><div class='d1'><img src='" + JsonData.RecordList[i].WxHeadImg + "' alt='' class='nametx'/></div><div class='d2'><span class='name'>" + JsonData.RecordList[i].WxNickName + "</span><span class='date'>" + JsonData.RecordList[i].QueryTime + "</span><span class='add'>" + JsonData.RecordList[i].Address + "</span></div><div class='d3'><span class='number'>" + '第' + (i + 1) + '次' + "</span></div></li>");
            //alert(JsonData.RecordList[i].WxHeadImg);
            if (JsonData.RecordList[i].WxHeadImg == '') {
                $('.nametx').attr('src', 'images/nm.png')
                $('.name').html('匿名')
            }
        };

        if (JsonData.IsColor) {
            var strColor = "";
            var strCode = SCode;
            var colorSet = JsonData.ColorValueSet;
            var SplitColor = colorSet.split("#");
            for (i = 0; i < SplitColor.length; i++) {
                strColor += "<a style='color:#" + SplitColor[i + 1] + "'>" + strCode.charAt(i) + "</a>";
            }
            $("#FCode").html(strColor);
        }
        else {
            $("#FCode").html(SCode);
        }

        if (JsonData.State != "0") {
            $("#validImg").attr("src", "../../" + JsonData.BarcodeImg);
        }
    }
}

function ValidCode(SecuritCode) {
    var isOK = false;
    if (SecuritCode.length != 0) {
        var hLength = HeadCode.length;
        var NHCode = SecuritCode.substring(0, hLength);
        if (HeadCode == NHCode) {
            isOK = true;
        }
        else {
            isOK = false;
        }
    }

    return isOK;
}

// 跳转https
function ToHttps() { 
        var url = window.location.href;
        if (url.indexOf("https") < 0) {
            url = url.replace("http:", "https:");
            window.location.replace(url);
            return false;
        }
        else return true;
}

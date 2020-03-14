function imgdragstart() { return false; }
if (window.Event)
    document.captureEvents(Event.MOUSEUP);
function nocontextmenu() {
    event.cancelBubble = true
    event.returnValue = false;
    return false;
}
function norightclick(e) {
    if (window.Event) {
        if (e.which == 2 || e.which == 3)
            return false;
    }
    else
        if (event.button == 2 || event.button == 3) {
            event.cancelBubble = true
            event.returnValue = false;
            return false;
        }
}
document.oncontextmenu = nocontextmenu; // for IE5+
document.onmousedown = norightclick; // for all others

var LastCode;

for (i in document.images) document.images[i].ondragstart = imgdragstart;

$(document).ready(function () {
    LastCode = "";
    $("#QueryResult").hide();
});

function QueryFun() {
    $("#re").hide();
    $("#QueryResult").show();
    if (LastCode != $("#Sel").val() && IsInteger()) {
        LastCode = $("#Sel").val();
        $.ajax({
            type: "post",
            contentType: "application/json",
            url: "/ashx/Handler.ashx/GetQueryCode",
            data: "{QueryCode:'" + $("#Sel").val() + "'}",
            dataType: 'json',
            success: function (e) {
                if (e.d != "") {
                    DealShowResult(e.d);
                } else {
                    $("#Label3").html("查询编码错误.");
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.status + " | " + XMLHttpRequest.readyState + " | " + textStatus);
            }
        });
    }
}

function DealShowResult(JsonData) {
    if (JsonData) {
        var jsontemp = eval('(' + JsonData + ')');
        var iNo = 1;
        
        if (jsontemp.IsColor == "Y") {
            var strColor="";
            var strCode = $("#Sel").val();
            var colorSet = jsontemp.ColorValueSet;
            var SplitColor = colorSet.split("#");
            for (i = 0; i < SplitColor.length; i++) {

                strColor += "<a style='color:#" + SplitColor[i+1] + "'>" + strCode.charAt(i) + "</a>";
            }   
            $("#Label2").html(strColor);
        }   
        else {
            $("#Label2").html($("#Sel").val());
        }
        if (jsontemp.State != "0") {
            $("#img").attr("src", jsontemp.BarCodeImg);
        }
        else {
            $("#img").attr("src", "/images/awtywtcw.png");
        }

        $("#Label3").html(jsontemp.QueryContent);
    }
}

//判断输入的字符是否为整数    
function IsInteger() {
    var str = document.getElementById('Sel').value.trim();

    if (str.length > 6) {
        var reg = new RegExp("^[0-9]*$");
        if (!reg.test(str)) {
            alert("对不起，您输入的防伪码位数不正确!"); //请将“整数类型”要换成你要验证的那个属性名称！
            return false;
        }   
        else return true;
    }
    else {
        alert("对不起，您输入的防伪码类型格式不正确!");
        return false;
    }
}

function CloseQuery() {
    $("#re").show();
    $("#QueryResult").hide();
}

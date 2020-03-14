
function ShowYearCharts(urls, id, unit, names, title, Type, Thedatas) {
    var datas;
    $.ajax({
        url: urls,
        data: eval('(' + Thedatas + ')'),
        type: "post",
        async: false,
        success: function (data) {
            datas = data;
        }
    });
    $('#' + id).highcharts({
        chart: {
            type: Type
        },
        title: {
            text: title
        },
        xAxis: {
            categories: [
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                '10',
                '11',
                '12'
            ],
            crosshair: true
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        yAxis: {
            title: {
                text: '(' + unit + ')'
            }
        },
        series: [{
            name: names,
            data: eval(datas)
        }]
    });
}

function ShowYearsCharts(urls, id, unit, names, title, Type) {
    var datas;
    $.ajax({
        url: urls,
        type: "post",
        async: false,
        success: function (data) {
            datas = data;
        }
    });
  
    $('#' + id).highcharts({
        chart: {
            type: Type
        },
        title: {
            text: title
        },
        xAxis: {
            categories: [
                names
  
            ],
            crosshair: true
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        yAxis: {
            title: {
                text: '(' + unit + ')'
            }
        },
        series: eval(datas)
    });
}
function PieCharts(urls, id, title, Type) {
    var datas;
    $.ajax({
        url: urls,
        type: "post",
        async: false,
        success: function (data) {
            datas = data;
        }
    });
    $('#' + id).highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: Type
        },
        title: {
            text: title
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: "Percent",
            colorByPoint: true,
            data: eval(datas)
        }]
    });
}

var page;
var data;
var map = new BMap.Map("map");   
map.centerAndZoom(new BMap.Point(113.938726, 33.832453), 13);    
map.enableScrollWheelZoom();
map.addControl(new BMap.NavigationControl()); 
map.addControl(new BMap.MapTypeControl()); 
var mapStyle = {
    style: "dark"
};
map.setMapStyle(mapStyle);
map.setCurrentCity("漯河市");
(function (document, window, $) {
    page = {
        init: function () {
            page.bind();
            page.initData();
        },
        bind: function () {
            $("#btnBuffer").click(function () {
                $(".check_increase").addClass("check_increase_act");
                $(".check_decrease").hide();
                $("#chemistry_check_in").show().siblings().hide();
                $.ajax({
                    url: '/Home/GetProject',
                    type: 'GET',
                    dataType: 'JSON',
                    success: function (res) {
                        if (res.code === 200) {
                            $('#chemistry_check_in .search_sesult_box').html(
                                template('projectTpl', { projectList: res.data })
                            );
                        } else {
                            console.warn(res.msg);
                        }
                    },
                    error: function () {
                        console.log('服务器异常，请配合后端程序使用');
                    }
                });
            });

            $("#btnReset").click(function () {
                map.clearOverlays();
                page.drawArea(data, 'green');
            });
        },
        initData: function () {
            InitMapArea();
            InitXunShi();
            InitArea();
            InitDIP();
            InitRegionCount();
        },
        drawArea: function (data, color) {
            for (var i = 0; i < data.length; i++) {
                var points = [];
                var point = data[i].area_zuobiao;
                var pointArr = point.split(';');
                pointArr = pointArr.filter(function (s) {
                    return s && s.trim();
                });
                if (pointArr != null && pointArr != '') {
                    for (var j = 0; j < pointArr.length; j++) {
                        points.push("new BMap.Point(" + pointArr[j].split(',')[1] + "," + pointArr[j].split(',')[0] + ")");
                    }
                }
                page.fun(i, "" + data[i].area_mubiaojingdu + "," + data[i].area_mubiaoweidu + "", "" + points.toString() + "", '' + data[i].Peasant_name + '', '' + color + '');
            }
        },
        fun: function (i, xy, arr, wb, ys) {
            //创建经纬度数组
            eval("var secRingCenter" + i + " = new BMap.Point(" + xy + ")");
            eval("var secRing" + i + " = [" + arr + "]");
            //创建多边形
            eval("var secRingPolygon" + i + "= new BMap.Polygon(secRing" + i + ", {fillColor: \"" + ys + "\",strokeColor: \"" + ys + "\", strokeWeight: 2})");
            //添加多边形到地图上
            map.addOverlay(eval("secRingPolygon" + i));
            //给多边形添加鼠标事件
            eval("secRingPolygon" + i).addEventListener("mouseover", function () {//鼠标经过时
                eval("secRingPolygon" + i).setStrokeColor("red"); //多边形边框为红色
                map.addOverlay(eval("secRingLabel" + i)); //添加多边形遮照
            });
            eval("secRingPolygon" + i).addEventListener("mouseout", function () {
                eval("secRingPolygon" + i).setStrokeColor(ys);
                map.removeOverlay(eval("secRingLabel" + i));
            });
            eval("secRingPolygon" + i).addEventListener("click", function () {
                map.zoomIn();
                eval("secRingPolygon" + i).setStrokeColor(ys);
                map.setCenter(eval("secRingCenter" + i));
            });
            //创建标签
            eval("var secRingLabel" + i + "= new BMap.Label(\"<b>" + wb + "</b>\", {offset: new BMap.Size(0, 0), position: secRingCenter" + i + "})");
            eval("secRingLabel" + i).setStyle({
                "z-index": "999999", "padding": "2px", "border": "1px solid #ccff00", "color": "white",
            });
        }
    };
    page.init();
})(document, window, jQuery);
function InitMapArea() {
    $.ajax({
        url: '/Home/GetAllArea',
        type: 'GET',
        dataType: 'JSON',
        success: function (res) {
            if (res.code === 200) {
                data = res.data;
                page.drawArea(data, 'green');
            } else {
                console.warn(res.msg);
            }
        },
        error: function () {
            console.log('服务器异常，请配合后端程序使用');
        }
    });
}
function InitXunShi() {
    $.ajax({
        url: '/Home/GetXunShi',
        type: 'GET',
        dataType: 'JSON',
        success: function (res) {
            if (res.code === 200) {
                result = res.data;
                $('.XunShiImages').html(
                    template('ImageItemTpl', { imageLists: res.data })
                );
            } else {
                console.warn(res.msg);
            }
        },
        error: function () {
            console.log('服务器异常，请配合后端程序使用');
        }
    });
}
function InitArea() {
    $.ajax({
        url: '/Home/GetArea',
        type: 'GET',
        dataType: 'JSON',
        success: function (res) {
            if (res.code === 200) {
                $('.area_list').html(
                    template('areaTpl', { areaList: res.data.dt1 })
                );
                $('.depart_number_box').html(
                    template('todayTpl', { todayCount: res.data.dt2 })
                );
            } else {
                console.warn(res.msg);
            }
        },
        error: function () {
            console.log('服务器异常，请配合后端程序使用');
        }
    });
}
function InitDIP() {
    $.ajax({
        url: '/Home/GetDIP',
        type: 'GET',
        dataType: 'JSON',
        success: function (res) {
            if (res.code === 200) {
                result = res.data;
                $('.message_scroll_box').html(
                    template('dipTpl', { dipList: res.data })
                );
            } else {
                console.warn(res.msg);
            }
        },
        error: function () {
            console.log('服务器异常，请配合后端程序使用');
        }
    });
}
function InitRegionCount() {
    $.ajax({
        url: '/Home/GetRegionCount',
        type: 'GET',
        dataType: 'JSON',
        success: function (res) {
            if (res.code === 200) {
                result = res.data;
                $('.data_show_box').html(
                    template('regionTpl', { regionCount: result[0].regionCount.toString().split('') })
                );
            } else {
                console.warn(res.msg);
            }
        },
        error: function () {
            console.log('服务器异常，请配合后端程序使用');
        }
    });

}
function projectArea(projectId) {
    $.ajax({
        url: '/Home/GetAreaByProject?projectId=' + projectId,
        type: 'GET',
        dataType: 'JSON',
        success: function (res) {
            if (res.code === 200) {
                map.clearOverlays();
                var data = res.data;
                page.drawArea(data, 'green');
            } else {
                console.warn(res.msg);
            }
        },
        error: function () {
            console.log('服务器异常，请配合后端程序使用');
        }
    });
}

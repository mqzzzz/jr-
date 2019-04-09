var page;
var data, dipAreaData;
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
            laydate.render({
                elem: '#txt_carDate'
                , value: getNowFormatDate()
            });
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
            $("#btnZhuanTi").click(function () {
                $(".check_increase").addClass("check_increase_act");
                $(".check_decrease").hide();
                $(".addition_check_in").show().siblings().hide();
                $.ajax({
                    url: '/Home/GetDipArea',
                    type: 'GET',
                    dataType: 'JSON',
                    success: function (res) {
                        if (res.code === 200) {
                            dipAreaData = res.data;
                            $('._dip_area').html(
                                template('dipareaTpl', { dipareaList: res.data })
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

            $("#btnCarCheck").click(function () {
                $(".check_increase").addClass("check_increase_act");
                $(".check_decrease").hide();
                $("#car_check_in").show().siblings().hide();
                $.ajax({
                    url: '/Home/GetNongJiList',
                    type: 'GET',
                    dataType: 'JSON',
                    success: function (res) {
                        if (res.code === 200) {
                            $('.car_list').html(
                                template('carTpl', { carList: res.data })
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
                page.fun(i, "" + data[i].area_mubiaojingdu + "," + data[i].area_mubiaoweidu + "", "" + points.toString() + "", '' + data[i].Peasant_name + '', '' + data[i].area_ID + '', '' + color + '');
            }
        },
        fun: function (i, xy, arr, wb, id, ys) {
            ys = ys == '' ? "Green" : ys;
            if (ys.indexOf(",") != -1)
                ys = "rgba(" + ys + ")";
            //创建经纬度数组
            eval("var secRingCenter" + i + " = new BMap.Point(" + xy + ")");
            eval("var secRing" + i + " = [" + arr + "]");
            //创建多边形
            eval("var secRingPolygon" + i + "= new BMap.Polygon(secRing" + i + ", {fillColor: \"" + ys + "\",strokeColor: \"" + ys + "\", strokeWeight: 2})");
            //添加多边形到地图上


            map.addOverlay(eval("secRingPolygon" + i));
            showText(eval("secRingPolygon" + i), wb, id, eval("secRingCenter" + i + ""), xy);
            ////给多边形添加鼠标事件
            //eval("secRingPolygon" + i).addEventListener("mouseover", function () {//鼠标经过时
            //    eval("secRingPolygon" + i).setStrokeColor("red"); //多边形边框为红色
            //    map.addOverlay(eval("secRingLabel" + i)); //添加多边形遮照
            //});
            //eval("secRingPolygon" + i).addEventListener("mouseout", function () {
            //    eval("secRingPolygon" + i).setStrokeColor(ys);
            //    map.removeOverlay(eval("secRingLabel" + i));
            //});
            //eval("secRingPolygon" + i).addEventListener("click", function () {
            //    map.zoomIn();
            //    eval("secRingPolygon" + i).setStrokeColor(ys);
            //    map.setCenter(eval("secRingCenter" + i));
            //});
            ////创建标签
            //eval("var secRingLabel" + i + "= new BMap.Label(\"<b>" + wb + "</b>\", {offset: new BMap.Size(0, 0), position: secRingCenter" + i + "})");
            //eval("secRingLabel" + i).setStyle({
            //    "z-index": "999999", "padding": "2px", "border": "1px solid #ccff00", "color": "white",
            //});
        }
    };
    page.init();
})(document, window, jQuery);

//显示信息
function showText(polygon, pName, id, point, xy) {
    //或的多边形的所有顶点
    // point = getCenterPoint(polygon.getPath());
    //获得中心点
    var label = new BMap.Label(pName, { offset: new BMap.Size(-40, -25), position: point });
    label.setStyle({ color: "#fff", fontSize: "14px", backgroundColor: "0.05", border: "0", fontWeight: "bold" });//对label 样式隐藏    
    polygon.addEventListener('mouseover', function () { map.addOverlay(label); });
    polygon.addEventListener('mouseout', function () { map.removeOverlay(label); });

    polygon.addEventListener('click', function () {
        layer.open({
            type: 2
            , title: '历史数据'
            , area: ['700px', '600px']
            , shade: 0
            , maxmin: true
            , content: '/Home/CollectHistory?areaId=' + id
        });        //var point = new BMap.Point(xy.split(',')[0], xy.split(',')[1]);
        //var infoWindow = new BMap.InfoWindow("<div class=\"bs-example\" data-example-id=\"button-group-sizing\">"
        //    + "<div class=\"btn-group\" role=\"group\" aria-label=\"...\">"
        //    + "<button type = \"button\" class= \"btn btn-default\" onclick='getAreaInfo(" + id + ",1)'>巡视</button>"
        //    + "<button type=\"button\" class=\"btn btn-default\" onclick='getAreaInfo(" + id + ",2)'>亩产</button>"
        //    + "<button type=\"button\" class=\"btn btn-default\" onclick='getAreaInfo(" + id + ",3)'>管理</button></div><br>"
        //    + "<div class=\"btn-group\" role=\"group\" aria-label=\"...\" style='padding-top:5px;'>"
        //    + "<button type=\"button\" class=\"btn btn-default\" onclick='getAreaInfo(" + id + ",4)'>整地</button>"
        //    + "<button type=\"button\" class=\"btn btn-default\" onclick='getAreaInfo(" + id + ",5)'>施肥</button>"
        //    + "<button type=\"button\" class=\"btn btn-default\" onclick='getAreaInfo(" + id + ",6)'>打药</button></div></div>");  // 创建信息窗口对象 
        //map.openInfoWindow(infoWindow, point); //开启信息窗口
    });
}
function getAreaInfo(areaId, typeId) {
    if (typeId === 1) {
        $(".check_increase").addClass("check_increase_act");
        $(".check_decrease").hide();
        $("#xunshi_check_in").show().siblings().hide();
        $.ajax({
            url: '/Home/GetXunShiByAreaId',
            data: { areaId: areaId },
            type: 'GET',
            dataType: 'JSON',
            success: function (res) {
                if (res.code === 200) {
                    $('.xunshi').html(
                        template('xunshiTpl', { xunshiList: res.data })
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
    else if (typeId === 2) {
        $(".check_increase").addClass("check_increase_act");
        $(".check_decrease").hide();
        $("#muchan_check_in").show().siblings().hide();
        $.ajax({
            url: '/Home/GetMuchanByAreaId',
            data: { areaId: areaId },
            type: 'GET',
            dataType: 'JSON',
            success: function (res) {
                if (res.code === 200) {
                    $('.muchan').html(
                        template('muchanTpl', { muchanList: res.data })
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
    else if (typeId === 3) {
        $(".check_increase").addClass("check_increase_act");
        $(".check_decrease").hide();
        $("#manage_check_in").show().siblings().hide();
        $.ajax({
            url: '/Home/GetManageByAreaId',
            data: { areaId: areaId },
            type: 'GET',
            dataType: 'JSON',
            success: function (res) {
                if (res.code === 200) {
                    $('.manage').html(
                        template('manageTpl', { manageList: res.data })
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
    else if (typeId === 4) {
        $(".check_increase").addClass("check_increase_act");
        $(".check_decrease").hide();
        $("#zhengdi_check_in").show().siblings().hide();
        $.ajax({
            url: '/Home/GetZhengDiByAreaId',
            data: { areaId: areaId },
            type: 'GET',
            dataType: 'JSON',
            success: function (res) {
                if (res.code === 200) {
                    $('.zhengdi').html(
                        template('zhengdiTpl', { zhengdiList: res.data })
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
    else if (typeId === 5) {
        $(".check_increase").addClass("check_increase_act");
        $(".check_decrease").hide();
        $("#shifei_check_in").show().siblings().hide();
        $.ajax({
            url: '/Home/GetShiFeiByAreaId',
            data: { areaId: areaId },
            type: 'GET',
            dataType: 'JSON',
            success: function (res) {
                if (res.code === 200) {
                    $('.shifei').html(
                        template('shifeiTpl', { shifeiList: res.data })
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
    else if (typeId === 6) {
        $(".check_increase").addClass("check_increase_act");
        $(".check_decrease").hide();
        $("#dayao_check_in").show().siblings().hide();
        $.ajax({
            url: '/Home/GetDaYaoByAreaId',
            data: { areaId: areaId },
            type: 'GET',
            dataType: 'JSON',
            success: function (res) {
                if (res.code === 200) {
                    $('.dayao').html(
                        template('dayaoTpl', { dayaoList: res.data })
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
}
function getCenterPoint(path) {

    var x = 0.0;
    var y = 0.0;
    for (var i = 0; i < path.length; i++) {
        x = x + parseFloat(path[i].lng);
        y = y + parseFloat(path[i].lat);
    }
    x = x / path.length;
    y = y / path.length;


    return new BMap.Point(x, y);

}

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
                if (res.data != null) {
                    $('.message_scroll_box').html(
                        template('dipTpl', { dipList: res.data })
                    );
                }
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
                if (res.data[0].regionCount != null) {
                    $('.data_show_box').html(
                        template('regionTpl', { regionCount: result[0].regionCount.toString().split('.')[0].split('') })
                    );
                }
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

function loadDipArea(areaId, dipId) {
    $.each(dipAreaData, function (i, item) {
        if (item.AREA_ID == areaId && item.DIP_ID == dipId) {
            map.clearOverlays();
            var points = [];
            var point = item.area_zuobiao;
            var pointArr = point.split(';');
            pointArr = pointArr.filter(function (s) {
                return s && s.trim();
            });
            if (pointArr != null && pointArr != '') {
                for (var j = 0; j < pointArr.length; j++) {
                    points.push("new BMap.Point(" + pointArr[j].split(',')[1] + "," + pointArr[j].split(',')[0] + ")");
                }
            }
            page.fun(i, "" + item.area_mubiaojingdu + "," + item.area_mubiaoweidu + "", "" + points.toString() + "", '' + item.Peasant_name + '', '', '' + item.DIP_yanse + '');
        }
    });
}

function GetTrail(nongjiId) {
    map.clearOverlays();
    $.ajax({
        url: '/Home/GetNongjiTrail?nongjiId=' + nongjiId + '&datetime=' + $("#txt_carDate").val(),
        type: 'GET',
        dataType: 'JSON',
        success: function (res) {
            if (res.code === 200) {
                map.clearOverlays();
                var data = res.data;
                var points = [];
                for (var i = 0; i < data.length; i++) {
                    points.push(new BMap.Point(data[i].shebei_jingdu, data[i].shebei_weidu));
                }
                var polyline = new BMap.Polyline(points,
                    { strokeColor: "#FFFF00", strokeWeight: 6, strokeOpacity: 0.5 }
                );
                map.addOverlay(polyline);
            } else {
                console.warn(res.msg);
            }
        },
        error: function () {
            console.log('服务器异常，请配合后端程序使用');
        }
    });
}

function getNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = year + seperator1 + month + seperator1 + strDate;
    return currentdate;
}

(function (document, window, $) {
    var page = {
        init: function () {
            //page.bind();
            page.initData();
        },
        bind: function () {
        },
        initData: function () {
            $('#table_day').bootstrapTable({
                method: "get",
                striped: true,
                singleSelect: false,
                url: '/Farmer/GetCurrDayList',
                dataType: "json",
                pagination: true, //分页
                sidePagination: "server",
                pageNumber: 1,
                pageSize: 5,
                queryParamsType: "",
                search: false, //显示搜索框
                contentType: "application/x-www-form-urlencoded",
                queryParams: function (params) {
                    return {
                        pageNum: params.pageNumber,
                        pageSize: params.pageSize
                    };
                },
                idField: "ROWNUM",//指定主键列
                columns: [
                    {
                        title: "姓名",
                        field: 'Peasant_name',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '联系方式',
                        field: 'Peasant_tep',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '所在乡镇',
                        field: 'Peasant_xiang',
                        align: 'center',
                        valign: 'middle'
                    },

                    {
                        title: '所在村庄',
                        field: 'Peasant_cun',
                        align: 'center'
                    }

                ]
            });
            $('#table_week').bootstrapTable({
                method: "get",
                striped: true,
                singleSelect: false,
                url: '/Farmer/GetWeekList',
                dataType: "json",
                pagination: true, //分页
                sidePagination: "server",
                pageNumber: 1,
                pageSize: 5,
                queryParamsType: "",
                search: false, //显示搜索框
                contentType: "application/x-www-form-urlencoded",
                queryParams: function (params) {
                    return {
                        pageNum: params.pageNumber,
                        pageSize: params.pageSize
                    };
                },
                idField: "ROWNUM",//指定主键列
                columns: [
                    {
                        title: "姓名",
                        field: 'Peasant_name',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '联系方式',
                        field: 'Peasant_tep',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '所在乡镇',
                        field: 'Peasant_xiang',
                        align: 'center',
                        valign: 'middle'
                    },

                    {
                        title: '所在村庄',
                        field: 'Peasant_cun',
                        align: 'center'
                    }

                ]
            });
            $('#table_month').bootstrapTable({
                method: "get",
                striped: true,
                singleSelect: false,
                url: '/Farmer/GetMonthList',
                dataType: "json",
                pagination: true, //分页
                sidePagination: "server",
                pageNumber: 1,
                pageSize: 5,
                queryParamsType: "",
                search: false, //显示搜索框
                contentType: "application/x-www-form-urlencoded",
                queryParams: function (params) {
                    return {
                        pageNum: params.pageNumber,
                        pageSize: params.pageSize
                    };
                },
                idField: "ROWNUM",//指定主键列
                columns: [
                    {
                        title: "姓名",
                        field: 'Peasant_name',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '联系方式',
                        field: 'Peasant_tep',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '所在乡镇',
                        field: 'Peasant_xiang',
                        align: 'center',
                        valign: 'middle'
                    },

                    {
                        title: '所在村庄',
                        field: 'Peasant_cun',
                        align: 'center'
                    }

                ]
            });
            $('#table_no').bootstrapTable({
                method: "get",
                striped: true,
                singleSelect: false,
                url: '/Farmer/GetMonthNoServerList',
                dataType: "json",
                pagination: true, //分页
                sidePagination: "server",
                pageNumber: 1,
                pageSize: 5,
                queryParamsType: "",
                search: false, //显示搜索框
                contentType: "application/x-www-form-urlencoded",
                queryParams: function (params) {
                    return {
                        pageNum: params.pageNumber,
                        pageSize: params.pageSize
                    };
                },
                idField: "ROWNUM",//指定主键列
                columns: [
                    {
                        title: "姓名",
                        field: 'Peasant_name',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '联系方式',
                        field: 'Peasant_tep',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '所在乡镇',
                        field: 'Peasant_xiang',
                        align: 'center',
                        valign: 'middle'
                    },
                    {
                        title: '所在村庄',
                        field: 'Peasant_cun',
                        align: 'center',
                        valign: 'middle'
                    },

                ]
            });
        }
    };
    page.init();
})(document, window, jQuery);
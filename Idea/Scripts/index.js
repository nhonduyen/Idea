﻿$(document).ready(function () {
    $('#lhome').addClass("active");
    $('#txtAddSchedule,#com_dt,#txtAddSchedule1')
        .datepicker({
            format: 'yyyy-mm-dd'
        }).on('changeDate', function (ev) {
            $(this).datepicker('hide');
        });
    if ($('#username').val()) {
        $('#btnUpload').prop('disabled', false);
    }
    else {
        $('#btnUpload').prop('disabled', true);
    }
    
    var tbProgress = $('#tbMainDefault').DataTable(
            {
                sort: false,
                "processing": true,
                "serverSide": true,
                "searching": false,
                ajax: {
                    type: "POST",
                    contentType: "application/json",
                    url: $('#hdUrl').val().replace("Action", "GetMainProject"),
                    data: function (d) {
                        // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                        return JSON.stringify({ dataTableParameters: d });
                    }
                }

            });
    var tbIdea = $('#tbNewIdea').DataTable(
            {
                sort: false,
                "processing": true,
                "serverSide": true,
                "searching": false,
                ajax: {
                    type: "POST",
                    contentType: "application/json",
                    url: $('#hdUrl').val().replace("Action", "GetIdea"),
                    data: function (d) {
                        return JSON.stringify({
                            dataTableParameters: d,
                            div: $('#filterDivsion1').val(),
                            dep: $('#filterDepartment1').val()
                        });
                    }
                }

            });
    var tbNewPrj = $('#tbNewPrj').DataTable(
           {
               sort: false,
               "processing": true,
               "serverSide": true,
               "searching": false,
               ajax: {
                   type: "POST",
                   contentType: "application/json",
                   url: $('#hdUrl').val().replace("Action", "GetNewProject"),
                   data: function (d) {
                       // note: d is created by datatable, the structure of d is the same with DataTableParameters model above
                       return JSON.stringify({
                           dataTableParameters: d,
                           div: $('#filterDivsion').val(),
                           dep: $('#filterDepartment').val(),
                           grade: $('#filterGrade').val()
                       });
                   }
               },

           });
    $.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var div = $('#filterDivsion').val();
        var grade = $('#filterGrade').val();
        var dep = $('#filterDepartment').val();
        if (div || dep || grade) {
            return true;
        }
        return false;
    }
);
    $('#filterDivsion1,#filterDepartment1').on('change', function () {
        if ($(this).attr('id') == 'filterDivsion1') {
            $('#filterDepartment1').find("option:gt(0)").remove();
            $.ajax({
                url: $('#hdUrl1').val().replace("Action", "GetDepartment"),
                data: JSON.stringify({
                    DIV: $(this).val()
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    for (var i = 0; i < data.length; i++) {
                        $('#filterDepartment1').append($('<option>', {
                            value: $.trim(data[i].DEPARTMENT),
                            text: data[i].DEPARTMENT
                        }));
                    }

                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        tbIdea.draw();
        return false;
    });
    $('#btnUpload').on('click', function () {
        var username = $('#username').val();
        if (!username) {
            bootbox.alert("Please login first (Đăng nhập để sử dụng)");
            $('#Reply,#Reply1,#btnLike,#btnLike1').prop('disabled', true);
            return false;
        }
        else {
            $('#Reply,#Reply1,#btnLike,#btnLike1,#btnRegIdea').prop('disabled', false);
        }
        $('#btnSetProject,#btnDelIdea').prop('disabled', true);
        $('#frmRegIdea')[0].reset();
        $('#tbReply').find("tr:not(:first)").remove();

        $('#Reply').attr('data-id', '');
        $('#cNow').text(moment().format('YYYY-MM-DD HH:mm:ss'));
        var name = $('#username').attr('data-name');
        var dept = $('#username').attr('data-dept');
        var division = $('#username').attr('data-div');

        //$('#division').selectize()[0].selectize.setValue(division, false);
        $('#division').val(division);

        $('#department').val(dept, false);
        $('#name').val(name);
        $('#btnRegIdea').attr('data-action', 0);
        $("#mdRegIdea").modal({
            backdrop: 'static',
            keyboard: false
        });
        return false;
    });

    $('#btnSetProject').on('click', function () {
        var username = $('#username').val();
        var emp = $('#btnRegIdea').attr('data-emp');
        if (!username) {
            $('#Reply,#Reply1,#btnLike,#btnLike1').prop('disabled', true);
        }
        else {
            $('#Reply,#Reply1,#btnLike,#btnLike1').prop('disabled', false);
        }
        if (username != emp) {
            bootbox.alert("You cannot set this idea");
        }
        $('#action_plan').attr('rowspan', 2);
        $('.trNewPlan').remove();
        $('#frmRegPrj')[0].reset();
        
        
        var id = $('#Reply').attr('data-id');
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetIdeaById"),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {

                    $('#btnRegPrj').attr('data-emp', $.trim(data[1].Value));
                    $('#txtTitle').val($.trim(data[2].Value));
                    $('#prj_quantiative').val($.trim(data[4].Value));
                    $('#prj_qualiative').val($.trim(data[5].Value));
                    $('#selDept').val($.trim(data[7].Value));
                    $('#frmRegPrj').attr('data-id', id);
                    $('#txtName').val($.trim(data[9].Value));
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        $("#mdRegIdea").modal('hide');
        $("#mdRegPrj").modal({
            backdrop: 'static',
            keyboard: false
        });
        return false;
    });
    $('#btnUpdate').on('click', function () {
        var username = $('#username').val();
        if (!username) {
            bootbox.alert("Please login to use this function");
        }
    });
    $('#Reply').on('click', function () {
        var username = $('#username').val();
        var comment = $('#comment').val();
        var ideaId = $('#Reply').attr('data-id');
      
        if (!username) {
            bootbox.alert("Please login to use this function");
            return false;
        }
        if (!ideaId) {
            bootbox.alert("Cannot comment on empty idea");
            return false;
        }
        if (!comment) {
            bootbox.alert("Please enter comment");
            return false;
        }
        var reply = { IDEA_ID: ideaId, REP_EMP_ID: username, COMMENTS: comment, REP_EMP_NAME: $('#username').attr('data-name') };

        var seq = $('#tbReply tr').length;
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "InsertIdeaComment"),
            data: JSON.stringify({
                reply: reply
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data > 0) {
                    $('<tr><td>'+seq+'</td><td>' + $('#username').attr('data-dept') + '</td><td>' + $('#username').attr('data-name') + '</td><td>' + comment + '</td><td>' + moment().format('YYYY-MM-DD hh:mm:ss') + '</td></tr>').prependTo("#tbReply > tbody");
                    tbIdea.ajax.reload();
                }
                else {
                    bootbox.alert("Fail");
                }
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });

        $('#comment').val('');

        return false;
    });
    $('#Reply1').on('click', function () {
        var username = $('#username').val();
        var comment = $('#comment1').val();
        var ideaId = $('#Reply1').attr('data-id');
        if (!username) {
            bootbox.alert("Please login to use this function");
            return false;
        }
        if (!ideaId) {
            bootbox.alert("Cannot comment on empty idea");
            return false;
        }
        if (!comment) {
            bootbox.alert("Please enter comment");
            return false;
        }
        var reply = { IDEA_ID: ideaId, REP_EMP_ID: username, REP_EMP_NAME: $('#username').attr('data-name'), COMMENTS: comment };

        var seq = $('#tbReply1 tr').length;

        $.ajax({
            url: $('#hdUrl').val().replace("Action", "InsertPrjComment"),
            data: JSON.stringify({
                reply: reply
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data > 0) {
                    $('<tr><td>' + seq + '</td><td>' + $('#username').attr('data-dept') + '</td><td>' + $('#username').attr('data-name') + '</td><td>' + comment + '</td><td>' + moment().format('YYYY-MM-DD hh:mm:ss') + '</td></tr>').prependTo("#tbReply1 > tbody");
                    //tbIdea.ajax.reload();
                    tbNewPrj.ajax.reload();
                    tbProgress.ajax.reload();
                }
                else {
                    bootbox.alert("Fail");
                }
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });

        $('#comment1').val('');

        return false;
    });
    $('#frmRegIdea').submit(function (e) {
        e.preventDefault();
        var username = $('#username').val();
        var host = $('#btnRegIdea').attr('data-emp');
        var action = parseInt($('#btnRegIdea').attr('data-action'));
        if (!$('#title').val()) {
            bootbox.alert("Please enter title (Hãy điền tiêu đề)");
            return false;
        }
        var idea = {
            ID: $('#Reply').attr('data-id'),
            EMP_ID: $('#username').val(),
            IDEA_TITLE: $('#title').val(),
            DETAIL: $('#detail').val(),
            QUANTITATIVE: $('#quantiative').val(),
            QUALITATIVE: $('#qualiative').val()
        };
        if (!idea.IDEA_TITLE) {
            alert('Please enter idea title');
            return false;
        }
        if (!idea.DETAIL) {
            alert('Please enter idea detail');
            return false;
        }
        var department = $('#department').val();
        //var division = $('#division').selectize()[0].selectize.getValue();
        var division = $('#division').val();
        if (action == 1) {
            if (username != host) {
                bootbox.alert("You cannot modify this idea");
                return false;
            }
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "RegIdea"),
                data: JSON.stringify({
                    idea: idea,
                    division: division,
                    department: department,
                    action: action
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        bootbox.alert("Save success");
                        tbIdea.ajax.reload();
                    }
                    else {
                        bootbox.alert("Fail");
                    }
                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        else {
            if (idea.IDEA_TITLE && idea.DETAIL) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "RegIdea"),
                    data: JSON.stringify({
                        idea: idea,
                        division: division,
                        department: department,
                        action: action
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        if (data > 0) {
                            bootbox.alert("Register success");
                            tbIdea.ajax.reload();
                        }
                        else {
                            bootbox.alert("Fail");
                        }
                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
            }
            else {
                alert('Please enter title and detail');
            }
        }

        $("#mdRegIdea").modal('hide');
        return false;
    });
    $('#tbNewIdea').on('click', '.title', function () {
        var username = $('#username').val();
        var role = $('#username').attr('data-role');
        if (username != $(this).attr('data-emp')) {
            $('#btnSetProject,#btnRegIdea,#btnDelIdea,#btnDelegate').prop('disabled', true);
            if (role == "2") {
                $('#btnDelIdea').prop('disabled', false);
            }
        }
        else {
            $('#btnSetProject,#btnRegIdea,#btnDelIdea,#btnDelegate').prop('disabled', false);
        }
        if (!username) {
            $('#Reply,#Reply1,#btnLike,#btnLike1,#btnDelIdea').prop('disabled', true);
        }
        else {
            $('#Reply,#Reply1,#btnLike,#btnLike1').prop('disabled', false);
        }
      
        var id = $(this).attr('id');
        $('#Reply').attr('data-id', id);
        $('#btnDelIdea').attr('data-ideaid', id);
        $('#btnRegIdea').attr('data-action', 1);
        $('#tbReply').find("tr:not(:first)").remove();
        $('#cDept').text($('#username').attr('data-dept'));
        $('#cName').text($('#username').attr('data-name'));
        $('#cNow').text(moment().format('YYYY-MM-DD HH:mm:ss'));
        var numLike = parseInt($(this).parent().parent().find('.like').children().text());
        $('#btnLike').find('.badge').text(numLike);
      
        $('#frmRegIdea')[0].reset();
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetIdeaReply"),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        $('#tbReply tr:last').after(data[i]);
                    }
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetIdeaById"),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {

                    $('#btnRegIdea').attr('data-emp', $.trim(data[1].Value));
                    $('#title').val($.trim(data[2].Value));
                    $('#detail').val($.trim(data[3].Value));
                    $('#quantiative').val($.trim(data[4].Value));
                    $('#qualiative').val($.trim(data[5].Value));
                    $('#department').val($.trim(data[7].Value));

                    $('#division').val($.trim(data[8].Value));
                    $('#name').val($.trim(data[9].Value));
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        $("#mdRegIdea").modal({
            backdrop: 'static',
            keyboard: false
        });
        return false;
    });
    $('#tbNewPrj,#tbMainDefault').on('click', '.title', function () {
        var username = $('#username').val();
        var host = $(this).attr('data-emp');
        var role = $('#username').attr('data-role');
        if (!username) {
            $('#Reply,#Reply1,#btnLike,#btnLike1,#btnExport,#btnDelPrj').prop('disabled', true);
        }
        else {
            $('#Reply,#Reply1,#btnLike,#btnLike1,#btnExport').prop('disabled', false);
        }
        if (username != host) {
            $('#btnSaveProcess,#btnDelPrj').prop('disabled', true);
            if (role && parseInt(role) > 0) {
                $('#btnExport').prop('disabled', false);
            }
            if (role && parseInt(role) == 2) {
                $('#btnExport,#btnDelPrj').prop('disabled', false);
            }
        }
        else {
            $('#btnSaveProcess,#btnExport,#btnDelPrj').prop('disabled', false);
        }
        var numLike = parseInt($(this).parent().parent().find('.like').children().text());
        $('#btnLike1').find('.badge').text(numLike);

        $('#tbReply1').find("tr:not(:first)").remove();
        $('.target,.result').val('');
        $('#current').text('');
        $('.trPlan').remove();
        $('#trTarget td').remove();
        $('#trResult td').remove();
        $('#prjNYear').remove();
        var dt1 = new Date();
        dt1.setFullYear(dt1.getFullYear() - 1);

        $('#trTarget').append('<td>Target</td><td><select id="selCurent1"><option value="' + dt1.getFullYear().toString().substr(-2) + ' total">' + dt1.getFullYear().toString().substr(-2) + ' total</option><option value="' + dt1.getFullYear().toString().substr(-2) + ' 2nd">' + dt1.getFullYear().toString().substr(-2) + ' 2nd</option><option value="' + dt1.getFullYear().toString().substr(-2) + ' 4Q">' + dt1.getFullYear().toString().substr(-2) + ' 4Q</option></select></td>');
        $('#trResult').append('<td>Result</td><td><input type="number" step="0.01" id="txtCurrent1" value="" class="form-control"/></td>');

        $('#action_plan1').attr('rowspan', 2);

        var id = $(this).attr('data-id');
        $('#Reply1').attr('data-id', id);
        $('#frmProcess')[0].reset();
        $('#IDEA_ID').val(id);
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetKPI"),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                var date = new Date(data[0].PRJ_MONTH + "-01");
                var date1 = new Date(date.getFullYear() + 1, date.getMonth(), 1);

                var year = parseInt(data[0].PRJ_MONTH.split("-")[0]);
                $('#prjYear').text(year);
                $('#kpiMonth td').remove();

                var col1 = 0;
                var col2 = 0;
                for (var d = date; d <= date1 ; d.setMonth(d.getMonth() + 1)) {

                    var prj_month = moment(d).format('YYYY-MM');

                    $('#kpiMonth').append('<td>' + prj_month.split("-")[1] + '</td>');
                    $('#trTarget').append('<td><input data-id="" class="form-control target" data-month="' + prj_month + '" type="number" step="0.01" min="0" value=""/></td>');
                    $('#trResult').append('<td><input data-id="" class="form-control result" data-month="' + prj_month + '" type="number" min="0" value=""/></td>');
                    if (year == d.getFullYear()) {
                        col1++;
                    }
                    else {
                        col2++;
                    }
                }
                $('#prjYear').attr('colspan', col1);
                if (col2 > 0) {
                    if ($('#prjNYear').length == 0) {
                        $("<td colspan='" + col2 + "' id='prjNYear'>" + (parseInt(year) + 1) + "</td>").insertAfter('#prjYear');
                    }
                    else {
                        $('#prjNYear').attr('colspan', col2);
                    }
                }
                if (data.length > 0) {
                   
                    for (var i = 0; i < data.length; i++) {
                        $.each($(".target"), function () {
                            if ($(this).attr('data-month') == data[i].PRJ_MONTH) {
                                $(this).val(data[i].TARGET_VALUE);
                                $(this).attr('data-id', data[i].ID);

                            }

                        });
                        $.each($(".result"), function () {
                            var thisMonth = new Date($(this).attr('data-month') + "-01");
                            if ($(this).attr('data-month') == data[i].PRJ_MONTH) {
                                $(this).val(data[i].RESULT_VALUE);
                                $(this).attr('data-id', data[i].ID);
                                $(this).attr('readonly', false);
                            }
                            if (moment(thisMonth).isAfter(new Date())) {
                                $(this).attr('readonly', true);
                            }
                            else {
                                $(this).attr('readonly', false);
                            }
                        });

                    }

                }
              
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetProgressReply"),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        $('#tbReply1 tr:last').after(data[i]);
                    }
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
       
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetPlan"),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {
                    var rowspan = parseInt($('#action_plan1').attr('rowspan'));
                    $('#action_plan1').attr('rowspan', rowspan + data.length);
                    for (var i = 0; i < data.length; i++) {
                       
                        var complete_dt = moment(data[i].COMPLETE_DATE).format('YYYY-MM-DD') == "0001-01-01" ? "" : moment(data[i].COMPLETE_DATE).format('YYYY-MM-DD');
                        var plan_dt = moment(data[i].PLAN_DATE).format('YYYY-MM-DD') == "0001-01-01" ? "" : moment(data[i].PLAN_DATE).format('YYYY-MM-DD');
                        var tr = '<tr class="trPlan">'
                                + '<td colspan="1">'
                                    + '<select class="form-control complete" data-id="' + data[i].ID + '">';
                        if (data[i].COMPLETE_YN == 0) {
                            tr += '<option value="0" selected="selected">No</option>'
                            + '<option value="1">Yes</option>';
                        }
                        else {
                            tr += '<option value="0">No</option>'
                            + '<option value="1" selected="selected">Yes</option>';
                        }
                        tr += '</select>'
                    + '</td>'
                    + '<td colspan="1">'
                        + '<input type="text" class="form-control com_dt" value="' + complete_dt + '" /></td>'
                    + '<td colspan="2">'
                        + '<textarea class="form-control plan">' + $.trim(data[i].PLAN_CONTENTS) + '</textarea></td>'
                    + '<td colspan="1">'
                       + '<div class="input-group"><input type="text" class="form-control schedule" value="' + plan_dt + '"/>'
                       +'<div class="input-group-btn">'
                      + '<button data-id="' + data[i].ID + '" class="btn btn-danger removeplan" type="button">'
                     +                               '<i class="glyphicon glyphicon-minus-sign"></i>'
                     +                           '</button>'
                     +                       '</div>'
                     +                   '</div></td>'
                + '</tr>';
                       
                        $('.trPlan1').after(tr);
                    }
                  
                    $('.schedule,.com_dt')
                        .datepicker({
                            format: 'yyyy-mm-dd'
                        }).on('changeDate', function (ev) {
                            $(this).datepicker('hide');
                        });
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },

        });

        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetProjectById"),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {
                    $('#frmProcess').attr('data-id', $.trim(data[0].Value));
                    $('#frmProcess').attr('data-emp', $.trim(data[1].Value));
                    $('#frmProcess').attr('data-date', moment(data[8].Value).format("YYYY-MM-DD"));

                    $('#title1').val($.trim(data[2].Value));
                    $('#txtKPIName1').val($.trim(data[4].Value));
                    //$('#unit1').selectize()[0].selectize.setValue($.trim(data[5].Value), false);
                    $('#unit1').val($.trim(data[5].Value));
                    $('#remark').val($.trim(data[9].Value));
                    $('#current').text($.trim(data[10].Value));
                    $('#selCurent1').val($.trim(data[10].Value));
                    $('#txtCurrent1').val($.trim(data[11].Value));
                    $('#selDept1').val($.trim(data[12].Value));
                    //$('#division1').selectize()[0].selectize.setValue($.trim(data[13].Value), false);
                    $('#division1').val($.trim(data[13].Value));
                    $('#pit1').val($.trim(data[3].Value));
                    $('#txtName1').val($.trim(data[14].Value));
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        $("#mdProcess").modal({
            backdrop: 'static',
            keyboard: false
        });
        return false;
    });

    $('#btnAddPlan').on('click', function () {
        var rowspan = parseInt($('#action_plan').attr('rowspan')) + 1;
        $('#action_plan').attr('rowspan', rowspan);
        var plan = $.trim($('#txtAddPlan').val());
        var schedule = $.trim($('#txtAddSchedule').val()).length == 0 ? moment().format('YYYY-MM-DD') : $.trim($('#txtAddSchedule').val());
        $('<tr class="trNewPlan">'
                               + '<td colspan="4">'
                                    + ' <textarea class="form-control newplan" data-id="" placeholder="Plan">' + plan + '</textarea>'
                                     
                               + ' </td>'
                               + ' <td colspan="2">'
                                + '   <div class="input-group" style="width:100%">'
                                  + '  <input type="text" class="form-control newschedule" value="' + schedule + '" />'
                                   + '  <div class="input-group-btn">'
                                        + '    <button class="btn btn-danger btnRemovePlan" type="button" data-id="">'
                                          + '<i class="glyphicon glyphicon-minus-sign"></i>'
                                           + ' </button>'
                                       + ' </div>'
                                  + '  </div></td>'
                           + ' </tr>').insertAfter($(this).closest('tr'));
        $('.newschedule')
        .datepicker({
            format: 'yyyy-mm-dd'
        }).on('changeDate', function (ev) {
            $(this).datepicker('hide');
        });
        $('#txtAddPlan').val('');
        $('#txtAddSchedule').val('');
        return false;
    });
    $('#btnAddPlan1').on('click', function () {
        
        var yesno = $('#com1').val();
        var com_dt = $('#com_dt').val();
        //var com_dt = $.trim($('#txtAddSchedule1').val()).length == 0 ? moment().format('YYYY-MM-DD') : $.trim($('#txtAddSchedule1').val());
        var plan = $.trim($('#txtAddPlan1').val());
       
        var schedule = $.trim($('#txtAddSchedule1').val());
        var rowspan = parseInt($('#action_plan1').attr('rowspan')) + 1;
        $('#action_plan1').attr('rowspan', rowspan);
        $('<tr class="trPlan">'
                                + '<td colspan="1">'
                                    + '<select class="form-control complete" data-id="">'
       
            +'<option value="' + yesno + '" selected="selected">No</option>'
            + '<option value="1">Yes</option>'
        + '</select>'
    + '</td>'
    + '<td colspan="1">'
        + '<input type="text" class="form-control com_dt" value="' + com_dt + '" /></td>'
    + '<td colspan="2">'
        + '<textarea class="form-control plan">' + $.trim(plan) + '</textarea></td>'
    + '<td colspan="1">'
       + '<div class="input-group"><input type="text" class="form-control schedule" value="' + schedule + '"/>'
       + '<div class="input-group-btn">'
      + '<button data-id="" class="btn btn-danger removeplan" type="button">'
     + '<i class="glyphicon glyphicon-minus-sign"></i>'
     + '</button>'
     + '</div>'
     + '</div></td>'
+ '</tr>').insertAfter($(this).closest('tr'));
        $('.schedule,.com_dt')
        .datepicker({
            format: 'yyyy-mm-dd'
        }).on('changeDate', function (ev) {
            $(this).datepicker('hide');
        });
        $('#txtAddPlan1').val('');
        $('#txtAddSchedule1').val('');
        return false;
    });
    $('#frmProcess').on('click', '.removeplan', function () {
        var conf = confirm('Are you sure you want to remove this plan?');
        if (conf) {
            var id = $(this).attr('data-id');
            var username = $('#username').val();
            if (username != $('#frmProcess').attr('data-emp')) {
                alert('You cannot delete this plan');
                return false;
            }
            if (id) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "RemovePlan"),
                    data: JSON.stringify({
                        ID: id
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        if (data > 0) {
                            bootbox.alert(status);

                        }

                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
            }
            var rowspan = parseInt($('#action_plan1').attr('rowspan'));
            $('#action_plan1').attr('rowspan', rowspan - 1);
            $(this).closest('tr').remove();
        }
        return false;
    });
    $('#tbRegPrj').on('click', '.btnRemovePlan', function () {
        var id = $(this).attr('data-id');
        $(this).closest('tr').remove();
        return false;
    });
    $('#frmRegPrj').submit(function (e) {
        e.preventDefault();
        $('#mdRegPrj').modal('hide');
        var username = $('#username').val();
        if (username != $('#btnRegPrj').attr('data-emp')) {
            bootbox.alert("You cannot register this project");

            return false;
        }
        var checkNumber = false;
        var plans = [];
        var kpis = [];
        var project = {
            EMP_ID: $('#btnRegPrj').attr('data-emp'),
            IDEA_ID: $('#frmRegPrj').attr('data-id'),
            IDEA_TITLE: $('#txtTitle').val(),
            PRJECT_GRADE: $('#pit').val(),
            KPI_NAME: $('#txtKPIName').val(),
            //KPI_UNIT: $('#unit').selectize()[0].selectize.getValue(),
            KPI_UNIT: $('#unit').val(),
            BACKGROUND: $('#background').val(),
            NAME: $('#txtName').val(),
            PRJ_CURR: $('#selCurent').val(),
            CURR_VALUE: $('#txtCurrent').val(),
            ATTACHMENT: $('#download').length > 0 ?  $('#download').attr('href').split('=')[1] : ""
        };
        if (!project.IDEA_TITLE) {
            alert('Please enter project title');
            return false;
        }
        if (!project.KPI_NAME) {
            alert('Please enter kpi name');
            return false;
        }
        $('.kmonth').each(function (i, obj) {
            if (isNaN($(this).val())) {
                bootbox.alert("Please enter valid number");
                return false;
            }
            else {
                if ($(this).val()) {
                    var target = {
                        ID: "",
                        IDEA_ID: $('#frmRegPrj').attr('data-id'),
                        PRJ_MONTH: $(this).attr('data-month'),
                        TARGET_VALUE: $(this).val()
                    };
                    kpis.push(target);
                }
            }
        });
        if ($('#txtAddPlan').val().length > 0 && $('#txtAddSchedule').val().length > 0) {
            var p = {
                ID: "",
                IDEA_ID: $('#frmRegPrj').attr('data-id'),
                PLAN_CONTENTS: $('#txtAddPlan').val(),
                PLAN_DATE: $('#txtAddSchedule').val()
            };
            plans.push(p);
        }
        $.each($(".newplan"), function () {
            if ($(this).val().length > 0) {
                var plan = {
                    ID: "",
                    IDEA_ID: $('#frmRegPrj').attr('data-id'),
                    PLAN_CONTENTS: $(this).val(),
                    PLAN_DATE: $(this).closest('tr').find('.newschedule').val()
                };
                plans.push(plan);
            }
        });

        if (plans.length > 0 && kpis.length > 0) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "RegPrj"),
                data: JSON.stringify({
                    Project: project,
                    Plans: plans,
                    KPIs: kpis
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        bootbox.alert(status);
                        tbIdea.ajax.reload();
                        tbNewPrj.ajax.reload();
                        tbProgress.ajax.reload();
                        $('#mdRegPrj').modal('hide');
                    }
                    if (data == -1) {
                        bootbox.alert("Cannot send email. Register successfully.");
                        tbIdea.ajax.reload();
                        tbNewPrj.ajax.reload();
                        tbProgress.ajax.reload();
                        $('#mdRegPrj').modal('hide');
                    }
                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        else {
            bootbox.alert("Please enter project plans and kpi");
        }
        return false;
    });

    $('#btnSaveProcess').click(function () {
      
        var username = $('#username').val();
       
        if (username != $('#frmProcess').attr('data-emp')) {
            bootbox.alert("You cannot modify this project");
            return false;
        }

        var plans = [];
        var kpis = [];
        var project = {
            EMP_ID: $('#frmProcess').attr('data-emp'),
            IDEA_ID: $('#frmProcess').attr('data-id'),
            IDEA_TITLE: $('#title1').val(),
            KPI_NAME: $('#txtKPIName1').val(),
            KPI_UNIT: $('#unit1').val(),
            PRJECT_GRADE: $('#pit1').val(),
            REMARK: $('#remark').val(),
            NAME: $('#txtName1').val(),
            PRJ_CURR: $('#selCurent1').val(),
            CURR_VALUE: $('#txtCurrent1').val()
        };

        $.each($(".trPlan"), function () {
            var plan = {
                ID: $(this).find('.complete').attr('data-id'),
                IDEA_ID: project.IDEA_ID,
                COMPLETE_YN: $(this).find('.complete').val(),
                COMPLETE_DATE: $(this).find('.com_dt').val(),
                PLAN_CONTENTS: $(this).find('.plan').val(),
                PLAN_DATE: $(this).closest('tr').find('.schedule').val()
            };
            plans.push(plan);
        });

        $('.target').each(function (i, obj) {
            var index = $(this).index();
            if (isNaN($(this).val()))
            {
                bootbox.alert("Please enter valid number");
                return false;
            }
            else {
                if ($(this).val()) {
                    var res = $('.result:eq(' + $(this).index() + ')').val();
                    var target = {
                        ID: $(this).attr('data-id'),
                        IDEA_ID: project.IDEA_ID,
                        TARGET_VALUE: $(this).val(),
                        RESULT_VALUE: $('.result').eq(index).val(),
                        PRJ_MONTH: $(this).attr('data-month')

                    };
                    kpis.push(target);
                }
            }
        });
        $('.result').each(function (j, obj) {
            if (isNaN($(this).val())) {
                bootbox.alert("Please enter valid number");
                return false;
            }
            for (var i = 0; i < kpis.length; i++) {
                if ($(this).attr('data-month') == kpis[i].PRJ_MONTH) {
                    kpis[i].RESULT_VALUE = $(this).val();
                }
            }

        });
        if (kpis.length > 0 && plans.length > 0) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "UpdateProject"),
                data: JSON.stringify({
                    Project: project,
                    Plans: plans,
                    KPIs: kpis
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        bootbox.alert(status);
                        tbNewPrj.ajax.reload();
                        tbProgress.ajax.reload();
                        $('#mdProcess').modal('hide');
                    }

                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        else {
            bootbox.alert("Please enter plans and kpi");
        }
        return false;
    });

    $('#btnLike,#btnLike1').on('click', function () {
        var username = $('#username').val();
        var name = $('#username').attr('data-name');
        var id = $('#Reply').attr('data-id');
        var like = "LikeIdea";
        var button = "btnLike";
        if ($(this).attr('id') == "btnLike1") {
            id = $('#Reply1').attr('data-id');
            like = "Like";
            button = "btnLike1";
        }
        if (!id) {
            bootbox.alert('Cannot like empty idea');
            return false;
        }
        var numLike = parseInt($(this).children('.badge').text()) + 1;

        var EMP = {
            EMP_ID: username,
            EMP_NAME: name
        };
        $.ajax({
            url: $('#hdUrl').val().replace("Action", like),
            data: JSON.stringify({
                IDEA_ID: id,
                EMP: EMP
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data > 0) {
                    if (button == "btnLike") {
                        $('#btnLike').children('.badge').text(numLike);
                        $('#btnLike').prop('disabled', true);
                    }
                    else {
                        $('#btnLike1').children('.badge').text(numLike);
                        $('#btnLike1').prop('disabled', true);
                    }
                    tbIdea.ajax.reload();
                    tbNewPrj.ajax.reload();
                    tbProgress.ajax.reload();
                   
                }
                else {
                    bootbox.alert("You already liked it");
                }
               
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
    });
    $('#tbNewIdea,#tbMainDefault,#tbNewPrj').on('click', '.rep', function (ev) {
        ev.preventDefault();
        var num = parseInt($(this).children('span').text());
        if (num == 0) return false;

        var e = $(this);
        var id = e.attr('data-id');
        var table_id = 1;
        if (e.closest("#tbNewIdea").length > 0) {
            table_id = 0;
        }
        e.off('hover');
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetRepDetail"),
            data: JSON.stringify({
                IDEA_ID: id,
                table: table_id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {
                    var names = "";

                    for (var i = 0; i < data.length; i++) {
                        if (table_id == 0) {
                            for (var j = 0; j < data[i].length; j++) {
                                names += data[i][j].Value + "<br>";
                            }
                        }
                        else {
                            names += data[i].REP_EMP_NAME + "<br>";
                        }
                    }

                    e.popover({
                        content: names,
                        html: true,
                        placement: "left"
                    }).popover('show');
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });

        return false;
    });
    $('#tbNewIdea,#tbMainDefault,#tbNewPrj').on('click', '.like', function (ev) {
        ev.preventDefault();
        var num = parseInt($(this).children('span').text());
        if (num == 0) return false;

        var e = $(this);
        var id = e.attr('data-id');
        var table = e.attr('data-table');
        e.off('hover');
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetLikeDetail"),
            data: JSON.stringify({
                IDEA_ID: id,
                table: table
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {
                    var names = "";
                    for (var i = 0; i < data.length; i++) {
                        names += data[i].EMP_NAME + "<br>";
                    }
                    e.popover({
                        content: names,
                        html: true,
                        placement: "left"
                    }).popover('show');
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });

        return false;
    });
    $('#btnDelIdea').on('click', function () {
       
        var id = $(this).attr('data-ideaid');
        if (!id)
            return false;
        var cfm = confirm("Are you sure you want to delete this idea?");
        if (cfm) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "DeleteIdea"),
                data: JSON.stringify({
                    ID: id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        $("#mdRegIdea").modal('hide');
                        tbIdea.ajax.reload();
                    }
                    else {
                        bootbox.alert("Delete fail");
                    }

                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        return false;
    });
    
    $('#btnDelPrj').on('click', function () {

        var id = $('#frmProcess').attr('data-id');
        if (!id)
            return false;
        var cfm = confirm("Are you sure you want to delete this project?");
        if (cfm) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "DeleteProject"),
                data: JSON.stringify({
                    ID: id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        $("#mdProcess").modal('hide');
                        tbProgress.ajax.reload();
                        tbNewPrj.ajax.reload();
                        tbIdea.ajax.reload();
                    }
                    else {
                        bootbox.alert("Delete fail");
                    }

                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        return false;
    });
    $('#btnDelegate').on('click', function () {

        var emp_id = $('#txtDelegate').val();
        var id = $('#btnDelIdea').attr('data-ideaid');
      
        if (!emp_id) {
            alert('Please enter Employee ID');
            return false;
        }
        var cfm = confirm("Are you sure you want to delegate this idea?");
        if (cfm) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "Delegate"),
                data: JSON.stringify({
                    ID: id,
                    EMP_ID: emp_id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        bootbox.alert(status);
                        tbIdea.ajax.reload();
                        $('#mdRegIdea').modal('hide');
                    }
                    else {
                        bootbox.alert("Delegate fail");
                    }

                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        return false;
    });

    $("input:file#upload1").fileupload({
        dataType: "json",
        url: $('#hdUrl1').val().replace("Action", "Upload"),
        autoUpload: true,
        add: function (e, data) {
            var uploadErrors = [];
            var fileType = data.originalFiles[0].name.split('.').pop(), allowdtypes = 'xls,xlsx';
            if (allowdtypes.indexOf(fileType) < 0) {
                uploadErrors.push('Invalid file type. Only excel file allowed');

            }
            if (data.originalFiles[0]['size'].length && data.originalFiles[0]['size'] > 5000000) {
                uploadErrors.push('Filesize is too big. Maximum is 5 Mb');

            }
            if (uploadErrors.length > 0) {
                alert(uploadErrors.join("\n"));
            } else {
                data.submit();
            }
        },
        //send: function () {
        //    spinner.spin(target);
        //},
        done: function (e, data) {
            var errors = data.result.Errors;
            if (errors && errors.length) {
                alert(errors);
            } else {
                alert("Upload Success");


                var href = $('#hdUrl1').val().replace("Action", "Download") + "?file=" + data.result;

                var a = '<a id="download" href="' + href + '")">Download</a>';
                $('.upload').append(a);

            }
        },
        always: function () {
            //spinner.spin(false);
        }
    });
});
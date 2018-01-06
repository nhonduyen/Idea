$(document).ready(function () {
    $('#lhome').addClass("active");
    $('#txtAddSchedule')
        .datepicker({
            format: 'yyyy-mm-dd'
        }).on('changeDate', function (ev) {
            $(this).datepicker('hide');
        });

    var $div1 = $('#division,#division1,#unit').selectize({
        create: true,
        sortField: 'text'
    });
   
    var tbProgress = $('#tbMainDefault').DataTable(
            {
                sort: false,
                "processing": true,
                "serverSide": true,
                "searching": false,
                ajax: {
                    type: "POST",
                    contentType: "application/json",
                    url: $('#tbMainDefault').attr('data-url'),
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
                    url: $('#tbNewIdea').attr('data-url'),
                    data: function (d) {
                        // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                        return JSON.stringify({ dataTableParameters: d });
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
                   url: $('#tbNewPrj').attr('data-url'),
                   data: function (d) {
                       // note: d is created by datatable, the structure of d is the same with DataTableParameters model above
                       return JSON.stringify({ dataTableParameters: d });
                   }
               }

           });
    $('#btnUpload').on('click', function () {
        var username = $('#username').val();
        if (!username) {
            bootbox.alert("Please login to use this function");
        }
        $('#btnSetProject').prop('disabled', true);
        $('#frmRegIdea')[0].reset();
        $('#Reply').attr('data-id', '');
        $('#cNow').text(moment().format('YYYY-MM-DD HH:mm:ss'));
        var name = $('#username').attr('data-name');
        var dept = $('#username').attr('data-dept');
        var division = $('#username').attr('data-div');
        
        $('#division').selectize()[0].selectize.setValue(division, false);
        $('#cDept').text(dept);
        $('#cName').text(name);
        var now = moment().format('YYYY-MM-DD');
        $('#cNow').text(now);
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

        if (username != emp) {
            bootbox.alert("You cannot set this idea");
        }
        var id = $('#Reply').attr('data-id');
        $.ajax({
            url: $('#tbNewIdea').attr('data-GetIdeaById'),
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
        if (!comment) {
            bootbox.alert("Please enter comment");
            return false;
        }
        var reply = { IDEA_ID: ideaId, REP_EMP_ID: username, COMMENTS: comment };

        var seq = parseInt($('#tbReply tr:last-child td:first-child').text());
        $.ajax({
            url: $('#tbReply').attr('data-url'),
            data: JSON.stringify({
                reply: reply
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data > 0) {
                    $('#tbReply tr:last').after('<tr><td>' + (seq + 1) + '</td><td>' + $('#username').attr('data-dept') + '</td><td>' + $('#username').attr('data-name') + '</td><td>' + comment + '</td><td>' + moment().format('YYYY-MM-DD hh:mm:ss') + '</td></tr>');
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
        if (!comment) {
            bootbox.alert("Please enter comment");
            return false;
        }
        var reply = { IDEA_ID: ideaId, REP_EMP_ID: username, COMMENTS: comment };

        var seq = parseInt($('#tbReply1 tr:last-child td:first-child').text());
        $.ajax({
            url: $(this).attr('data-url'),
            data: JSON.stringify({
                reply: reply
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data > 0) {
                    $('#tbReply1 tr:last').after('<tr><td>' + (seq + 1) + '</td><td>' + $('#username').attr('data-dept') + '</td><td>' + $('#username').attr('data-name') + '</td><td>' + comment + '</td><td>' + moment().format('YYYY-MM-DD hh:mm:ss') + '</td></tr>');
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

        $('#comment1').val('');

        return false;
    });
    $('#frmRegIdea').submit(function (e) {
        e.preventDefault();
        var username = $('#username').val();
        var host = $('#btnRegIdea').attr('data-emp');
        var action = parseInt($('#btnRegIdea').attr('data-action'));
        var idea = {
            ID: $('#Reply').attr('data-id'),
            EMP_ID: $('#username').val(),
            IDEA_TITLE: $('#title').val(),
            DETAIL: $('#detail').val(),
            QUANTITATIVE: $('#quantiative').val(),
            QUALITATIVE: $('#qualiative').val()
        };
        var department = $('#department').val();
        var division = $('#division').selectize()[0].selectize.getValue();
        if (action == 1) {
            if (username != host) {
                bootbox.alert("You cannot modify this idea");
                return false;
            }
            $.ajax({
                url: $('#btnRegIdea').attr('data-url'),
                data: JSON.stringify({
                    idea: idea,
                    division: division,
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
            $.ajax({
                url: $('#btnRegIdea').attr('data-url'),
                data: JSON.stringify({
                    idea: idea,
                    division: division,
                    department:department,
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

        $("#mdRegIdea").modal('hide');
        return false;
    });
    $('#tbNewIdea').on('click', '.title', function () {
        $('#btnSetProject').prop('disabled', false);
        var id = $(this).attr('id');
        $('#Reply').attr('data-id', id);
        $('#btnRegIdea').attr('data-action', 1);
        $('#tbReply tbody').children('tr:not(:first)').remove();
        $('#cDept').text($('#username').attr('data-dept'));
        $('#cName').text($('#username').attr('data-name'));
        $('#cNow').text(moment().format('YYYY-MM-DD HH:mm:ss'));
        $('#frmRegIdea')[0].reset();
        $.ajax({
            url: $('#tbNewIdea').attr('data-url-getreply'),
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
            url: $('#tbNewIdea').attr('data-GetIdeaById'),
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
                   
                    $('#division').selectize()[0].selectize.setValue($.trim(data[8].Value), false);
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
        $('#tbReply1 tbody').children('tr:not(:first)').remove();
        $('.target,.result').val('');
        $('.trPlan').remove();
        $('#action_plan1').attr('rowspan', 1);

        var id = $(this).attr('data-id');
        $('#Reply1').attr('data-id', id);

        $('#tbReply1 tbody').children('tr:not(:first)').remove();
        $('#cDept1').text($('#username').attr('data-dept'));
        $('#cName1').text($('#username').attr('data-name'));
        $('#cNow1').text(moment().format('YYYY-MM-DD HH:mm:ss'));
        $('#frmProcess')[0].reset();
        $.ajax({
            url: $('#tbReply1').attr('data-getreply'),
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
            url: $('#tbProcess').attr('data-plan'),
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
                        var rowspan = parseInt($('#action_plan1').attr('rowspan'));
                        var complete_dt = moment(data[i].COMPLETE_DATE).format('YYYY-MM-DD') == "0001-01-01" ? "" : moment(data[i].COMPLETE_DATE).format('YYYY-MM-DD');
                        var plan_dt = moment(data[i].PLAN_DATE).format('YYYY-MM-DD') == "0001-01-01" ? "" : moment(data[i].PLAN_DATE).format('YYYY-MM-DD');
                        var tr = '<tr class="trPlan">'
                                + '<td colspan="1">'
                                    + '<select class="form-control complete" data-id="'+data[i].ID+'">';
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
                        + '<input type="text" class="form-control plan" value="' + $.trim(data[i].PLAN_CONTENTS) + '" /></td>'
                    + '<td colspan="1">'
                       + ' <input type="text" class="form-control schedule" value="' + plan_dt + '"/></td>'
                + '</tr>';
                        $('#action_plan1').parent('tr').after(tr);
                        $('#action_plan1').attr('rowspan', rowspan + 1);
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
            url: $('#tbProcess').attr('data-kpi'),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {
                    var year = data[0].PRJ_MONTH.split("-")[0];
                    $('#prjYear').text(year);
                    for (var i = 0; i < data.length; i++) {
                        var month = parseInt(data[i].PRJ_MONTH.split("-")[1]) - 1;
                        $('.target:eq(' + month + ')').val(data[i].TARGET_VALUE);
                        $('.target:eq(' + month + ')').attr('data-id', data[i].ID);
                        $('.target:eq(' + month + ')').attr('data-month', $.trim(data[i].PRJ_MONTH));
                        $('.result:eq(' + month + ')').val(data[i].RESULT_VALUE);
                        $('.result:eq(' + month + ')').attr('data-id', data[i].ID);
                    }
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        $.ajax({
            url: $('#tbProcess').attr('data-getprj'),
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
                    $('#title1').val($.trim(data[2].Value));
                    $('#txtKPIName1').val($.trim(data[4].Value));
                    $('#unit1').selectize()[0].selectize.setValue($.trim(data[5].Value), false);
                    $('#remark').val($.trim(data[9].Value));
                    $('#selDept1').val($.trim(data[10].Value));
                    $('#division1').selectize()[0].selectize.setValue($.trim(data[10].Value), false);


                    $('#txtName1').val($.trim(data[12].Value));
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
        $('<tr>'
                               + '<td colspan="4">'
                                 + '   <div class="input-group" style="width:100%">'
                                    + ' <input type="text" class="form-control newplan" data-id="" value="' + plan + '" placeholder="Plan">'
                                      + '  <div class="input-group-btn">'
                                        + '    <button class="btn btn-danger btnRemovePlan" type="button" data-id="">'
                                          + '<i class="glyphicon glyphicon-minus-sign"></i>'
                                           + ' </button>'
                                       + ' </div>'
                                  + '  </div>'
                               + ' </td>'
                               + ' <td colspan="2">'
                                  + '  <input type="text" class="form-control newschedule" value="' + schedule + '" /></td>'
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
    $('#tbRegPrj').on('click', '.btnRemovePlan', function () {
        var id = $(this).attr('data-id');
        $(this).closest('tr').remove();
        return false;
    });
    $('#frmRegPrj').submit(function (e) {
        e.preventDefault();

        var username = $('#username').val();
        if (username != $('#btnRegPrj').attr('data-emp')) {
            bootbox.alert("You cannot register this project");
            return false;
        }
       
        var plans = [];
        var kpis = [];
        var project = {
            EMP_ID: $('#btnRegPrj').attr('data-emp'),
            IDEA_ID: $('#frmRegPrj').attr('data-id'),
            IDEA_TITLE: $('#txtTitle').val(),
            PRJECT_GRADE: $('#pit').val(),
            KPI_NAME: $('#txtKPIName').val(),
            KPI_UNIT:  $('#unit').selectize()[0].selectize.getValue(),
            BACKGROUND: $('#background').val(),
            NAME: $('#txtName').val(),

        };

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

        $('.kmonth').each(function (i, obj) {
            if ($(this).val().length > 0) {
                var target = {
                    ID: "",
                    IDEA_ID: $('#frmRegPrj').attr('data-id'),
                    PRJ_MONTH: $(this).attr('data-month'),
                    TARGET_VALUE: $(this).val()
                };
                kpis.push(target);
            }
        });
        $.ajax({
            url: $('#frmRegPrj').attr('data-url'),
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

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        return false;
    });

    $('#frmProcess').submit(function (e) {
        e.preventDefault();

        var username = $('#username').val();
        if (username != $(this).attr('data-emp')) {
            bootbox.alert("You cannot modify this project");
            return false;
        }
      
        var plans = [];
        var kpis = [];
        var project = {
            EMP_ID: $(this).attr('data-emp'),
            IDEA_ID: $(this).attr('data-id'),
            IDEA_TITLE: $('#title1').val(),
            KPI_NAME: $('#txtKPIName1').val(),
            KPI_UNIT: $('#unit1').selectize()[0].selectize.getValue(),
            REMARK: $('#remark').val(),
            NAME: $('#txtName1').val(),

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
            if ($(this).val().length > 0) {
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
        });
        $('.result').each(function (j, obj) {
            for (var i = 0; i < kpis.length; i++) {
                if ($(this).attr('data-id') == kpis[i].ID) {
                    kpis[i].RESULT_VALUE = $(this).val();
                }
            }
            
        });
      
        $.ajax({
            url: $(this).attr('data-url'),
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
        return false;
    });

    $('#tbMainDefault').on('click', '.res', function () {
        $('.target1,.result1').text('');
        var id = $(this).attr('data-id');
        $.ajax({
            url: $('#tbMainDefault').attr('data-kpi'),
            data: JSON.stringify({
                ID: id
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data.length > 0) {
                    var year = data[0].PRJ_MONTH.split("-")[0];
                    $('#prjYear1').text(year);
                    for (var i = 0; i < data.length; i++) {
                        var month = parseInt(data[i].PRJ_MONTH.split("-")[1]) - 1;
                        $('.target1:eq(' + month + ')').text(data[i].TARGET_VALUE);
                        $('.result1:eq(' + month + ')').text(data[i].RESULT_VALUE);
                    }
                    $('#mdResult').modal();
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
    });
});
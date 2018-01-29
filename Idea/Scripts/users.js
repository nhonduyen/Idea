$(document).ready(function () {
    $('#btnNew').click(function () {
        $('#frmModify')[0].reset();
        $('#frmModify').attr("data-id", "0");
        $('#btnResetPass').prop('disabled', true);
        $("#mdEmp").modal({
            backdrop: 'static',
            keyboard: false
        });
        return false;
    });
    $('#btnResetPass').click(function () {
        EMP_ID = $('#frmModify').attr("data-id");
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "ResetPassword"),
            data: JSON.stringify({
                EMP_ID: EMP_ID
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data > 0) {
                    bootbox.alert("Success. Password is 123");
                    $("#mdEmp").modal('hide');
                }
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
    
        return false;
    });
    $('#frmModify').submit(function () {
        
        var action = $('#frmModify').attr("data-id");
        var EMP = {
            EMP_ID: $('#txtId').val(),
            EMP_NAME: $('#txtName').val(),
            DIVISION: $('#ddlDivision').val(),
            DEPARTMENT: $('#ddlDepartment').val(),
            EMAIL: $('#txtEmail').val(),
            ROLE: $('#ddlRole').val()
        };
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "InsertUser"),
            data: JSON.stringify({
                EMP: EMP,
                Action: action
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data > 0) {
                    tbUser.ajax.reload();
                    $("#mdEmp").modal('hide');
                    bootbox.alert("Success");
                }
                if (data == -1) {
                    $("#mdEmp").modal('hide');
                    bootbox.alert("User exists!");
                }
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        return false;
    });
    $('#tbMainDefault').on('click', '.empId', function () {
        var EMP_ID = $(this).attr("data-emp");
        $('#btnResetPass').prop('disabled', false);
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetUserById"),
            data: JSON.stringify({
                EMP_ID: EMP_ID
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data) {
                    $('#frmModify').attr("data-id", data.EMP_ID);
                    $('#txtId').val($.trim(data.EMP_ID));
                    $('#txtName').val($.trim(data.EMP_NAME));
                    $('#ddlDivision').val($.trim(data.DIVISION));
                    $('#ddlDepartment').val($.trim(data.DEPARTMENT));
                    $('#txtEmail').val($.trim(data.EMAIL));
                    $('#ddlRole').val(data.ROLE);
                    $("#mdEmp").modal({
                        backdrop: 'static',
                        keyboard: false
                    });
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        return false;
    });
    var tbUser = $('#tbMainDefault').DataTable(
           {
               sort: false,
               "processing": true,
               "serverSide": true,
               "searching": true,
               ajax: {
                   type: "POST",
                   contentType: "application/json",
                   url: $('#hdUrl').val().replace("Action", "GetUsers"),
                   data: function (d) {
                       // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                       return JSON.stringify({ dataTableParameters: d });
                   }
               }

           });

});
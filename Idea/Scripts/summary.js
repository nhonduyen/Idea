$(document).ready(function () {
    $('#lsum').addClass("active");
    $('#txtFrom,#txtFrom1,#txtTo,#txtTo1')
       .datepicker({
           format: 'yyyy-mm-dd'
       }).on('changeDate', function (ev) {
           $(this).datepicker('hide');
       });
    $('#btnSearch').on('click', function () {
        if ($('#txtTo').val() && $('#txtFrom').val()) {
            tbNewPrj.draw();
        }
        else
            bootbox.alert("Please select date range");
        return false;
    });
    $('#btnSearch1').on('click', function () {
        if ($('#txtTo1').val() && $('#txtFrom1').val()) {
            tbIdea.draw();
        }
        else
            bootbox.alert("Please select date range");
        return false;
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
                   url: $('#hdUrl').val().replace("Action", "GetIdeaSum"),
                   data: function (d) {
                       // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                       return JSON.stringify({ dataTableParameters: d, from: $('#txtFrom1').val(), to: $('#txtTo1').val() });
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
                   url: $('#hdUrl').val().replace("Action", "GetProjectSum"),
                   data: function (d) {
                       // note: d is created by datatable, the structure of d is the same with DataTableParameters model above
                       return JSON.stringify({
                           dataTableParameters: d,
                           from: $('#txtFrom').val(),
                           to: $('#txtTo').val()
                       });
                   }
               },

           });
});
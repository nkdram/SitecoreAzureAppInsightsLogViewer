var azurelogviewer = (function ($) {

    var handleAjax = function (url, success, postData) {
        if (postData !== undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: postData,
                success: success,
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                dataType: "json"
            });
        }
        else {
            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                dataType: "json",
                success: success,
                error: function (data) {
                    console.log(data);
                }
            });

        }
    };


    function getCookie(name) {
        var value = "; " + document.cookie;
        var parts = value.split("; " + name + "=");
        if (parts.length == 2) return parts.pop().split(";").shift();
    }

    var _azureLogViewer = {
        table: {},
        endPoints: {
            "getCloudInstaces": "/api/AzureLogs/AzureLogViewer/GetCloudInstances",
            "postLogRequest": "/api/AzureLogs/AzureLogViewer/GetLogData"
        },
        postData: {
            "LogType": "",
            "FromDate": "",
            "ToDate": "",
            "CloudInstance": "",
            "ExpressionType": "",
            "Message": "",
            "Limit": "5000"
        },
        datePickerLabel: "",
        renderTable: function () {
            _azureLogViewer.loader(true);
            //Call renderer ONLY after the DOM is loaded
            handleAjax(_azureLogViewer.endPoints.postLogRequest, function (data) {
                console.log(data);
                _azureLogViewer.table.clear().draw(false);
                if (data.NumberOfRows && data.NumberOfRows > 0) {
                    for (var i = 0; i < data.NumberOfRows; i++) {
                        _azureLogViewer.table.row.add([
                            i,
                            moment(data.Logs[i].TimeStamp).format('LLLL'),
                            data.Logs[i].LogType,
                            data.Logs[i].LogMessage
                        ]).draw(false);
                    }
                }
                _azureLogViewer.loader(false);
            }, _azureLogViewer.postData);

        },
        getLogTypes: function () {
            _azureLogViewer.postData.LogType = "";
            $('.btn-group .form-check-label.active').each(function (elem) {
                if (_azureLogViewer.postData.LogType === "")
                    _azureLogViewer.postData.LogType += $(this).find('.form-check-input').val();
                else
                    _azureLogViewer.postData.LogType += "," + $(this).find('.form-check-input').val();
            });
        },
        getExpAndMessage: function () {
            _azureLogViewer.postData.ExpressionType = $('#messgeExpression').val();
            _azureLogViewer.postData.Message = $('#message').val();
        },
        initializeDatePicker: function () {
            $('#daterangepicker').daterangepicker({
                "timePicker": true,
                ranges: {
                    'Last-30-Mins': [moment().subtract(30, 'minutes'), moment()],
                    'Last-Hour': [moment().subtract(1, 'hours'), moment()],
                    'Today': [moment().startOf('day'), moment()],
                    'Yesterday': [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                },
                locale: {
                    format: 'ddd MMM DD YYYY HH:mm:ss z'
                },
                "startDate": moment(),
                "endDate": moment(),
                "minDate": moment().subtract(1, 'year'),
                "maxDate": moment()
            }, function (start, end, label) {
                console.log('New date range selected: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD') + ' (predefined range: ' + label + ')');
                _azureLogViewer.postData.FromDate = start.format();
                _azureLogViewer.postData.ToDate = end.format();
                if (label !== 'Custom Range') {
                    $(this).val(label);
                    _azureLogViewer.datePickerLabel = label;
                }
                else {
                    _azureLogViewer.datePickerLabel = start.format('YYYY-MM-DD HH:mm:ss z') + ' to ' + end.format('YYYY-MM-DD HH:mm:ss z');
                }
            });
            $('#daterangepicker').on('apply.daterangepicker', function (ev, picker) {
                if (picker.label !== 'Custom Range') {
                    $(this).val(_azureLogViewer.datePickerLabel);
                }
            });


            $(document).on('click', '.cloud-instances button', function () {
                $('#dropdownMenuMenu').text($(this).text());
                $('#dropdownMenuMenu').attr('value', $(this).attr('value'));
                _azureLogViewer.postData.CloudInstance = $(this).attr('value');

            });

            $('#log-viewer-form').submit(function (e) {
                e.preventDefault();

                _azureLogViewer.postData.Limit = $('#form-limit').val();
                //Get post data ready
                _azureLogViewer.getLogTypes();
                _azureLogViewer.getExpAndMessage();
                //End Post Data

                //Rendering table
                _azureLogViewer.renderTable();
            });

        },
        getCloudInstaces: function () {
            _azureLogViewer.loader(true);
            handleAjax(this.endPoints.getCloudInstaces, function (data) {
                console.log(data);
                $('.cloud-instances').html('');
                if (data.NumberOfRows && data.NumberOfRows > 0) {
                    for (var i = 0; i < data.NumberOfRows; i++) {
                        $('.cloud-instances').append('<button class="dropdown-item" value="' + data.Logs[i].CloudRole + '" type="button">' + data.Logs[i].InstanceName + " - " + data.Logs[i].CloudRole + '</button>');
                    }

                    //init first option
                    $('#dropdownMenuMenu').val(data.Logs[0].CloudRole);
                    $('#dropdownMenuMenu').text(data.Logs[0].InstanceName + " - " + data.Logs[0].CloudRole);
                    _azureLogViewer.postData.CloudInstance = data.Logs[0].CloudRole;
                }
                _azureLogViewer.loader(false);
            });
        },
        loader: function (show) {
            if (show === true)
                $('div.loading').show();
            else
                $('div.loading').hide();
        },
        init: function () {
            this.initializeDatePicker();
            this.getCloudInstaces();
            this.loader(false);

            _azureLogViewer.table = $('#tablePreview').DataTable({
                "createdRow": function (row, data, dataIndex) {
                    var rowclass = "table-primary";
                    if (data[2] == "2")
                        rowclass = "table-warning";
                    if (data[2] == "3")
                        rowclass = "table-danger";
                    $(row).addClass(rowclass);
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "20%", "targets": 1 },
                    { "width": "5%", "targets": 2 }
                ],
                dom: 'Bfrtip',
                buttons: [
                    'copy', 'csv', 'excel'
                ],
                "pageLength": 50
            });

        }

    };
    _azureLogViewer.init();
    return _azureLogViewer;
})(jQuery);
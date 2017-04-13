﻿var DoIzendaConfig = function () {
    var hostApi = "http://localhost:8200/api/";
    var configJson = {
        "WebApiUrl": hostApi,
        "BaseUrl": "/izenda",
        "RootPath": "/Scripts/izenda",
        "CssFile": "izenda-ui.css",
        "Routes": {
            "Settings": "settings",
            "New": "new",
            "Dashboard": "dashboard",
            "Report": "report",
            "ReportViewer": "reportviewer",
            "ReportViewerPopup": "reportviewerpopup",
            "Viewer": "viewer"
        },
        "Timeout": 3600
    };
    IzendaSynergy.config(configJson);

};

function errorFunc() {
    alert('Token was not generated correctly. Please login.');

    // confirm dialog
    alertify.confirm("Your token was not generated correctly, please login.", function () {
        // user clicked "ok"
    }, function() {
        // user clicked "cancel"
    });
}

var DoRender = function (successFunc) {
    $.ajax({
        type: "GET",
        url: "/user/GenerateToken",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: successFunc,
        error: errorFunc
    });
};



var izendaInit = function () {
    function successFunc(data, status) {
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        IzendaSynergy.render(document.getElementById('izenda-root'));
    }

    this.DoRender(successFunc);

};

var izendaInitReport = function () {

    function successFunc(data, status) {
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        IzendaSynergy.renderReportPage(document.getElementById('izenda-root'));
    }

    this.DoRender(successFunc);

};

var izendaInitSetting = function () {

    function successFunc(data, status) {
        console.info(data);
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        IzendaSynergy.renderSettingPage(document.getElementById('izenda-root'));
    }

    this.DoRender(successFunc);
};

//var izendaInitReportPart = function () {
//
//    function successFunc(data, status) {
//        console.info(data);
//        var currentUserContext = {
//            token: data.token
//        };
//
//        IzendaSynergy.setCurrentUserContext(currentUserContext);
//        IzendaSynergy.renderReportPart(document.getElementById('izenda-part1'), {
//            "id": "804B35C8-44A4-4535-A484-F27E8ABA410D"
//        });
//    }
//
//    this.DoRender(successFunc);
//};
var izendaInitReportPart = function (reportParts) {

    function successFunc(data, status) {
        console.info(data);
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        for (var i = 0; i < reportParts.length; i++) {
            if (reportParts[i].overridingFilterValue) {
                IzendaSynergy.renderReportPart(document.getElementById(reportParts[i].selector), {
                    "id": reportParts[i].id,
                    "overridingFilterValue": reportParts[i].overridingFilterValue,
                });
            }
            else {
                IzendaSynergy.renderReportPart(document.getElementById(reportParts[i].selector), {
                    "id": reportParts[i].id
                });
            }

        }
    }

    this.DoRender(successFunc);
};


var izendaInitReport = function () {

    function successFunc(data, status) {
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        IzendaSynergy.renderReportPage(document.getElementById('izenda-root'));
    }

    this.DoRender(successFunc);

};

//var izendaInitReportViewer = function () {
//
//    function successFunc(data, status) {
//        var currentUserContext = {
//            token: data.token
//        };
//
//        IzendaSynergy.setCurrentUserContext(currentUserContext);
//        IzendaSynergy.renderReportViewerPage(document.getElementById('izenda-root'), "C2946606-7159-4FB3-82B7-E7D4ED3162A0");
//    }
//
//    this.DoRender(successFunc);
//
//};

// Render report viewer to a <div> tag by report id
var izendaInitReportViewer = function (reportId) {
    function successFunc(data, status) {
        var currentUserContext = {
            token: data.token
        };
        IzendaSynergy.setCurrentUserContext(currentUserContext);
        IzendaSynergy.renderReportViewerPage(document.getElementById('izenda-root'), reportId);
    }

    this.DoRender(successFunc);

};

var izendaInitReportCustomFilters = function (reportObject) {

    function successFunc(data, status) {
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        if (reportObject.filtersObj) {
            IzendaSynergy.renderReportViewerPage(document.getElementById(reportObject.selector), reportObject.id, reportObject.filtersObj);
        }
        else {
            IzendaSynergy.renderReportViewerPage(document.getElementById(reportObject.selector), reportObject.id);
        }
    }

    this.DoRender(successFunc);

};

var izendaInitDashboard = function () {

    function successFunc(data, status) {
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        IzendaSynergy.renderDashboardPage(document.getElementById('izenda-root'));
    }

    this.DoRender(successFunc);

};

var izendaInitReportDesigner = function () {

    function successFunc(data, status) {
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        IzendaSynergy.renderReportDesignerPage(document.getElementById('izenda-root'));
        
    }

    this.DoRender(successFunc);

};

var izendaInitNewDashboard = function () {

    function successFunc(data, status) {
        var currentUserContext = {
            token: data.token
        };

        IzendaSynergy.setCurrentUserContext(currentUserContext);
        IzendaSynergy.renderNewDashboardPage(document.getElementById('izenda-root'));
    }

    this.DoRender(successFunc);

};


var izendaInitReportPartExportViewer = function (reportPartId, token) {
    var currentUserContext = {
        token: token
    };
    IzendaSynergy.setCurrentUserContext(currentUserContext);
    IzendaSynergy.renderReportPart(document.getElementById('izenda-root'), {
        id: reportPartId,
        useQueryParam: true,
        useHash: false
    });
};
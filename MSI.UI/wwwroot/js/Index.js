$(document).ready(function () {
    GetUserLogin();
});

function GetUserLogin() {
    var options = {};
    options.url = "https://localhost:44332/api/User/GetDataUser";
    options.type = "GET";
    options.beforeSend = function (request) {
        request.setRequestHeader("Authorization",
            "Bearer " + sessionStorage.getItem("token"));
    };
    options.dataType = "json";
    options.success = function (data) {
        console.log(data.firstName);
        $('#lblHello').text('Hello ' + data.firstName + ' ' + data.lastName); 
    };
    options.error = function (a, b, c) {
        alert('Error Occured When Calling API');
    };
    $.ajax(options);
}
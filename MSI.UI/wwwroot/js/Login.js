
$(document).ready(function () {
    $('#btnLogin').on('click', function (e) {
        var _username = $('#username').val();
        var _password = $('#password').val();
        var model = {
            Username: _username,
            Password: _password
        }
       
        $.ajax({
            type: "POST",
            url: 'https://localhost:44332/api/User/login',
            contentType: 'application/json; charset=utf-8',
            dataType: "json",
            data: JSON.stringify(model),
            error: function (response) {
                alert(response.error);
            },
            success: function (response) {
                //$.cookie('token', data.token)
                sessionStorage.setItem("token", response.token);
                location.href = '/Home/Index';
            }
        });
    });
});
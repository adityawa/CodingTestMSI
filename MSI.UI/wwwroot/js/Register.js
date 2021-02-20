$(document).ready(function () {
    $('#btnRegister').on('click', function (e) {
        var valid = this.form.checkValidity(); 
        if (valid) {
            event.preventDefault(); 
            var _gender;
            if ($("#rbMale").is(":checked")) {
                _gender = 'M';
            }
            else if ($("#rbFemale").is(":checked")) {
                _gender = 'F';
            }

            var model = {
                FirstName: $('#firstName').val(),
                LastName: $('#lastName').val(),
                Email: $('#email').val(),
                Password: $('#password').val(),
                PhoneNumber: $('#phoneNumber').val(),
                Gender: _gender,
                DOB: $('#dob').val()
            }

            $.ajax({
                type: "POST",
                url: 'https://localhost:44332/api/User/register',
                contentType: 'application/json; charset=utf-8',
                dataType: "json",
                data: JSON.stringify(model),
                error: function (response) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Failed',
                        text: response.responseJSON.message,

                    })
                },
                success: function (response) {
                    Swal.fire({
                        title: 'Success',
                        text: "User Successfully Registered",
                        icon: 'success',
                        
                       
                        
                        confirmButtonText: 'Ok!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            location.href = '/Account/Login';
                        }
                    })
                }
            });
        }
      
    });
   
  
});
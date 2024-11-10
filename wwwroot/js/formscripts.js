function onBlazorReadyLog() {
    // login formos post
    $('#login-form').submit(function (event) {
        event.preventDefault();

        var formData = {
            email: $('#email').val(),
            password: $('#password').val()
        };

        var url = '/login?useCookies=true&useSessionCookies=true';

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                location.reload();
                console.log('Login successful:', response);
            },
            error: function (error) {
                alert("Bad username or password.");
                console.log('Login failed:', error);
            }
        });
    });
};

function onBlazorReadyReg() {
    // registration formos post
    $('#registration-form').submit(function (event) {
        event.preventDefault();

        var formData = {
            "email": $('#email').val(),
            "userName": $('#username').val(),
            "password": $('#password').val()
        };

        var url = '/register';

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                console.log('Registration successful:', response);
                alert('Registration successful');
            },
            error: function (error) {
                const errs = error.responseJSON.errors;
                console.log('Registration failed:', errs);
                let errorMessage = "The following errors occurred:\n";

                for (const key in errs) {
                    if (errs.hasOwnProperty(key)) {
                        errorMessage += `${key}: ${errs[key].join(", ")}\n`;
                    }
                }

                alert(errorMessage);
            }
        });
    });
};
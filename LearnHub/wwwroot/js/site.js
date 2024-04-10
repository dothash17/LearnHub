function showLogin() {
    document.getElementById("loginForm").style.display = "block";
    document.getElementById("registerForm").style.display = "none";
    document.querySelector('#loginModal .full-width-btn.active').classList.remove('active');
    document.querySelector('#loginModal .full-width-btn:nth-child(1)').classList.add('active');
}

function showRegister() {
    document.getElementById("loginForm").style.display = "none";
    document.getElementById("registerForm").style.display = "block";
    document.querySelector('#loginModal .full-width-btn.active').classList.remove('active');
    document.querySelector('#loginModal .full-width-btn:nth-child(2)').classList.add('active');
}

function validateLoginForm() {
    var formElement = document.getElementById("loginForm");
    var formData = new FormData(formElement);

    fetch(formElement.action, {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (response.ok) {
            window.location.href = '/Home/Index';
        } else {
            return response.text();
        }
    })
    .then(errorText => {
        if (errorText) {
            document.getElementById("loginError").innerText = errorText;
            $('#loginModal').modal('show');
        }
    });
}

function validateRegisterForm() {
    var formElement = document.getElementById("registerForm");
    var formData = new FormData(formElement);

    if (!formElement.checkValidity()) {
        formElement.reportValidity();
        return;
    }

    fetch(formElement.action, {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (response.ok) {
            window.location.href = '/Home/Index';
        } else {
            return response.text();
        }
    })
    .then(errorText => {
        if (errorText) {
            document.getElementById("registerError").innerText = errorText;
            $('#loginModal').modal('show');
        }
    });
}
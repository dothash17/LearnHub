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
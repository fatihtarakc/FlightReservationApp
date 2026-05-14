document.addEventListener('DOMContentLoaded', function () {

    // Auto-dismiss alerts after 5 seconds
    setTimeout(function () {
        document.querySelectorAll('.alert.alert-dismissible').forEach(function (el) {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(el);
            if (bsAlert) bsAlert.close();
        });
    }, 5000);

    // Password show/hide toggle (generic, for any page using #togglePw)
    var togglePwBtn = document.getElementById('togglePw');
    var togglePwIcon = document.getElementById('togglePwIcon');
    var pwInput = document.getElementById('pw-input') || document.getElementById('Password');

    if (togglePwBtn && pwInput) {
        togglePwBtn.addEventListener('click', function () {
            var show = pwInput.type === 'password';
            pwInput.type = show ? 'text' : 'password';
            if (togglePwIcon) {
                togglePwIcon.className = show ? 'bi bi-eye-slash' : 'bi bi-eye';
            }
        });
    }

    // Confirm booking/cancel dialogs
    document.querySelectorAll('[data-confirm]').forEach(function (el) {
        el.addEventListener('click', function (e) {
            if (!window.confirm(el.dataset.confirm)) {
                e.preventDefault();
            }
        });
    });

});

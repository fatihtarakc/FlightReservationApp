(function () {
    'use strict';

    var btn = document.getElementById('signInPanelBtn');
    var panel = document.getElementById('signInPanel');
    var closeBtn = document.getElementById('signInPanelClose');
    var form = document.getElementById('signInPanelForm');
    var errorEl = document.getElementById('signInPanelError');
    var submitBtn = document.getElementById('signInPanelSubmit');

    if (!btn || !panel) return;

    var genericError = panel.dataset.errorGeneric || 'Bir hata oluştu.';

    function positionPanel() {
        var r = btn.getBoundingClientRect();
        panel.style.top = (r.bottom + 6) + 'px';
        var right = window.innerWidth - r.right;
        panel.style.right = right + 'px';
        panel.style.left = 'auto';
    }

    function openPanel() {
        positionPanel();
        panel.style.display = 'block';
        var inp = document.getElementById('siUsername');
        if (inp) inp.focus();
    }

    function closePanel() {
        panel.style.display = 'none';
        if (errorEl) errorEl.style.display = 'none';
    }

    btn.addEventListener('click', function (e) {
        e.stopPropagation();
        panel.style.display === 'block' ? closePanel() : openPanel();
    });

    window.openSignInPanel = function () {
        openPanel();
    };

    if (closeBtn) closeBtn.addEventListener('click', closePanel);

    document.addEventListener('click', function (e) {
        if (panel.style.display !== 'block') return;
        if (panel.contains(e.target) || e.target === btn) return;
        closePanel();
    });

    document.addEventListener('keydown', function (e) { if (e.key === 'Escape') closePanel(); });
    window.addEventListener('resize', function () { if (panel.style.display === 'block') positionPanel(); });

    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            if (errorEl) errorEl.style.display = 'none';
            if (submitBtn) { submitBtn.disabled = true; submitBtn.style.opacity = '.7'; }

            fetch('/api/account/signin-panel', {
                method: 'POST',
                body: new FormData(form)
            })
            .then(function (r) { return r.json(); })
            .then(function (res) {
                if (res.success) {
                    window.location.href = res.redirectUrl || '/';
                } else {
                    if (errorEl) {
                        errorEl.textContent = res.message || genericError;
                        errorEl.style.display = 'block';
                    }
                    if (submitBtn) { submitBtn.disabled = false; submitBtn.style.opacity = '1'; }
                }
            })
            .catch(function () {
                if (errorEl) {
                    errorEl.textContent = genericError;
                    errorEl.style.display = 'block';
                }
                if (submitBtn) { submitBtn.disabled = false; submitBtn.style.opacity = '1'; }
            });
        });
    }
})();

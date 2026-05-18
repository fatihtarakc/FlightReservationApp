(function () {
    'use strict';

    var stack = document.getElementById('sw-toast-stack');
    if (!stack) return;

    var toasts;
    try { toasts = JSON.parse(stack.dataset.toasts || '[]'); } catch (e) { return; }
    if (!toasts.length) return;

    var cfg = {
        success: { bg: 'bg-success text-white', icon: 'fa-check-circle',         close: 'btn-close-white' },
        danger:  { bg: 'bg-danger text-white',  icon: 'fa-times-circle',         close: 'btn-close-white' },
        warning: { bg: 'bg-warning text-dark',  icon: 'fa-exclamation-triangle', close: '' },
        info:    { bg: 'bg-info text-dark',     icon: 'fa-info-circle',           close: '' }
    };

    var active = [];

    toasts.forEach(function (item) {
        var t   = item.t || 'info';
        var c   = cfg[t] || cfg.info;
        var div = document.createElement('div');
        div.className = 'toast border-0 shadow mb-2 ' + c.bg;
        div.setAttribute('role', 'alert');
        div.setAttribute('aria-live', 'assertive');
        div.setAttribute('aria-atomic', 'true');
        div.innerHTML =
            '<div class="toast-body d-flex align-items-center gap-2 py-3 px-3">' +
            '<i class="fas ' + c.icon + ' fa-lg flex-shrink-0"></i>' +
            '<span class="fw-semibold flex-grow-1">' + escHtml(item.m || '') + '</span>' +
            '<button type="button" class="btn-close ' + c.close + ' flex-shrink-0" data-bs-dismiss="toast" aria-label="Close"></button>' +
            '</div>';
        stack.appendChild(div);
        var bsT = new bootstrap.Toast(div, { autohide: false });
        bsT.show();
        active.push(bsT);
    });

    /* dismiss all on next click anywhere (after small delay to avoid instant dismiss) */
    if (active.length) {
        setTimeout(function () {
            document.addEventListener('click', function dismissAll() {
                active.forEach(function (t) { try { t.hide(); } catch (e) {} });
                document.removeEventListener('click', dismissAll);
            });
        }, 250);
    }

    function escHtml(s) {
        return String(s)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
})();

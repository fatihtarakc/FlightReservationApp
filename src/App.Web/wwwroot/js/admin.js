// ── Client-side Bootstrap Toast (no TempData required) ──────────────────────
window.swToastClient = function (type, message) {
    var cfg = {
        success: { bg: 'bg-success text-white', icon: 'fa-check-circle',         close: 'btn-close-white' },
        danger:  { bg: 'bg-danger text-white',  icon: 'fa-times-circle',         close: 'btn-close-white' },
        warning: { bg: 'bg-warning text-dark',  icon: 'fa-exclamation-triangle', close: '' },
        info:    { bg: 'bg-info text-dark',     icon: 'fa-info-circle',           close: '' }
    };
    var c = cfg[type] || cfg.info;
    var stack = document.getElementById('sw-toast-stack');
    if (!stack) {
        stack = document.createElement('div');
        stack.id = 'sw-toast-stack';
        stack.className = 'position-fixed end-0 p-3';
        stack.style.cssText = 'top:72px;z-index:1055;min-width:320px;max-width:440px';
        document.body.appendChild(stack);
    }
    var div = document.createElement('div');
    div.className = 'toast border-0 shadow mb-2 ' + c.bg;
    div.setAttribute('role', 'alert');
    div.setAttribute('aria-live', 'assertive');
    div.setAttribute('aria-atomic', 'true');
    div.innerHTML =
        '<div class="toast-body d-flex align-items-center gap-2 py-3 px-3">' +
        '<i class="fas ' + c.icon + ' fa-lg flex-shrink-0"></i>' +
        '<span class="fw-semibold flex-grow-1">' +
        String(message).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;') +
        '</span>' +
        '<button type="button" class="btn-close ' + c.close + ' flex-shrink-0" data-bs-dismiss="toast" aria-label="Close"></button>' +
        '</div>';
    stack.appendChild(div);
    new bootstrap.Toast(div, { autohide: true, delay: 4000 }).show();
};

// ── Validation info-icon popovers (all admin/passenger form inputs) ───────────
(function () {
    var _icons = new WeakMap();

    function showValidationIcon(input, message) {
        if (!input || !message) return;
        var name = input.name || '';
        var vSpan = name ? document.querySelector('[data-valmsg-for="' + name + '"]') : null;
        var anchor = vSpan || (input.closest('.input-group') || input).nextSibling;
        if (!anchor || !anchor.parentNode) return;

        var entry = _icons.get(input);
        if (!entry) {
            var icon = document.createElement('button');
            icon.type = 'button';
            icon.className = 'btn btn-link p-0 ms-1 text-danger border-0 val-info-icon';
            icon.style.cssText = 'font-size:.85rem;vertical-align:middle;line-height:1;display:inline-block';
            icon.setAttribute('tabindex', '-1');
            icon.innerHTML = '<i class="fas fa-info-circle"></i>';
            anchor.parentNode.insertBefore(icon, anchor);
            var pop = new bootstrap.Popover(icon, {
                trigger: 'click',
                placement: 'top',
                content: message,
                container: 'body'
            });
            _icons.set(input, { icon: icon, pop: pop });
        } else {
            entry.icon.style.display = 'inline-block';
            try { entry.pop.setContent({ '.popover-body': message }); } catch (e) {}
        }
    }

    function hideValidationIcon(input) {
        var entry = _icons.get(input);
        if (!entry) return;
        try { entry.pop.hide(); } catch (e) {}
        entry.icon.style.display = 'none';
    }

    function wireServerErrors() {
        document.querySelectorAll('[data-valmsg-for]').forEach(function (span) {
            var fieldName = span.getAttribute('data-valmsg-for');
            if (!fieldName) return;
            var input = document.querySelector('[name="' + fieldName + '"]') ||
                        document.getElementById(fieldName);
            if (!input) return;

            function sync() {
                var txt = span.textContent.trim();
                if (txt) {
                    span.style.display = 'none';
                    showValidationIcon(input, txt);
                } else {
                    span.style.display = '';
                    hideValidationIcon(input);
                }
            }
            sync();
            new MutationObserver(sync).observe(span, { childList: true, subtree: true, characterData: true });
        });
    }

    // Show icon when input fires invalid (from form.checkValidity())
    document.addEventListener('invalid', function (e) {
        var input = e.target;
        if (!input || !input.name) return;
        var msg = input.validationMessage;
        if (msg) showValidationIcon(input, msg);
    }, true);

    // Hide icon when input becomes valid again
    document.addEventListener('change', function (e) {
        var input = e.target;
        if (!input || !_icons.has(input)) return;
        if (input.checkValidity()) hideValidationIcon(input);
    });

    document.addEventListener('input', function (e) {
        var input = e.target;
        if (!input || !_icons.has(input)) return;
        if (input.checkValidity()) hideValidationIcon(input);
    });

    document.addEventListener('DOMContentLoaded', wireServerErrors);
})();

// Show exception detail in log modal
function showLogDetail(ex) {
    const el = document.getElementById('logDetail');
    if (el) el.textContent = ex;
}

document.addEventListener('DOMContentLoaded', function () {

    var _confirmCallback = null;
    var confirmModalEl   = document.getElementById('confirmModal');
    var confirmModalBody = document.getElementById('confirmModalBody');
    var confirmModalOk   = document.getElementById('confirmModalOk');
    var bsConfirmModal   = confirmModalEl ? new bootstrap.Modal(confirmModalEl) : null;

    function showConfirm(msg, onOk) {
        if (!bsConfirmModal) { if (onOk) onOk(); return; }
        if (confirmModalBody) confirmModalBody.textContent = msg;
        _confirmCallback = onOk;
        bsConfirmModal.show();
    }

    window.swShowConfirm = showConfirm;

    if (confirmModalOk) {
        confirmModalOk.addEventListener('click', function () {
            bsConfirmModal.hide();
            if (_confirmCallback) { _confirmCallback(); _confirmCallback = null; }
        });
    }

    // Confirm-action buttons (delete / cancel in list views)
    document.querySelectorAll('.confirm-action-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var url = this.dataset.actionUrl;
            var msg = this.dataset.confirmMsg;
            showConfirm(msg, function () {
                var form = document.getElementById('actionForm');
                if (form) { form.action = url; form.submit(); }
            });
        });
    });

    // Cancel-booking buttons (passenger MyBookings)
    document.querySelectorAll('.cancel-booking-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var id  = this.dataset.bookingId;
            var msg = this.dataset.confirmMsg;
            showConfirm(msg, function () {
                var form = document.getElementById('cancelForm');
                if (form) { form.action = '/Passenger/Booking/Cancel/' + id; form.submit(); }
            });
        });
    });

    // Airport Create: uppercase IATA input
    const iataInput = document.querySelector('[name="IataCode"]');
    if (iataInput) {
        iataInput.addEventListener('input', function () {
            this.value = this.value.toUpperCase();
        });
    }

    // Flight Create: auto-calculate arrival time from schedule duration
    const scheduleSelect = document.getElementById('scheduleSelect');
    if (scheduleSelect) {
        const toLocalDT = function (d) {
            var p = function (n) { return n < 10 ? '0' + n : '' + n; };
            return d.getFullYear() + '-' + p(d.getMonth() + 1) + '-' + p(d.getDate()) + 'T' + p(d.getHours()) + ':' + p(d.getMinutes());
        };
        const calcArrival = function () {
            const opt = scheduleSelect.options[scheduleSelect.selectedIndex];
            if (!opt || !opt.value) return;
            const duration = parseInt(opt.getAttribute('data-duration'));
            if (!duration) return;
            const depInput = document.querySelector('[name="Form.DepartureTime"]');
            if (!depInput || !depInput.value) return;
            const d = new Date(depInput.value);
            d.setMinutes(d.getMinutes() + duration);
            const arrInput = document.querySelector('[name="Form.ArrivalTime"]');
            if (arrInput) arrInput.value = toLocalDT(d);
        };
        scheduleSelect.addEventListener('change', calcArrival);
        const depInput = document.querySelector('[name="Form.DepartureTime"]');
        if (depInput) depInput.addEventListener('change', calcArrival);
    }

    // Flight Create: populate hidden AirlineId when aircraft is selected
    const aircraftSelect = document.getElementById('aircraftSelect');
    const airlineIdInput = document.getElementById('airlineIdInput');
    if (aircraftSelect && airlineIdInput) {
        aircraftSelect.addEventListener('change', function () {
            const opt = this.options[this.selectedIndex];
            airlineIdInput.value = opt ? (opt.getAttribute('data-airline-id') || '') : '';
        });
    }

    // Route Create/Edit: prevent same origin and destination
    const routeForm = document.getElementById('routeForm');
    const originSelect = document.getElementById('originSelect');
    const destinationSelect = document.getElementById('destinationSelect');
    if (routeForm && originSelect && destinationSelect) {
        const msg = routeForm.dataset.sameAirportMsg || '';
        const validate = function () {
            const o = originSelect.value;
            const d = destinationSelect.value;
            if (o && d && o === d) {
                if (typeof swToastClient === 'function') swToastClient('warning', msg);
                destinationSelect.value = '';
            }
        };
        originSelect.addEventListener('change', validate);
        destinationSelect.addEventListener('change', validate);
    }

    // Passenger Settings: password strength meter
    const pwInput = document.querySelector('[name="PasswordForm.NewPassword"]');
    if (pwInput) {
        pwInput.addEventListener('input', function () {
            const pw = this.value;
            let strength = 0;
            if (pw.length >= 8) strength += 25;
            if (/[A-Z]/.test(pw)) strength += 25;
            if (/[0-9]/.test(pw)) strength += 25;
            if (/[^A-Za-z0-9]/.test(pw)) strength += 25;
            const bar = document.getElementById('pwStrength');
            if (bar) {
                bar.style.width = strength + '%';
                bar.className = 'progress-bar ' + (strength <= 25 ? 'bg-danger' : strength <= 50 ? 'bg-warning' : strength <= 75 ? 'bg-info' : 'bg-success');
            }
        });
    }
});

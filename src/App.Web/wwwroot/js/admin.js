// Show exception detail in log modal
function showLogDetail(ex) {
    const el = document.getElementById('logDetail');
    if (el) el.textContent = ex;
}

document.addEventListener('DOMContentLoaded', function () {

    // Confirm-action buttons (delete / cancel in list views)
    document.querySelectorAll('.confirm-action-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const url = this.dataset.actionUrl;
            const msg = this.dataset.confirmMsg;
            if (!confirm(msg)) return;
            const form = document.getElementById('actionForm');
            if (form) { form.action = url; form.submit(); }
        });
    });

    // Cancel-booking buttons (passenger MyBookings)
    document.querySelectorAll('.cancel-booking-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const id = this.dataset.bookingId;
            const msg = this.dataset.confirmMsg;
            if (!confirm(msg)) return;
            const form = document.getElementById('cancelForm');
            if (form) { form.action = '/Passenger/Booking/Cancel/' + id; form.submit(); }
        });
    });

    // Airport Create: uppercase IATA input
    const iataInput = document.querySelector('[name="IataCode"]');
    if (iataInput) {
        iataInput.addEventListener('input', function () {
            this.value = this.value.toUpperCase();
        });
    }

    // Flight Create: auto-calculate arrival time from route duration
    const routeSelect = document.getElementById('routeSelect');
    if (routeSelect) {
        const calcArrival = function () {
            const opt = routeSelect.options[routeSelect.selectedIndex];
            if (!opt || !opt.value) return;
            const duration = parseInt(opt.getAttribute('data-duration'));
            if (!duration) return;
            const depInput = document.querySelector('[name="Form.DepartureTime"]');
            if (!depInput || !depInput.value) return;
            const d = new Date(depInput.value);
            d.setMinutes(d.getMinutes() + duration);
            const arrInput = document.querySelector('[name="Form.ArrivalTime"]');
            if (arrInput) arrInput.value = d.toISOString().slice(0, 16);
        };
        routeSelect.addEventListener('change', calcArrival);
        const depInput = document.querySelector('[name="Form.DepartureTime"]');
        if (depInput) depInput.addEventListener('change', calcArrival);
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
            if (o && d && o === d) { alert(msg); destinationSelect.value = ''; }
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

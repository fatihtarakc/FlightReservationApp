(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {

        // Bootstrap 5 native validation feedback for all forms with novalidate
        document.querySelectorAll('form[novalidate]').forEach(function (form) {
            form.addEventListener('submit', function (e) {
                if (!form.checkValidity()) {
                    e.preventDefault();
                    e.stopPropagation();
                    var msg = form.dataset.validationMsg ||
                              document.body.dataset.valMsg ||
                              'Lütfen zorunlu alanları doğru doldurunuz.';
                    if (typeof swToastClient === 'function') swToastClient('danger', msg);
                }
                form.classList.add('was-validated');
            }, false);

            // Real-time feedback on blur
            form.querySelectorAll('input, select, textarea').forEach(function (el) {
                el.addEventListener('blur', function () {
                    if (form.classList.contains('was-validated')) return;
                    el.classList.toggle('is-invalid', !el.checkValidity());
                    el.classList.toggle('is-valid', el.checkValidity() && el.value !== '');
                });
            });
        });

        // Route form: same-airport client-side guard
        var routeForm = document.getElementById('routeForm');
        if (routeForm) {
            var originSel = document.getElementById('originSelect');
            var destSel   = document.getElementById('destinationSelect');
            var sameMsg   = routeForm.dataset.sameAirportMsg || 'Origin and destination airports must be different.';

            function validateRouteAirports() {
                if (originSel && destSel && originSel.value && destSel.value && originSel.value === destSel.value) {
                    destSel.setCustomValidity(sameMsg);
                } else if (destSel) {
                    destSel.setCustomValidity('');
                }
            }

            if (originSel) originSel.addEventListener('change', validateRouteAirports);
            if (destSel)   destSel.addEventListener('change', validateRouteAirports);
        }

        // Flight form: arrival must be after departure
        var depInput = document.querySelector('[name="Form.DepartureTime"]');
        var arrInput = document.querySelector('[name="Form.ArrivalTime"]');
        if (depInput && arrInput) {
            var flightForm  = depInput.closest('form');
            var arrivalMsg  = (flightForm && flightForm.dataset.arrivalMsg) || 'Arrival time must be after departure time.';

            function validateFlightTimes() {
                if (!depInput.value || !arrInput.value) {
                    arrInput.setCustomValidity('');
                    return;
                }
                var dep = new Date(depInput.value);
                var arr = new Date(arrInput.value);
                if (isNaN(dep.getTime()) || isNaN(arr.getTime())) {
                    arrInput.setCustomValidity('');
                    return;
                }
                arrInput.setCustomValidity(arr <= dep ? arrivalMsg : '');
            }

            depInput.addEventListener('change', validateFlightTimes);
            arrInput.addEventListener('change', validateFlightTimes);
        }

        // Password strength bar (passenger settings)
        var pwdInput    = document.querySelector('[name="PasswordForm.NewPassword"]');
        var strengthBar = document.getElementById('pwStrength');
        if (pwdInput && strengthBar) {
            pwdInput.addEventListener('input', function () {
                var v = this.value;
                var score = 0;
                if (v.length >= 8)            score++;
                if (/[A-Z]/.test(v))          score++;
                if (/[0-9]/.test(v))          score++;
                if (/[^a-zA-Z0-9]/.test(v))   score++;
                var pct = (score / 4) * 100;
                strengthBar.style.width = pct + '%';
                strengthBar.className = 'progress-bar ' + (
                    pct <= 25 ? 'bg-danger'  :
                    pct <= 50 ? 'bg-warning' :
                    pct <= 75 ? 'bg-info'    : 'bg-success'
                );
            });
        }

        // Schedule Code: auto-uppercase on input
        var scheduleCodeInput = document.querySelector('[name="Form.Code"]');
        if (scheduleCodeInput) {
            scheduleCodeInput.addEventListener('input', function () {
                var pos = this.selectionStart;
                this.value = this.value.toUpperCase();
                this.setSelectionRange(pos, pos);
            });
        }

        // Schedule form: ValidTo must be after ValidFrom
        var scheduleValidFrom = document.querySelector('[name="Form.ValidFrom"]');
        var scheduleValidTo   = document.querySelector('[name="Form.ValidTo"]');
        if (scheduleValidFrom && scheduleValidTo) {
            function validateScheduleDates() {
                if (!scheduleValidTo.value) { scheduleValidTo.setCustomValidity(''); return; }
                var from = new Date(scheduleValidFrom.value);
                var to   = new Date(scheduleValidTo.value);
                var msg  = scheduleValidTo.dataset.validToMsg || '';
                scheduleValidTo.setCustomValidity((!isNaN(from) && !isNaN(to) && to <= from) ? msg : '');
            }
            scheduleValidFrom.addEventListener('change', validateScheduleDates);
            scheduleValidTo.addEventListener('change', validateScheduleDates);
        }

        // Schedule form: at least one day must be selected (capture phase, fires before Bootstrap validation)
        var scheduleForm = document.getElementById('scheduleForm');
        if (scheduleForm) {
            scheduleForm.addEventListener('submit', function (e) {
                var checkboxes = scheduleForm.querySelectorAll('input[name="Form.SelectedDays"]');
                if (checkboxes.length === 0) return;
                var anyChecked = Array.from(checkboxes).some(function (c) { return c.checked; });
                if (!anyChecked) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    scheduleForm.classList.add('was-validated');
                    var msg = scheduleForm.dataset.daysMsg || 'En az bir gün seçilmelidir.';
                    if (typeof swToastClient === 'function') swToastClient('danger', msg);
                }
            }, true);
        }

        // IataCode: auto-uppercase on input
        var iataInput = document.querySelector('[name="IataCode"]');
        if (iataInput) {
            iataInput.addEventListener('input', function () {
                var pos = this.selectionStart;
                this.value = this.value.toUpperCase();
                this.setSelectionRange(pos, pos);
            });
        }

        // IcaoCode: auto-uppercase on input
        var icaoInput = document.querySelector('[name="IcaoCode"]');
        if (icaoInput) {
            icaoInput.addEventListener('input', function () {
                var pos = this.selectionStart;
                this.value = this.value.toUpperCase();
                this.setSelectionRange(pos, pos);
            });
        }

        // TailNumber: auto-uppercase on input
        var tailInput = document.querySelector('[name="TailNumber"]');
        if (tailInput) {
            tailInput.addEventListener('input', function () {
                var pos = this.selectionStart;
                this.value = this.value.toUpperCase();
                this.setSelectionRange(pos, pos);
            });
        }

        // FlightNumber: auto-uppercase on input
        var flightNumInput = document.querySelector('[name="Form.FlightNumber"]');
        if (flightNumInput) {
            flightNumInput.addEventListener('input', function () {
                var pos = this.selectionStart;
                this.value = this.value.toUpperCase();
                this.setSelectionRange(pos, pos);
            });
        }
    });
})();

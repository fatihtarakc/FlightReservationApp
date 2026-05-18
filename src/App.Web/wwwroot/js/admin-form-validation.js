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
                var depVal = depInput.value.length === 16 ? depInput.value + ':00' : depInput.value;
                var arrVal = arrInput.value.length === 16 ? arrInput.value + ':00' : arrInput.value;
                var dep = new Date(depVal);
                var arr = new Date(arrVal);
                if (isNaN(dep.getTime()) || isNaN(arr.getTime())) {
                    arrInput.setCustomValidity('');
                    return;
                }
                arrInput.setCustomValidity(arr <= dep ? arrivalMsg : '');
            }

            depInput.addEventListener('change', validateFlightTimes);
            depInput.addEventListener('input',  validateFlightTimes);
            arrInput.addEventListener('change', validateFlightTimes);
            arrInput.addEventListener('input',  validateFlightTimes);

            if (flightForm) {
                flightForm.addEventListener('submit', validateFlightTimes, true);
            }
        }

        // Flight price ordering validation
        var ecoInput   = document.querySelector('[name="Form.EconomyPrice"]');
        var premInput  = document.querySelector('[name="Form.PremiumEconomyPrice"]');
        var bizInput   = document.querySelector('[name="Form.BusinessPrice"]');
        var firstInput = document.querySelector('[name="Form.FirstClassPrice"]');

        if (ecoInput && premInput && bizInput && firstInput) {
            function validatePrices() {
                var eco  = parseFloat(ecoInput.value)   || 0;
                var prem = parseFloat(premInput.value)  || 0;
                var biz  = parseFloat(bizInput.value)   || 0;
                var fst  = parseFloat(firstInput.value) || 0;

                premInput.setCustomValidity(
                    (premInput.value !== '' && prem <= eco)
                        ? (premInput.dataset.gtEconomyMsg || '')
                        : ''
                );
                bizInput.setCustomValidity(
                    (bizInput.value !== '' && biz <= prem)
                        ? (bizInput.dataset.gtPremiumMsg || '')
                        : ''
                );
                firstInput.setCustomValidity(
                    (firstInput.value !== '' && fst <= biz)
                        ? (firstInput.dataset.gtBusinessMsg || '')
                        : ''
                );
            }

            [ecoInput, premInput, bizInput, firstInput].forEach(function (inp) {
                inp.addEventListener('input',  validatePrices);
                inp.addEventListener('change', validatePrices);
            });

            var priceForm = ecoInput.closest('form');
            if (priceForm) {
                priceForm.addEventListener('submit', validatePrices, true);
            }
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

        // Airline combobox ↔ Aircraft combobox
        var airlineSelect  = document.getElementById('airlineSelect');
        var aircraftSelect = document.getElementById('aircraftSelect');
        var airlineIdInput = document.getElementById('airlineIdInput');

        if (airlineSelect && aircraftSelect && airlineIdInput) {
            // Build sorted unique airline list from aircraft options
            var airlinesMap = {};
            Array.from(aircraftSelect.options).forEach(function (opt) {
                var id   = opt.dataset.airlineId;
                var name = opt.dataset.airlineName;
                if (id && name && !/^0+$/.test(id.replace(/-/g, '')))
                    airlinesMap[id] = name;
            });
            Object.keys(airlinesMap).sort(function (a, b) {
                return airlinesMap[a].localeCompare(airlinesMap[b]);
            }).forEach(function (id) {
                var o = document.createElement('option');
                o.value = id;
                o.textContent = airlinesMap[id];
                airlineSelect.appendChild(o);
            });

            // Filter aircraft list to only the selected airline's planes
            function filterAircraft(airlineId) {
                Array.from(aircraftSelect.options).forEach(function (opt) {
                    if (!opt.value) return;
                    opt.hidden = airlineId ? (opt.dataset.airlineId !== airlineId) : false;
                });
                // If current aircraft no longer matches, reset it
                var cur = aircraftSelect.options[aircraftSelect.selectedIndex];
                if (cur && cur.value && cur.dataset.airlineId !== airlineId) {
                    aircraftSelect.value = '';
                    airlineIdInput.value = airlineId;
                }
            }

            // Airline changed → filter aircraft, set hidden field
            airlineSelect.addEventListener('change', function () {
                var id = airlineSelect.value;
                airlineIdInput.value = id;
                filterAircraft(id);
            });

            // Aircraft changed → sync airline select and hidden field
            aircraftSelect.addEventListener('change', function () {
                var opt = aircraftSelect.options[aircraftSelect.selectedIndex];
                var id  = opt ? opt.dataset.airlineId : '';
                airlineSelect.value  = id || '';
                airlineIdInput.value = id || '';
            });

            // Init on page load (Edit scenario: aircraft may already be pre-selected)
            var preSelected = aircraftSelect.options[aircraftSelect.selectedIndex];
            if (preSelected && preSelected.value && preSelected.dataset.airlineId) {
                var preId = preSelected.dataset.airlineId;
                airlineSelect.value  = preId;
                airlineIdInput.value = preId;
                filterAircraft(preId);
            }
        }
    });
})();

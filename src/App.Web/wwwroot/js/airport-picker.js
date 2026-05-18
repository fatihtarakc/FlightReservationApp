(function () {
    'use strict';

    var panel     = document.getElementById('airportPanel');
    var listEl    = document.getElementById('airportList');
    var countryEl = document.getElementById('countryList');
    var closeBtn  = document.getElementById('airportPanelClose');
    var depPicker = document.getElementById('departurePicker');
    var arrPicker = document.getElementById('arrivalPicker');
    var depInput  = document.getElementById('departureIataInput');
    var arrInput  = document.getElementById('arrivalIataInput');
    var depText   = document.getElementById('departureText');
    var arrText   = document.getElementById('arrivalText');

    if (!panel || !depPicker) return;

    var allAirports       = [];
    var availableArrivals = null; // null = show all; array = route-filtered
    var activeField       = null;
    var activeCountry     = null;
    var defaultCountry    = (panel.dataset.defaultCountry || '').trim();

    var labelSelectDep = depPicker.dataset.placeholder || '—';
    var labelSelectArr = arrPicker ? (arrPicker.dataset.placeholder || '—') : '—';

    // ── Bootstrap airport data ───────────────────────────────────────────────

    function _signalReady() {
        document.dispatchEvent(new CustomEvent('airportsReady'));
    }

    var _retryCount = 0;
    var _maxRetries = 5;
    var _retryDelay = 1500;

    function _fetchAirports() {
        fetch('/api/airports-list')
            .then(function (r) { return r.json(); })
            .then(function (data) {
                if (Array.isArray(data) && data.length > 0) {
                    allAirports = data;
                    // If a departure IATA is already selected (e.g. from URL on /flight),
                    // pre-load the filtered arrival list immediately.
                    var existingDep = depInput ? depInput.value.trim() : '';
                    if (existingDep) loadAvailableDestinations(existingDep);
                    _signalReady();
                } else {
                    _scheduleRetry();
                }
            })
            .catch(function () { _scheduleRetry(); });
    }

    function _scheduleRetry() {
        if (_retryCount < _maxRetries) {
            _retryCount++;
            setTimeout(_fetchAirports, _retryDelay);
        } else {
            _signalReady();
        }
    }

    _fetchAirports();

    // ── Route-filtered destinations ──────────────────────────────────────────

    function loadAvailableDestinations(originIata) {
        if (!originIata) { availableArrivals = null; return; }
        fetch('/api/available-destinations?origin=' + encodeURIComponent(originIata))
            .then(function (r) { return r.json(); })
            .then(function (data) { availableArrivals = Array.isArray(data) ? data : null; })
            .catch(function () { availableArrivals = null; });
    }

    // ── Source list for the current field ────────────────────────────────────

    function getSourceList() {
        if (activeField === 'arrival' && availableArrivals !== null) {
            return availableArrivals;
        }
        return allAirports;
    }

    // ── Positioning ──────────────────────────────────────────────────────────

    function positionPanel(trigger) {
        var r = trigger.getBoundingClientRect();
        var w = Math.min(500, window.innerWidth - 16);
        var left = r.left;
        if (left + w > window.innerWidth - 8) left = window.innerWidth - w - 8;
        panel.style.top   = (r.bottom + 4) + 'px';
        panel.style.left  = Math.max(8, left) + 'px';
        panel.style.width = w + 'px';
    }

    // ── Countries ────────────────────────────────────────────────────────────

    function getCountries() {
        var src  = getSourceList();
        var seen = {};
        var list = [];
        src.forEach(function (a) {
            if (a.country && !seen[a.country]) { seen[a.country] = true; list.push(a.country); }
        });
        return list.sort();
    }

    function renderCountries(countries) {
        if (!countryEl) return;
        countryEl.innerHTML = '';
        countries.forEach(function (c) {
            var item = document.createElement('div');
            item.className = 'country-item' + (c === activeCountry ? ' active' : '');
            item.textContent = c;
            item.addEventListener('click', function () {
                activeCountry = c;
                countryEl.querySelectorAll('.country-item').forEach(function (el) {
                    el.classList.remove('active');
                });
                item.classList.add('active');
                var src = getSourceList();
                renderAirports(src.filter(function (a) { return a.country === c; }));
                listEl.scrollTop = 0;
            });
            countryEl.appendChild(item);
        });
    }

    // ── Airports ─────────────────────────────────────────────────────────────

    function renderAirports(airports) {
        if (!listEl) return;
        listEl.innerHTML = '';
        airports.forEach(function (a) {
            var item = document.createElement('div');
            item.className = 'airport-item';
            var city = document.createElement('div');
            city.className   = 'airport-item-city';
            city.textContent = a.city;
            var iata = document.createElement('div');
            iata.className   = 'airport-item-iata';
            iata.textContent = a.iataCode;
            item.appendChild(city);
            item.appendChild(iata);
            item.addEventListener('click', function () { selectAirport(a); });
            listEl.appendChild(item);
        });
    }

    // ── Open / Close ─────────────────────────────────────────────────────────

    function openPanel(field, trigger) {
        activeField = field;
        positionPanel(trigger);
        trigger.classList.add('picker-open');

        var countries = getCountries();

        // Pick the active country: keep current if still valid, else prefer the
        // configured default, else fall back to the first available country.
        if (!activeCountry || countries.indexOf(activeCountry) === -1) {
            activeCountry = countries.indexOf(defaultCountry) !== -1
                ? defaultCountry
                : (countries[0] || null);
        }

        renderCountries(countries);
        var src = getSourceList();
        renderAirports(activeCountry
            ? src.filter(function (a) { return a.country === activeCountry; })
            : src);
        var activeEl = countryEl ? countryEl.querySelector('.country-item.active') : null;
        if (activeEl) activeEl.scrollIntoView({ block: 'nearest' });
        panel.style.display = 'flex';
    }

    function closePanel() {
        panel.style.display = 'none';
        activeField = null;
        if (depPicker) depPicker.classList.remove('picker-open');
        if (arrPicker) arrPicker.classList.remove('picker-open');
    }

    // ── Select ───────────────────────────────────────────────────────────────

    function selectAirport(a) {
        if (activeField === 'departure') {
            if (depInput) depInput.value = a.iataCode;
            if (depText)  { depText.textContent = a.city; depText.classList.remove('airport-placeholder'); }
            // Reset arrival and kick off route-filtered destination fetch
            if (arrInput) arrInput.value = '';
            if (arrText)  { arrText.textContent = labelSelectArr; arrText.classList.add('airport-placeholder'); }
            activeCountry = null; // so arrival panel re-defaults to defaultCountry
            loadAvailableDestinations(a.iataCode);
        } else {
            if (arrInput) arrInput.value = a.iataCode;
            if (arrText)  { arrText.textContent = a.city; arrText.classList.remove('airport-placeholder'); }
        }
        closePanel();
    }

    // ── Trigger clicks ───────────────────────────────────────────────────────

    depPicker.addEventListener('click', function (e) {
        e.stopPropagation();
        panel.style.display === 'flex' && activeField === 'departure'
            ? closePanel()
            : openPanel('departure', depPicker);
    });

    if (arrPicker) {
        arrPicker.addEventListener('click', function (e) {
            e.stopPropagation();
            panel.style.display === 'flex' && activeField === 'arrival'
                ? closePanel()
                : openPanel('arrival', arrPicker);
        });
    }

    if (closeBtn) closeBtn.addEventListener('click', closePanel);

    document.addEventListener('click', function (e) {
        if (panel.style.display !== 'flex') return;
        if (panel.contains(e.target) || e.target === depPicker || e.target === arrPicker) return;
        closePanel();
    });

    document.addEventListener('keydown', function (e) { if (e.key === 'Escape') closePanel(); });

    window.addEventListener('resize', function () {
        if (panel.style.display !== 'flex') return;
        positionPanel(activeField === 'departure' ? depPicker : arrPicker);
    });

    window.addEventListener('scroll', function () {
        if (panel.style.display !== 'flex') return;
        positionPanel(activeField === 'departure' ? depPicker : arrPicker);
    }, true);
})();

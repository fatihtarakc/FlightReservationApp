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

    var allAirports   = [];
    var activeField   = null;
    var activeCountry = null;

    var labelSelectDep = depPicker.dataset.placeholder || '—';
    var labelSelectArr = arrPicker ? (arrPicker.dataset.placeholder || '—') : '—';

    fetch('/api/airports-list')
        .then(function (r) { return r.json(); })
        .then(function (data) { allAirports = data; })
        .catch(function () {});

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
        var seen = {};
        var list = [];
        allAirports.forEach(function (a) {
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
                renderAirports(allAirports.filter(function (a) { return a.country === c; }));
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
        if (!activeCountry && countries.length) activeCountry = countries[0];
        renderCountries(countries);
        renderAirports(activeCountry
            ? allAirports.filter(function (a) { return a.country === activeCountry; })
            : allAirports);
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
            if (arrInput) arrInput.value = '';
            if (arrText)  { arrText.textContent = labelSelectArr; arrText.classList.add('airport-placeholder'); }
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

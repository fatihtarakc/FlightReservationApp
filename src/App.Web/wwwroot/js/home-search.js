(function () {
    'use strict';

    var paxToggle = document.getElementById('passengerToggle');
    var paxPanel  = document.getElementById('passengerPanel');
    var paxPicker = document.getElementById('passengerPicker');

    if (!paxPicker) return;

    var labelPassenger = paxPicker.dataset.labelPassenger || 'Yolcu';
    var labelEconomy   = paxPicker.dataset.labelEconomy   || 'Economy';
    var labelBusiness  = paxPicker.dataset.labelBusiness  || 'Business';

    var state = {
        cabin:      parseInt(paxPicker.dataset.cabin       || '1') || 1,
        passengers: parseInt(paxPicker.dataset.passengers  || '1') || 1,
        maxSeats:   9
    };

    // ── Passenger UI ─────────────────────────────────────────────────────────

    function updatePaxUI() {
        var cabinName = state.cabin === 3 ? labelBusiness : labelEconomy;
        var sumEl  = document.getElementById('passengerSummary');
        var pIn    = document.getElementById('passengersInput');
        var cIn    = document.getElementById('seatClassInput');
        var cntEl  = document.getElementById('passengerCount');
        if (sumEl)  sumEl.textContent    = state.passengers + ' ' + labelPassenger + ' · ' + cabinName;
        if (pIn)    pIn.value            = state.passengers;
        if (cIn)    cIn.value            = state.cabin;
        if (cntEl)  cntEl.textContent    = state.passengers;
        var minusBtn = document.getElementById('paxMinus');
        var plusBtn  = document.getElementById('paxPlus');
        if (minusBtn) minusBtn.disabled  = state.passengers <= 1;
        if (plusBtn)  plusBtn.disabled   = state.passengers >= state.maxSeats;
        document.querySelectorAll('.pax-cabin-opt').forEach(function (o) {
            o.classList.toggle('active', parseInt(o.dataset.cabin) === state.cabin);
        });
    }

    // ── Panel ────────────────────────────────────────────────────────────────

    function positionPanel() {
        if (!paxPanel || !paxToggle) return;
        var r = paxToggle.getBoundingClientRect();
        paxPanel.style.top   = (r.bottom + 4) + 'px';
        paxPanel.style.left  = r.left + 'px';
        paxPanel.style.width = Math.max(r.width, 280) + 'px';
    }
    function openPanel()  { positionPanel(); paxPanel.style.display = 'block'; }
    function closePanel() { if (paxPanel) paxPanel.style.display = 'none'; }

    if (paxToggle) {
        paxToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            paxPanel && paxPanel.style.display === 'block' ? closePanel() : openPanel();
        });
    }

    document.addEventListener('click', function (e) {
        if (!paxPanel || paxPanel.style.display !== 'block') return;
        if (paxPanel.contains(e.target) || e.target === paxToggle) return;
        closePanel();
    });
    document.addEventListener('keydown', function (e) { if (e.key === 'Escape') closePanel(); });
    window.addEventListener('scroll', function () { if (paxPanel && paxPanel.style.display === 'block') positionPanel(); }, true);
    window.addEventListener('resize', function () { if (paxPanel && paxPanel.style.display === 'block') positionPanel(); });

    var confirmBtn = document.getElementById('passengerConfirm');
    if (confirmBtn) confirmBtn.addEventListener('click', closePanel);

    document.querySelectorAll('.pax-cabin-opt').forEach(function (opt) {
        opt.addEventListener('click', function () {
            state.cabin = parseInt(this.dataset.cabin);
            if (state.passengers > state.maxSeats) state.passengers = Math.max(1, state.maxSeats);
            updatePaxUI();
        });
    });

    var minusBtn = document.getElementById('paxMinus');
    var plusBtn  = document.getElementById('paxPlus');
    if (minusBtn) minusBtn.addEventListener('click', function () {
        if (state.passengers > 1) { state.passengers--; updatePaxUI(); }
    });
    if (plusBtn) plusBtn.addEventListener('click', function () {
        if (state.passengers < state.maxSeats) { state.passengers++; updatePaxUI(); }
    });

    updatePaxUI();
})();

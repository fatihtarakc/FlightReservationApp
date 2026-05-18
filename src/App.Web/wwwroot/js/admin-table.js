(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('table.admin-sortable').forEach(initTable);
    });

    function initTable(table) {
        var tbody = table.querySelector('tbody');
        if (!tbody) return;

        var tableResponsive = table.closest('.table-responsive');
        if (!tableResponsive) return;

        var allRows  = Array.from(tbody.querySelectorAll('tr'));
        var dataRows = allRows.filter(function (r) { return r.querySelectorAll('td').length > 1; });
        var emptyRow = allRows.find(function (r) {
            var td = r.querySelector('td');
            return td && parseInt(td.getAttribute('colspan') || '1') > 1;
        });

        var pageSize    = 10;
        var currentPage = 1;
        var sortCol     = -1;
        var sortAsc     = true;

        /* ── toolbar ────────────────────────────────────────────────────── */
        var toolbar = document.createElement('div');
        toolbar.className = 'px-3 py-2 border-bottom bg-white d-flex align-items-center gap-2 flex-wrap';
        toolbar.innerHTML =
            '<span class="text-muted small fw-semibold">Sayfa başı kayıt:</span>' +
            '<select class="form-select form-select-sm w-auto page-size-sel">' +
            '<option value="10" selected>10</option>' +
            '<option value="20">20</option>' +
            '<option value="50">50</option>' +
            '</select>' +
            '<span class="ms-auto text-muted small page-info"></span>';
        tableResponsive.parentNode.insertBefore(toolbar, tableResponsive);

        /* ── pagination ─────────────────────────────────────────────────── */
        var pagerWrap = document.createElement('div');
        pagerWrap.className = 'px-3 py-2 d-flex justify-content-center';
        pagerWrap.innerHTML = '<ul class="pagination pagination-sm mb-0 tbl-pagination"></ul>';
        tableResponsive.parentNode.insertBefore(pagerWrap, tableResponsive.nextSibling);

        /* ── sort headers ───────────────────────────────────────────────── */
        table.querySelectorAll('th[data-col]').forEach(function (th) {
            th.style.cursor     = 'pointer';
            th.style.userSelect = 'none';
            th.style.whiteSpace = 'nowrap';

            var icon = document.createElement('span');
            icon.className = 'sort-icon ms-1';
            icon.innerHTML = '<i class="fas fa-sort text-secondary opacity-50"></i>';
            th.appendChild(icon);

            th.addEventListener('click', function () {
                var col = parseInt(th.dataset.col);
                if (sortCol === col) {
                    sortAsc = !sortAsc;
                } else {
                    sortCol = col;
                    sortAsc = true;
                }
                table.querySelectorAll('th[data-col] .sort-icon').forEach(function (ic) {
                    ic.innerHTML = '<i class="fas fa-sort text-secondary opacity-50"></i>';
                });
                icon.innerHTML = sortAsc
                    ? '<i class="fas fa-sort-up text-danger"></i>'
                    : '<i class="fas fa-sort-down text-danger"></i>';

                dataRows.sort(function (a, b) {
                    var av = cellValue(a, col);
                    var bv = cellValue(b, col);
                    return sortAsc ? compare(av, bv) : compare(bv, av);
                });

                currentPage = 1;
                render();
            });
        });

        /* ── page size change ───────────────────────────────────────────── */
        toolbar.querySelector('.page-size-sel').addEventListener('change', function () {
            pageSize    = parseInt(this.value);
            currentPage = 1;
            render();
        });

        /* ── helpers ────────────────────────────────────────────────────── */
        function cellValue(row, col) {
            var cells = row.querySelectorAll('td');
            return col < cells.length ? cells[col].textContent.trim() : '';
        }

        function parseNum(s) {
            var n = s.replace(/[₺$€£\s]/g, '');
            /* remove thousand-separator dots (Turkish: 1.234.567) */
            n = n.replace(/\.(?=\d{3}(?:[.,\D]|$))/g, '');
            n = n.replace(',', '.');
            /* strip trailing units: km, dk, etc. */
            n = n.split(/[^\d.-]/)[0];
            return parseFloat(n);
        }

        function parseDate(s) {
            var m = s.match(/^(\d{2})\.(\d{2})\.(\d{4})(?:\s+(\d{2}):(\d{2}))?/);
            if (!m) return null;
            return new Date(+m[3], +m[2] - 1, +m[1], +(m[4] || 0), +(m[5] || 0));
        }

        function compare(a, b) {
            var na = parseNum(a), nb = parseNum(b);
            if (!isNaN(na) && !isNaN(nb)) return na - nb;
            var da = parseDate(a), db = parseDate(b);
            if (da && db) return da - db;
            return a.localeCompare(b, undefined, { sensitivity: 'base', numeric: true });
        }

        /* ── render ─────────────────────────────────────────────────────── */
        function render() {
            var total      = dataRows.length;
            var totalPages = Math.max(1, Math.ceil(total / pageSize));
            if (currentPage > totalPages) currentPage = totalPages;

            var start = (currentPage - 1) * pageSize;
            var end   = Math.min(start + pageSize, total);

            /* rebuild tbody */
            while (tbody.firstChild) tbody.removeChild(tbody.firstChild);

            if (total === 0 && emptyRow) {
                tbody.appendChild(emptyRow);
            } else {
                dataRows.forEach(function (r, i) {
                    r.style.display = (i >= start && i < end) ? '' : 'none';
                    tbody.appendChild(r);
                });
            }

            /* page info */
            toolbar.querySelector('.page-info').textContent =
                total + ' kayıt — ' + (start + 1) + '-' + end + ' gösteriliyor';

            /* pagination */
            var ul = pagerWrap.querySelector('.tbl-pagination');
            ul.innerHTML = '';

            if (totalPages <= 1) { pagerWrap.style.display = 'none'; return; }
            pagerWrap.style.display = '';

            function li(label, page, disabled, active) {
                var item = document.createElement('li');
                item.className = 'page-item' +
                    (disabled ? ' disabled' : '') + (active ? ' active' : '');
                var a = document.createElement('a');
                a.className = 'page-link';
                a.href = '#';
                a.innerHTML = label;
                a.addEventListener('click', function (e) {
                    e.preventDefault();
                    if (!disabled) { currentPage = page; render(); }
                });
                item.appendChild(a);
                ul.appendChild(item);
            }

            li('«', currentPage - 1, currentPage === 1, false);

            var s = Math.max(1, Math.min(currentPage - 2, totalPages - 4));
            var e = Math.min(totalPages, s + 4);
            for (var p = s; p <= e; p++) {
                li(p, p, false, p === currentPage);
            }

            li('»', currentPage + 1, currentPage === totalPages, false);
        }

        /* ── row click navigation ───────────────────────────────────────── */
        dataRows.forEach(function (row) {
            if (!row.dataset.href) return;
            row.style.cursor = 'pointer';
            row.addEventListener('click', function (e) {
                if (e.target.closest('a, button, input, select, textarea')) return;
                window.location.href = row.dataset.href;
            });
        });

        render();
    }
})();

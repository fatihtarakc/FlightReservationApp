$(document).ready(function () {
    var selectedSeatId = null;
    var seatClassNames = { 1: 'Economy', 2: 'Premium Economy', 3: 'Business', 4: 'First Class' };

    $(document).on('click', '.seat-btn[data-available="true"]', function () {
        $('.seat-btn.seat-selected').each(function () {
            var origClass = $(this).data('orig-class');
            if (origClass) $(this).removeClass('seat-selected').addClass(origClass);
        });

        var $btn = $(this);
        var origClass = $btn.hasClass('seat-available') ? 'seat-available'
            : $btn.hasClass('seat-business') ? 'seat-business'
            : $btn.hasClass('seat-first') ? 'seat-first'
            : 'seat-available';

        $btn.data('orig-class', origClass).removeClass(origClass).addClass('seat-selected');

        selectedSeatId = $btn.data('seat-id');
        $('#selectedSeatNumber').text($btn.data('seat-number'));
        $('#selectedSeatClass').text('(' + (seatClassNames[$btn.data('seat-class')] || '') + ')');
        $('#selectedSeatInfo').removeClass('d-none');
    });

    var bookBtn = document.getElementById('bookNowBtn');
    if (bookBtn) {
        bookBtn.addEventListener('click', function () {
            if (!selectedSeatId) { alert(this.dataset.pleaseSelect || ''); return; }
            window.location.href = this.dataset.bookingUrl + '&seatId=' + selectedSeatId;
        });
    }
});

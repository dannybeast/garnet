$(document).ready(function () {
    $('.only-digits').keydown(function (event) {
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 110 || event.keyCode == 190) {
        }
        else {
            if ((event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    });
});

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
}

function isValidForm(form_id) {

    if (form_id === undefined) {
        form = $('form');
    } else {
        form = form_id;
    }

    var check = true;

    var items = [];

    // validate email
    $(form).find('input.email').each(function () {
        if ($(this).val().length > 0 && !isValidEmailAddress($(this).val())) {
            items.push($(this));
        }
    });

    // validate required-more
    var more = false;

    $(form).find('.required-more').each(function () {
        if ($(this).val().length > 0) {
            more = true;
        }
    });

    $(form).find('.required-more').each(function () {
        if (!more) {
            items.push($(this));
        }
    });

    // validate required
    $(form).find('input.required, textarea.required, select.required').each(function (index) {
        if (($.trim($(this).val()).length == 0 ) || ($(this).val() == 'выберите значение' )){
            items.push($(this));
        }
    });

    // remove errors
    $('input.required, textarea.required, select.required, .required-more, input.email').each(function (index) {
    	if ($(this).next('.chosen-container').length ) {
    		$(this).next('.chosen-container').find('.chosen-single').removeClass('error_red');
    	}
        $(this).removeClass('error_red');
    });
    // set errors
    $(items).each(function () {
    	if ($(this).val() == 'выберите значение' ) {
    		$(this).next('.chosen-container').find('.chosen-single').addClass('error_red');
    	}
        $(this).addClass('error_red');
    });

    return items.length == 0;
}
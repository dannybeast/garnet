function showTemplateOptions() {
    if ($('[name="template_allow_subpartitions"]').attr('checked') == 'checked') {
        $('.child-template-option').show();
    } else {
        $('.child-template-option').hide();
    }
}
$(document).ready(function () {
    $('.chkall').change(function () {
        if ($(this).attr('checked') == 'checked') {
            $('.chk').attr('checked', 'checked');
            $("#copy").show();
        } else {
            $('.chk').removeAttr('checked');
            $("#copy").hide();
        }
    });

    $('.chk').change(function () {
        if ($('input[type=checkbox]:checked').length > 0) {
            $("#copy").show();
        } else {
            $("#copy").hide();
        }
    });

    $('#copy').click(function () {
        var ids = "";

        $('input[type=checkbox].chk:checked').each(function (index) {
            ids = ids + $(this).val() + ',';
        });

        $('#copy').attr('href', $('#copy').attr('href') + '&content_ids=' + ids);
    });

    showTemplateOptions();

    $('[name="template_allow_subpartitions"]').change(function () {
        showTemplateOptions();
    });

    // button
    $('.button_element').button();

    // other
    $('#main_menu a, #pager a').not('.ui-state-hover').hover(function () {
        $(this).addClass('ui-state-hover');
    }, function () {
        $(this).removeClass('ui-state-hover');
    });

    $('.wr_settings .settings').click(function() {
        $(this).parent().find('.drop_down').toggleClass('hidden');
        return false;
    });

    // redactor
        $('.redactor').each(function(index){
            $(this).redactor({
                lang: 'ru',
                imageUpload: $(this).attr('data-upload-image-url')
            });
        });
    
    // sortable
    $('.sortable').sortable({
        update: function (event, ui) {
            var url = ''; var ids = []; var orders = [];

            $('.sortable tr').each(function () {
                url = $(this).parent().attr('data-update-url');
                ids.push($(this).find('td input[name="id"]').val());
                orders.push($(this).index());
            });

            $.ajax({
                type: 'POST',
                url: url,
                data: { ids: ids, orders: orders },
                dataType: 'json',
                traditional: true
            });
        }
    });

    $('.image-sortable').sortable({
        update: function (event, ui) {
            var url = ''; var ids = []; var orders = [];

            $('.image-sortable div').each(function () {
                url = $(this).parent().attr('data-update-url');
                ids.push($(this).attr('data-image-id'));
                orders.push($(this).index());
            });

            $.ajax({
                type: 'POST',
                url: url,
                data: { ids: ids, orders: orders },
                dataType: 'json',
                traditional: true
            });
        }
    });

    $('.file-sortable').sortable({
        update: function (event, ui) {
            var url = ''; var ids = []; var orders = [];

            $('.file-sortable div').each(function () {
                url = $(this).parent().attr('data-update-url');
                ids.push($(this).attr('data-file-id'));
                orders.push($(this).index());
            });

            $.ajax({
                type: 'POST',
                url: url,
                data: { ids: ids, orders: orders },
                dataType: 'json',
                traditional: true
            });
        }
    });

    // grouping fieldset

    $('fieldset.collapsible').each(function (index, fieldset) {
        if ($(fieldset).hasClass('collapsed')) {
            $(fieldset).find('legend').html('<span class="ui-icon ui-icon-triangle-1-e"></span>' + $(fieldset).find('legend').html());
            $(fieldset).find('div').hide();
        }
        else {
            $(fieldset).find('legend').html('<span class="ui-icon ui-icon-triangle-1-s"></span>' + $(fieldset).find('legend').html());
            $(fieldset).find('div').show();
        }
    });

    $('fieldset.collapsible legend').click(function () {
        var fieldset = $(this).parent();

        if (fieldset.hasClass('collapsed')) {
            fieldset.find('legend span.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
            fieldset.find('div').slideDown('medium');
            fieldset.removeClass('collapsed');
            fieldset.addClass('expanded');
        }
        else {
            fieldset.find('legend span.ui-icon').removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
            fieldset.find('div').slideUp('medium');
            fieldset.removeClass('expanded');
            fieldset.addClass('collapsed');
        }
    });
})
$(document).ready(function(){
    $("body").prepend("<div id='admin_panel'><a href='' class='close'>Закрыть [x] </a><a href=''>Редактировать</a><span class='divider'>|</span><a href=''>История изменений</a><span class='divider'>|</span><a href=''>Последние документы</a><div class='clear'></div></div>")
    $("#admin_panel .close").click(function(){
        $(this).parent().hide();
        return false;
    })    
})
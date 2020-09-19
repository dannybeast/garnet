function translite(text) {
    var transl = new Array();
    transl['А'] = 'a'; transl['а'] = 'a';
    transl['Б'] = 'b'; transl['б'] = 'b';
    transl['В'] = 'v'; transl['в'] = 'v';
    transl['Г'] = 'g'; transl['г'] = 'g';
    transl['Д'] = 'd'; transl['д'] = 'd';
    transl['Е'] = 'e'; transl['е'] = 'e';
    transl['Ё'] = 'yo'; transl['ё'] = 'yo';
    transl['Ж'] = 'zh'; transl['ж'] = 'zh';
    transl['З'] = 'z'; transl['з'] = 'z';
    transl['И'] = 'i'; transl['и'] = 'i';
    transl['Й'] = 'j'; transl['й'] = 'j';
    transl['К'] = 'k'; transl['к'] = 'k';
    transl['Л'] = 'l'; transl['л'] = 'l';
    transl['М'] = 'm'; transl['м'] = 'm';
    transl['Н'] = 'n'; transl['н'] = 'n';
    transl['О'] = 'o'; transl['о'] = 'o';
    transl['П'] = 'p'; transl['п'] = 'p';
    transl['Р'] = 'r'; transl['р'] = 'r';
    transl['С'] = 's'; transl['с'] = 's';
    transl['Т'] = 't'; transl['т'] = 't';
    transl['У'] = 'u'; transl['у'] = 'u';
    transl['Ф'] = 'f'; transl['ф'] = 'f';
    transl['Х'] = 'h'; transl['х'] = 'h';
    transl['Ц'] = 'c'; transl['ц'] = 'c';
    transl['Ч'] = 'ch'; transl['ч'] = 'ch';
    transl['Ш'] = 'sh'; transl['ш'] = 'sh';
    transl['Щ'] = 'sch'; transl['щ'] = 'sch';
    transl['Ъ'] = 'j'; transl['ъ'] = 'j';
    transl['Ы'] = 'i'; transl['ы'] = 'i';
    transl['Ь'] = 'j'; transl['ь'] = 'j';
    transl['Э'] = 'e'; transl['э'] = 'e';
    transl['Ю'] = 'yu'; transl['ю'] = 'yu';
    transl['Я'] = 'ya'; transl['я'] = 'ya';
    transl[' '] = '-';

    var result = '';

    for (i = 0; i < text.length; i++) {
        if (transl[text[i]] != undefined) { result += transl[text[i]]; }
        else { result += text[i]; }
    }

    return result;
}
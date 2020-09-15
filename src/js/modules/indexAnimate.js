export default function () {
    let $intro = document.querySelector('.js-intro-animation');
    let deviceWidth = document.documentElement.clientWidth;
    if ($intro) {

        if(deviceWidth > 992){
            let lib = appendScript(`https://code.createjs.com/createjs-2015.11.26.min.js`, function(){
                let script = appendScript(`indexAnimateScript.js`);
                document.body.appendChild(script);
            });

            if (!document.body.classList.contains('createjs-added')) {
                document.body.appendChild(lib);
                document.body.classList.add('createjs-added')
            }
        }

    }

    function appendScript(src, callback){
        var script = document.createElement('script')
        script.setAttribute('type', 'text/javascript')
        script.src = src;
        script.onload = function() {
            if(callback){
            callback();
            }
        };
        return script
    }
}

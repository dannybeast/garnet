import $ from 'jquery';
export default function(){
      
    let $input = $('.js-accelerator-counter');
    if($input){

        var i = this && this.__assign || function() {
                return (i = Object.assign || function(t) {
                    for (var a, n = 1, i = arguments.length; n < i; n++)
                        for (var e in a = arguments[n]) Object.prototype.hasOwnProperty.call(a, e) && (t[e] = a[e]);
                    return t
                }).apply(this, arguments)
            },
            e = function() {
                function t(t, a, n) {
                    var e = this;
                    this.target = t, this.endVal = a, this.options = n, this.version = "2.0.4", this.defaults = {
                        startVal: 0,
                        decimalPlaces: 0,
                        duration: 2,
                        useEasing: !0,
                        useGrouping: !0,
                        // smartEasingThreshold: 999,
                        // smartEasingAmount: 333,
                        separator: ",",
                        decimal: ".",
                        prefix: "",
                        suffix: ""
                    }, this.finalEndVal = null, this.useEasing = !0, this.countDown = !1, this.error = "", this.startVal = 0, this.paused = !0, this.count = function(t) {
                        e.startTime || (e.startTime = t);
                        var a = t - e.startTime;
                        e.remaining = e.duration - a, e.useEasing ? e.countDown ? e.frameVal = e.startVal - e.easingFn(a, 0, e.startVal - e.endVal, e.duration) : e.frameVal = e.easingFn(a, e.startVal, e.endVal - e.startVal, e.duration) : e.countDown ? e.frameVal = e.startVal - (e.startVal - e.endVal) * (a / e.duration) : e.frameVal = e.startVal + (e.endVal - e.startVal) * (a / e.duration), e.countDown ? e.frameVal = e.frameVal < e.endVal ? e.endVal : e.frameVal : e.frameVal = e.frameVal > e.endVal ? e.endVal : e.frameVal, e.frameVal = Math.round(e.frameVal * e.decimalMult) / e.decimalMult, e.printValue(e.frameVal), a < e.duration ? e.rAF = requestAnimationFrame(e.count) : null !== e.finalEndVal ? e.update(e.finalEndVal) : e.callback && e.callback()
                    }, this.formatNumber = function(t) {
                        var a, n, i, r, s, o = t < 0 ? "-" : "";
                        if (a = Math.abs(t).toFixed(e.options.decimalPlaces), i = (n = (a += "").split("."))[0], r = n.length > 1 ? e.options.decimal + n[1] : "", e.options.useGrouping) {
                            s = "";
                            for (var l = 0, u = i.length; l < u; ++l) 0 !== l && l % 3 == 0 && (s = e.options.separator + s), s = i[u - l - 1] + s;
                            i = s
                        }
                        return e.options.numerals && e.options.numerals.length && (i = i.replace(/[0-9]/g, function(t) {
                            return e.options.numerals[+t]
                        }), r = r.replace(/[0-9]/g, function(t) {
                            return e.options.numerals[+t]
                        })), o + e.options.prefix + i + r + e.options.suffix
                    }, this.easeOutExpo = function(t, a, n, i) {
                        return n * (1 - Math.pow(2, -10 * t / i)) * 1024 / 1023 + a
                    }, this.options = i({}, this.defaults, n), this.formattingFn = this.options.formattingFn ? this.options.formattingFn : this.formatNumber, this.easingFn = this.options.easingFn ? this.options.easingFn : this.easeOutExpo, this.startVal = this.validateValue(this.options.startVal), this.frameVal = this.startVal, this.endVal = this.validateValue(a), this.options.decimalPlaces = Math.max(this.options.decimalPlaces), this.decimalMult = Math.pow(10, this.options.decimalPlaces), this.resetDuration(), this.options.separator = String(this.options.separator), this.useEasing = this.options.useEasing, "" === this.options.separator && (this.options.useGrouping = !1), this.el = "string" == typeof t ? document.getElementById(t) : t, this.el ? this.printValue(this.startVal) : this.error = "[CountUp] target is null or undefined"
                }
                return t.prototype.determineDirectionAndSmartEasing = function() {
                    var t = this.finalEndVal ? this.finalEndVal : this.endVal;
                    this.countDown = this.startVal > t;
                    var a = t - this.startVal;
                    if (Math.abs(a) > this.options.smartEasingThreshold) {
                        this.finalEndVal = t;
                        var n = this.countDown ? 1 : -1;
                        this.endVal = t + n * this.options.smartEasingAmount, this.duration = this.duration / 2
                    } else this.endVal = t, this.finalEndVal = null;
                    this.finalEndVal ? this.useEasing = !1 : this.useEasing = this.options.useEasing
                }, t.prototype.start = function(t) {
                    this.error || (this.callback = t, this.duration > 0 ? (this.determineDirectionAndSmartEasing(), this.paused = !1, this.rAF = requestAnimationFrame(this.count)) : this.printValue(this.endVal))
                }, t.prototype.pauseResume = function() {
                    this.paused ? (this.startTime = null, this.duration = this.remaining, this.startVal = this.frameVal, this.determineDirectionAndSmartEasing(), this.rAF = requestAnimationFrame(this.count)) : cancelAnimationFrame(this.rAF), this.paused = !this.paused
                }, t.prototype.reset = function() {
                    cancelAnimationFrame(this.rAF), this.paused = !0, this.resetDuration(), this.startVal = this.validateValue(this.options.startVal), this.frameVal = this.startVal, this.printValue(this.startVal)
                }, t.prototype.update = function(t) {
                    cancelAnimationFrame(this.rAF), this.startTime = null, this.endVal = this.validateValue(t), this.endVal !== this.frameVal && (this.startVal = this.frameVal, this.finalEndVal || this.resetDuration(), this.determineDirectionAndSmartEasing(), this.rAF = requestAnimationFrame(this.count))
                }, t.prototype.printValue = function(t) {
                    var a = this.formattingFn(t);
                    "INPUT" === this.el.tagName ? this.el.value = a : "text" === this.el.tagName || "tspan" === this.el.tagName ? this.el.textContent = a : this.el.innerHTML = a
                }, t.prototype.ensureNumber = function(t) {
                    return "number" == typeof t && !isNaN(t)
                }, t.prototype.validateValue = function(t) {
                    var a = Number(t);
                    return this.ensureNumber(a) ? a : (this.error = "[CountUp] invalid start or end value: " + t, null)
                }, t.prototype.resetDuration = function() {
                    this.startTime = null, this.duration = 1e3 * Number(this.options.duration), this.remaining = this.duration
                }, t
            }();
 

    let val = $("#total-counter").val(); 
    let increment = $input.data('increment'); 
    let finish = 9999999999;
    let till = (finish-val) / increment;

    var r = val,
    s = new e("total-counter", finish, {
        useEasing: !1,
        startVal: r,
        decimalPlaces: 9,
        duration: till,
        separator: " "
    });
    s.start()

 }
}
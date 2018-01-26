/**
 * jQuery Callback
 * @author Alberto Bottarini <alberto.bottarini@gmail.com
 * @version 1.1
 * @homepage http://code.google.com/p/jquerycallback
 *
 * jQuery-callback permits jQuery developer to have a real control to their callback functions. 
 * With this plugin you can set custom parameters and custom scope to each callback defined in your script. 
 */

(function (jq) {
    var asArray = function (a) {
        return Array.prototype.slice.call(a, 0);
    }
    jq.delegate = function (func, scope, params, overwriteDefault) {
        return function () {
            if (!$.isArray(params)) params = [params];
            if (!overwriteDefault) params = asArray(arguments).concat(params);
            func.apply(scope, params);
        }
    }
    jq.callback = function (func, params, overwriteDefault) {
        return jq.delegate(func, this, params, overwriteDefault);
    }
})(jQuery);

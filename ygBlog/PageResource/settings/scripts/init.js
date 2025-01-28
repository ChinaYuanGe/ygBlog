var sepicalModifier = {
    "g:password": {
        load: (x) => {
            return "";
        },
        prepare: (x) => {
            return md5(x);
        }
    },
    "mainpage:my_links": {
        load: (x) => {
            return x;
        },
        prepare: (x) => {
            return x.replaceAll('\r', '\n').replaceAll("\n\n","\n");
        }
    }
}

$(function () {
    let list = $("div[ygdat-target]");
    list.each((x) => {
        let target = list[x];
        var textarea = $(target).find("textarea")[0];

        var nk = target.getAttribute("ygdat-target");
        var val = target.getAttribute("ygdat-value");

        if (typeof (sepicalModifier[nk]) == 'object') {
            textarea.value = (sepicalModifier[nk].load(val));
        }
        else textarea.value = val;

        let sendBtn = $(target).find("button")[0];

        $(sendBtn).click((x) => {
            let namekey = nk;
            let value = textarea.value;
            if (typeof (sepicalModifier[namekey]) == 'object'){
                value = sepicalModifier[namekey].prepare(value);
            }
            setRemoteConfig(namekey, value);
        });
    });
});
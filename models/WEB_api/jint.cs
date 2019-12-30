using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;

namespace basicClasses.models.WEB_api
{

    [appliable("Action")]
    [info("filler.   message.body.  Jint is a Javascript interpreter for .NET which provides full ECMA 5.1")]
    class jint : ModelBase
    {
        [model("")]
        [info("javascript module text. creates context of execution that can be reused if omitted in further use ")]
        public static readonly string source = "source";

        [model("only one name of global function, object methods not supported")]
        [info("function")]
        public static readonly string function = "function";

        [model("")]
        [info("same order as arguments of a function")]
        public static readonly string arguments = "arguments";

        Engine eng;

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);
            instanse.ExecActionModelsList(spec[arguments]);

            if (spec.isHere(source))
            eng = new Jint.Engine().Execute(spec.V(source)); ;

            if (eng != null && spec.isHere(function))
            {
                var pr = new object[spec[arguments].listCou];

                //  spec[arguments].ListValues().ToArray();

                try
                {
                    var res = eng.Invoke(spec.V(function), spec[arguments].ListValues().ToArray());

                    message.body = res.ToString();
                }
                catch(Exception e)
                {
                    message.body = e.Message;
                }
            }

            message.CopyArr(new opis());
        }

       // https://sites.google.com/a/chromium.org/chromedriver/
        //ChromeOptions coptions = new ChromeOptions();
        //ChromeOptions options = coptions.setHeadless(true);
        //WebDriver driver = new ChromeDriver(options);


        //        invoke JavaScript function reference

        //    var add = new Engine()
        //        .Execute("function add(a, b) { return a + b; }")
        //        .GetValue("add")
        //        ;

        //        add.Invoke(1, 2); // -> 3
        //or directly by name

        //    var engine = new Engine()
        //        .Execute("function add(a, b) { return a + b; }")
        //        ;

        //        engine.Invoke("add", 1, 2); // -> 3

    }
}

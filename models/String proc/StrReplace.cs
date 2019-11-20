using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [info("filler")]
    [appliable("func BodyValueModificator")]
    class StrReplace : ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string to_replace = "to_replace";

        [info("")]
        [model("")]
        public static readonly string by_this = "by_this";

        [info("in text values <to_replace> and <by_this> use underline to mark spacing")]
        [model("")]
        public static readonly string replace_underline_by_space = "replace_underline_by_space";

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(replace_underline_by_space))
            {
                var repl = string.IsNullOrEmpty(spec.V(to_replace)) ? " " : spec.V(to_replace).Replace('_', ' ');
                message.body = message.body.Replace(repl, spec.V(by_this).Replace('_', ' '));
            }
            else
            {
                var repl = string.IsNullOrEmpty(spec.V(to_replace)) ? " " : spec.V(to_replace);
                message.body = message.body.Replace(repl, spec.V(by_this));
            }
        }
    }
}

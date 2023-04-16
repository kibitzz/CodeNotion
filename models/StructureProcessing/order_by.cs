using Microsoft.Azure.Cosmos.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    [appliable("BodyValueModificator")]
    [info("")]
    public class order_by : ModelBase
    {
        [model("spec_tag")]
        [info("asc    or   desc")]
        public static readonly string order = "order";

        [model("spec_tag")]
        [info(" items part name to sort array")]
        public static readonly string order_by_property = "order_by_property";

        [model("spec_tag")]
        [info(" items partition names")]
        public static readonly string order_by_partition = "order_by_partition";

        [model("spec_tag")]
        [info(" items partition names")]
        public static readonly string order_by_body = "order_by_body";

        [model("spec_tag")]
        [info(" items model names")]
        public static readonly string order_by_model = "order_by_model";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string sort_as_text = "sort_as_text";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string sort_as_number = "sort_as_number";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string sort_by_text_length = "sort_by_text_length";




        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            bool ascOrder = ms.V(order) != "desc";

            int sortType = 1;

            if (ms.isHere(sort_as_text)) {  sortType = 1; } 
            else if(ms.isHere(sort_by_text_length))  { sortType = 2; }
            else if (ms.isHere(sort_as_number)) { sortType = 4; }


            if (ms.isHere(order_by_property))
            {
                message.SortArrayBy_pname_body(ms.V(order_by_property), sortType, ascOrder);
            }

            if (ms.isHere(order_by_partition))
            {
                message.SortThisArrayBy_items_pname(sortType, GetSortType(ms[order_by_partition], ascOrder));
            }

            if (ms.isHere(order_by_body))
            {
                message.SortArrayBy_items_body(sortType, GetSortType(ms[order_by_body], ascOrder));
            }

            if (ms.isHere(order_by_model))
            {
                message.SortThisArrayBy_items_model(sortType, GetSortType(ms[order_by_model], ascOrder));
            }
            
        }

        private bool GetSortType(opis o, bool def)
        {
            return (o.body == "asc" || o.body == "desc") ? (o.body != "desc") : def;
        }

    }
}

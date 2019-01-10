using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
    [info("опис контексту що формується ядром програми(для кожного системного/організаційного поняття) і не може бути зконфігуровано")]
    public class context : ModelBase
    {
        [info("Текст що містить термін, розшифровку котрого містить даний контекст")]
        [model("")]
        public static readonly string Owner = "Owner";

        [info("Системне чи рутове (організаційне) поняття, котре відповідає за предоставлення інформації поточного та вложених контекстів для інших гілок дерева")]
        [model("")]
        public static readonly string Organizer = "Organizer";

        [info("ідентифікатор контексту, створений для випадків коли вміст контексту копіюється до іншого обєкту класу, і по ссилці вони будуть різні, та містять опис одного контексту")]
        [model("")]
        public static readonly string ID = "ID";

        [info("context this belongs to ")]
        [model("wrapper")]
        public static readonly string Higher = "Higher";

        [info("список понять корті знаходяться в даному контексті, якщо елемент цього списку має свій власний контекст, то цей контекст міститься в списку subcon")]
        [model("ListOf_ModelNotion")]
        public static readonly string items = "items";

        [info("список підпорядкованих контекстів")]
        [model("ListOf_context")]
        public static readonly string subcon = "subcon";

        [info("тут містяться специфікації інстансів для конкретного контексту. створюється гілка з іменем інстанса")]
        [model("")]
        public static readonly string instance = "instance";

        [info("список спільний для всіх контекстів дерева понять , котрий заповнюється усіма контекстами. Там кожен організатор контексту створює свою гілку(з іменем поняття) і додає до неї усі контексти де являється організатором ")]
        [model("")]
        public static readonly string sys = "sys";

       
        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using basicClasses.models;
using basicClasses.Factory;
using basicClasses.models.WEB_api;
using System.Net;
using basicClasses.models.sys_ext;

namespace basicClasses
{
    public partial class Form1 : Form
    {
        Parser inputParser;

        bool keyDown;
        bool highlightDerivants;
        bool highlightUnspec;
        bool textcolored;
        bool texthighlight;
        string HighlightedWord;
        opis HighlightedOpis;
        opis TreeViewOpis;
        string EditingProperty;
        string currParseText;

        string PointedWord;
        string HighlightedWordTreeEdited;
        string deletedWord;
        string copiedText;
        Point mouPos;

        /// <summary>
        /// поточний елемент дерева опису поняття
        /// </summary>
        opis EditingOpis;
        opis partInfo;
        opis copiedBranch;
        string EditingOpisValue;

        opis currContext;

        ModelFactory mf;
        Thread moneyThread;
        Thread modelLocherThread;

        int prevSplitPos;
        TreeNode notionTree;
        bool doSplitterMove;

        public Form1()
        {
            InitializeComponent();

            this.Text = "repl";
            doSplitterMove = Screen.PrimaryScreen.Bounds.Width < 1400;

            tabControl2.Appearance = TabAppearance.FlatButtons;
            tabControl2.ItemSize = new Size(0, 1);
            tabControl2.SizeMode = TabSizeMode.Fixed;

            prevSplitPos = splitContainer1.SplitterDistance - 30;

            this.Width = Screen.PrimaryScreen.Bounds.Width + 7;
            this.Height = Screen.PrimaryScreen.Bounds.Height - 32;
            this.Left = -7;
            this.Top = -6;

            //   opis.raiseEvents = true;
            Parser.ContextGlobal = new opis();

            inputParser = new Parser();
            Parser p = new Parser();
            DateTime st = DateTime.Now;
            Parser.LoadEnvironment();
            DateTime fin = DateTime.Now;
            TimeSpan ts = fin - st;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
            ServicePointManager.DefaultConnectionLimit = 20;

           

            treeView2.ShowNodeToolTips = false;
            treeView3.ShowNodeToolTips = false;           
            //  treeView3.ShowLines = false;

            treeView3.ItemHeight = 32;
            treeView3.Indent = 34;
            treeView3.LineColor = Color.FromArgb(59,59,59);

            #region node drag n drop sorting   
            treeView3.AllowDrop = true;
            // Add event handlers for the required drag events.  
            treeView3.ItemDrag += new ItemDragEventHandler(treeView1_ItemDrag);
            treeView3.DragEnter += new DragEventHandler(treeView1_DragEnter);
            treeView3.DragOver += new DragEventHandler(treeView1_DragOver);
            treeView3.DragDrop += new DragEventHandler(treeView1_DragDrop);


            #endregion

            //treeView3.ShowPlusMinus = false;
            //// treeView3.ImageList
            //TreeNode tn = new TreeNode();
            //tn.b
            treeView2.Nodes.Clear();
            //treeView2.Nodes.Add(Parser.ContextGlobal.GetDebugTree(3));

            string[] arr = DataFileUtils.LoadLines("code.txt");
            richTextBox1.Lines = arr;

            arr = DataFileUtils.LoadLines("codeInput.txt");
            richTextBox3.Lines = arr;
            //richTextBox3.AppendText("   context parsed "+ts.ToString());

            currParseText = "";

            opis.listOfVisualisedCircularRefs = new opis();
            mouPos = new Point(0, 0);
         //   button6_Click(null, null);
            colorInput();

            mf = new ModelFactory();
            BuildModelsList("all", "");

            PointedWord = "";

            SysInstance.callbackFrm = this;
            guiGelegate gui = new guiGelegate(popupBanner);
            SysInstance.updateform = gui;

           
            opis.do_not_build_debug = false;
            CreateBrowserW();

        }

        public void popupBanner()
        {
            textBox5.Text = SysInstance.messageBannertext;
            textBox5.Visible = true;
        }

        public void popupBanner(string msg)
        {
            textBox5.Text = msg;
            textBox5.Visible = true;
        }

        public void CreateBrowserW()
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string regTempl = textBox1.Text;
            string data = textBox2.Text;

            Regex mask = new Regex(regTempl);
            checkBox1.Checked = false;
            if (mask.IsMatch(data))
            {
                checkBox1.Checked = true;
              
            }

            textBox3.Text = "";
            Match m = mask.Match(data);

            while (m.Success)
            {

                textBox3.Text += "    " + m.Groups["first"].Value;

                textBox3.Text += "    " + m.Groups["second"].Value;
                textBox3.Text += "    " + m.Groups["third"].Value;
                textBox3.Text += "    " + m.Groups["four"].Value;


                m = m.NextMatch();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Parser p = new Parser();
            string[] arr = richTextBox1.Lines;
            DataFileUtils.savefile(arr, "code.txt");

            Array.Reverse(arr);

            p.dataOut = new string[10000];

            opis conte = new opis();
            //p.parse(conte, arr);

            richTextBox2.Lines = p.dataOut;

            string ddd = this.richTextBox1.Rtf;

            color();

            treeView1.Nodes.Clear();

            opis replica = conte["curr_object"].Duplicate();


            replica.CleanPartitions("currPart,currPartType,object_type");
            treeView1.Nodes.Add(replica.GetDebugTree());
            replica.TransformLoadedRaw();
            replica.CleanPartitions("#array,#PartitionName");
            treeView1.Nodes.Add(replica.GetDebugTree());

        }

        void color()
        {
            string rftHeader = @"{\rtf1\ansi\ansicpg1251\deff0\deflang1049{\fonttbl{\f0\fnil\fcharset204 Microsoft Sans Serif;}}";
            //            string rftHeader = @"{\rtf1\ansi\ansicpg1251\deff0{\fonttbl{\f0\fnil\fcharset204 Microsoft Sans Serif;}}";                                         //128  // 114   // 41  143 44    128
            rftHeader += @"{\colortbl ;\red138\green43\blue226;\red139\green0\blue0;\red0\green139\blue139;\red0\green0\blue205;\red0\green0\blue0;\red250\green90\blue160;\red250\green10\blue27;}";
            rftHeader += @"\viewkind4\uc1\pard\cf1\b\f0\fs19 ";


            //            rftHeader += @"\viewkind4\uc1\pard\b\cf1\lang1049\f0\fs18";

            StringBuilder SB = new StringBuilder();

            string[] uncoloredLines = new string[this.richTextBox1.Lines.Length];
            this.richTextBox1.Lines.CopyTo(uncoloredLines, 0);

            string[] arrFunc1 = new string[]{"присвоїти","отримати","зменшити","цикл","елемент","розмір",
                "перевірити","якщо","послідовність","запамятати","збільшити"
            ,"зменшити","інакше","успіх","кінець","межациклу","кінець","кінець"};

            string[] arrFunc = new string[] { "отримати", "перевірити", "елемент", "розмір", "послідовність", "закінчення" };
            //,"","","","","","",""};

            string[] arrCondit = new string[] { "якщо", "інакше", "успіх" };

            string[] arrHelp = new string[] { "і", "або", "то", "чи", "до", "в", "з", "на" };

            string[] arrUnary = new string[] { "присвоїти", "зменшити", "запамятати", "збільшити", "додати" };

            string[] arrCycle = new string[] { "цикл", "межациклу", "циклу", "кінець", "продовжити" };

            string[] arrSecFunc = new string[] { "вивести", "додатьсвойство", "установититипзначення",
                "присвоїтьОбєктПаразамкнута", "присвоїтьзначення" };
            string[] arrBlock = new string[] { "блок", "кінецьблоку" };

            foreach (string uncoloredText in uncoloredLines)
            {
                string[] tmp = uncoloredText.Split();
                int cou = 0;
                foreach (string s in tmp)
                {
                    int t = 0;
                    if (Array.IndexOf(arrFunc, s) != -1) { SB.Append(GetStrColorByExpr(3)); t = 1; }
                    if (Array.IndexOf(arrCondit, s) != -1) { SB.Append(GetStrColorByExpr(1)); t = 1; }
                    if (Array.IndexOf(arrHelp, s) != -1) { SB.Append(GetStrColorByExpr(1)); t = 1; } //5
                    if (Array.IndexOf(arrUnary, s) != -1) { SB.Append(GetStrColorByExpr(4)); t = 1; }
                    if (Array.IndexOf(arrCycle, s) != -1) { SB.Append(GetStrColorByExpr(6)); t = 1; }
                    if (Array.IndexOf(arrSecFunc, s) != -1) { SB.Append(GetStrColorByExpr(5)); t = 1; }
                    if (Array.IndexOf(arrBlock, s) != -1) { SB.Append(GetStrColorByExpr(2)); t = 1; }

                    if (t == 0)
                    {
                        SB.Append(GetStrColorByExpr(cou == 0 ? 3 : 222));
                    }

                    SB.Append(string.IsNullOrWhiteSpace(s) ? "" : " " + s.Replace("{", "\\{").Replace("}", "\\}"));

                    cou++;
                }
                SB.Append(@"\line ");

            }

            string RTF = "";
            RTF = SB.ToString();
            //RTF = RTF.Substring(0, RTF.Length - 5); // delete last "\par "


            this.richTextBox1.Rtf = rftHeader + RTF + "}";
            this.richTextBox1.Update();
        }


        string GetStrColorByExpr(int s)
        {
            string rez;
            switch (s)
            {
                case 1:
                    rez = @"\cf4 ";
                    break;

                case 2:
                    rez = @"\cf1 ";
                    break;
                case 3:
                    rez = @"\cf2 ";
                    break;
                case 4:
                    rez = @"\cf3 ";
                    break;

                case 5:
                    rez = @"\cf6 ";
                    break;

                case 6:
                    rez = @"\cf7 ";
                    break;

                case 7:
                    rez = @"\cf8 ";
                    break;

                case 8:
                    rez = @"\cf9 ";
                    break;

                case 9:
                    rez = @"\cf10 ";
                    break;

                case 10:
                    rez = @"\cf11 ";
                    break;

                case 11:
                    rez = @"\cf12 ";
                    break;

                case 12:
                    rez = @"\cf13 ";
                    break;

                case 13:
                    rez = @"\cf14 ";
                    break;

                case 14:
                    rez = @"\cf15 ";
                    break;

                case 15:
                    rez = @"\cf16 ";
                    break;

                default:
                    rez = @"\cf5 ";
                    break;
            }

            return rez;
        }

        private void button3_Click(object sender, EventArgs e)
        {
          //  Parser.SaveEnvironment();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (moneyThread != null)
                moneyThread.Abort();

            if (modelLocherThread != null)
                modelLocherThread.Abort();


            Parser.SaveEnvironment();

            string[] arr = richTextBox3.Lines;
            DataFileUtils.savefile(arr, "codeInput.txt");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Parser.ContextGlobal = new opis();
            Parser.ContextGlobal.PartitionName = "ContextGlobal";
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox3_SelectionChanged(object sender, EventArgs e)
        {

            //string prev =HighlightedWord;
            ////this.richTextBox4.AppendText(richTextBox3.SelectedText);
            //HighlightedWord = richTextBox3.SelectedText.Trim();     

            //if (HighlightedWord != prev && HighlightedWord.Length > 0 && HighlightedWord.Length <18)
            //{
            //    texthighlight = true;

            //    highlightWord();
            //    textcolored = true;

            //        textcolored = true;
            //        texthighlight = false;
            //}



        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            currParseText = "";

            foreach (string s in richTextBox3.Lines)
            {
                currParseText += " " + s;
            }

            //if (!keyDown && !textcolored && !texthighlight)
            //{
            //    colorInput();
            //}
            textcolored = false;
        }

        void highlightWord()
        {
            listBox2.Visible = false;
            if (prevSplitPos > 0 && doSplitterMove)
                splitContainer1.SplitterDistance = prevSplitPos;

            if (!string.IsNullOrEmpty(HighlightedWord))
            {
               // create and add new word to dictionary
                HighlightedOpis = Parser.ContextGlobal["words"].getForm(HighlightedWord);
                if (string.IsNullOrEmpty(HighlightedOpis.PartitionName))
                {
                    HighlightedOpis = objBases.baseOpisNotion();
                    Parser.ContextGlobal["words"][HighlightedWord] = HighlightedOpis;
                    SetStateEdited();
                }

                colorInput();

                PrepareWordInput();              

            }
        }

        void colorInput()
        {
            textcolored = true;
            string rftHeader = @"{\rtf1\ansi\ansicpg1251\deff0\deflang1049{\fonttbl{\f0\fnil\fcharset204{\*\fname Courier New;}Courier New CYR;}}";
            //            string rftHeader = @"{\rtf1\ansi\ansicpg1251\deff0{\fonttbl{\f0\fnil\fcharset204 Microsoft Sans Serif;}}";           PT Mono                          //128  // 114   // 41  143 44    128
            //                                                                                                                  255	245	238                     6                           7            new          8                 9                        10                            11                    12
            rftHeader += @"{\colortbl ;\red138\green43\blue226;\red139\green0\blue0;\red0\green139\blue139;\red0\green0\blue205;\red255\green224\blue204;\red252\green133\blue184;\red250\green10\blue27;\red77\green166\blue255;\red203\green151\blue255;\red255\green172\blue132;\red191\green235\blue182;\red163\green192\blue254;}";
            rftHeader += @"\viewkind4\uc1\pard\cf1\f0\fs24 ";
            //\viewkind4\uc1\pard\cf1\lang1033\f0\fs28 h\lang1049\f1\par

            //            rftHeader += @"\viewkind4\uc1\pard\b\cf1\lang1049\f0\fs18";

            StringBuilder SB = new StringBuilder();

            //richTextBox3.AutoScrollOffset
            Rectangle r = richTextBox3.DisplayRectangle;
            Point p = new Point(140, r.Height - 13);  // y= 440

            int uuu = richTextBox3.GetCharIndexFromPosition(p);


            int selid = this.richTextBox3.SelectionStart;

            string[] uncoloredLines = new string[this.richTextBox3.Lines.Length];
            this.richTextBox3.Lines.CopyTo(uncoloredLines, 0);

            foreach (string uncoloredText in uncoloredLines)
            {
                string[] tmp = uncoloredText.Split();
                int cou = 0;
                foreach (string s in tmp)
                {
                    string str = s.Trim(new char[] { '(', ')', '{', '}',
                        '.', ',', ':', '[', ']',
                        '!', '#' , '<', '>', '-', '+','=','?'}).Replace("\"", "");


                    opis curr = Parser.ContextGlobal["words"].getForm(str);

                    if (HighlightedOpis == curr)
                    {
                        SB.Append(GetStrColorByExpr(6));
                    }
                    else
                        if (!string.IsNullOrEmpty(curr.body))
                    {
                        SB.Append(GetStrColorByExpr(1));
                    }
                    else
                    {
                        if (highlightUnspec && !curr.isInitlze)
                        {
                            //SB.Append(GetStrColorByExpr(Convert.ToInt32(textBox5.Text))); // 5 for sudjennya
                        }
                        else
                        {
                            string color = GetStrColorByExpr(222);
                            string description = curr.V(ModelNotion.intellection);

                            if (description.Contains("результат")) color = GetStrColorByExpr(9);
                            if (description.Contains("суждение")) color = GetStrColorByExpr(5);
                            if (description.Contains("клас")) color = GetStrColorByExpr(7);
                            if (description.Contains("функция") || description.Contains("дія")) color = GetStrColorByExpr(8);
                            //if (description.Contains("оператор")) color = GetStrColorByExpr(7);
                            if (description.Contains("спецификатор")) color = GetStrColorByExpr(10);
                            if (description.Contains("системн")) color = GetStrColorByExpr(11);

                            if (HighlightedOpis != null && HighlightedOpis.PartitionName != null)
                            {
                                if (highlightDerivants && description.Contains(HighlightedOpis.PartitionName)
                                    && (description.Contains(HighlightedOpis.PartitionName + " ") || description.EndsWith(HighlightedOpis.PartitionName)))
                                    color = GetStrColorByExpr(6);

                                var onto = curr.V(ModelNotion.ontology);
                                if (highlightDerivants && onto.Contains(HighlightedOpis.PartitionName)
                                    && (onto.Contains(HighlightedOpis.PartitionName + " ") || onto.EndsWith(HighlightedOpis.PartitionName)))
                                    color = GetStrColorByExpr(6);
                            }

                            SB.Append(color);
                        }


                    }

                    SB.Append(string.IsNullOrWhiteSpace(s) ? "" : " " + s.Replace("{", "\\{").Replace("}", "\\}"));

                    cou++;
                }
                SB.Append(@"\line ");

            }

            string RTF = "";
            RTF = SB.ToString();
            //RTF = RTF.Substring(0, RTF.Length - 5); // delete last "\par "


            this.richTextBox3.Rtf = rftHeader + RTF + "}";
            //this.richTextBox3.SelectionStart = uuu;
            this.richTextBox3.SelectionStart = selid;
            //richTextBox3.Refresh();

            //this.richTextBox3.Update();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            treeView2.Nodes.Clear();
            //label1.Text = "W count " + Parser.ContextGlobal["words"].listCou.ToString();
            TreeNode m = Parser.ContextGlobal["words"].GetDebugTree(3);
            m.Text += " "+ Parser.ContextGlobal["words"].listCou.ToString();
            treeView2.Nodes.Add(m);
            treeView2.TreeViewNodeSorter = null;
            treeView2.Sort();
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!texthighlight)
            {
                if (e.Node.Tag != null)
                {
                    TreeNode top = treeView2.Nodes[0];

                    if (top.Tag != null
                        && ((opis)top.Tag).PartitionName == "templates_of_models")
                    {
                        DialogResult result1 = MessageBox.Show("Add selected Template?",
    "Important Question",
    MessageBoxButtons.YesNo);

                        if (result1 == DialogResult.Yes)
                        {
                            SetStateEdited();
                            copiedBranch = ((opis)e.Node.Tag).Duplicate();
                            paste_Click(null, null);
                        }
                        return;
                    }

                    if (top.Tag != null
                       && ((opis)top.Tag).PartitionName == "references_list")
                    {
                        treeView3.TopNode = ((opis)e.Node.Tag).treeElem;
                        treeView3.SelectedNode = ((opis)e.Node.Tag).treeElem;
                        return;
                    }


                    if (top.Tag != null
                      && ((opis)top.Tag).PartitionName == "notion_tree")
                    {
                        opis mnmn = new opis();

                        opis clicked = ((opis)e.Node.Tag);
                        mnmn.AddArr(Parser.ContextGlobal["words"].getForm(clicked.PartitionName));
                        mnmn.AddArr(Parser.ContextGlobal["words"].getForm(clicked.PartitionKind));

                        listBox2.Items.Clear();


                        if (SysInstance.Log != null)
                        {
                            MsgLogItem[] lll = new MsgLogItem[SysInstance.Log.Count];
                            SysInstance.Log.CopyTo(lll);
                            foreach (MsgLogItem m in lll)
                            {
                                if (m.haveResponderOrSender(clicked))
                                {
                                    listBox2.Items.Add(m);

                                    if (!mnmn.isHere("I n s t a n c e"))
                                    {
                                        opis j = m.o["THIS_INS_final"].W();

                                        mnmn["I n s t a n c e"].AddArr(j.W("spec"));
                                        mnmn["I n s t a n c e"].AddArr(j["requisites"]);
                                        mnmn["I n s t a n c e"].AddArr(j["tags"]);
                                        //mnmn["I n s t a n c e"]["sharedVariablesContext"] = j["sharedVariablesContext"];
                                    }

                                    opis msginf = new opis();
                                    if (m.o.W("inf").isInitlze)
                                    {
                                        if (m.o.W("inf").PartitionName == "answer")
                                            msginf["answr"].CopyParams(m.o.W("inf"));
                                        else
                                            msginf.CopyParams(m.o.W("inf")[0]);

                                        if (!(m.o.W("inf").PartitionName == "answer") &&
                                            (m.o.W("inf").PartitionKind == "message"))
                                            msginf.CopyParams(m.o.W("inf"));
                                    }
                                    else
                                        msginf.CopyParams(m.o["inf"][0]);

                                    if (msginf.isHere("answer"))
                                        msginf["answer"]["context"] = new opis();

                                    if (msginf.isHere("initiator"))
                                        msginf["initiator"] = new opis();

                                    msginf.PartitionName = m.info;

                                    mnmn["m e s s a g e s"].AddArr(msginf);
                                }
                            }
                        }


                        HighlightedOpis = mnmn;
                        texthighlight = true;
                        colorInput();
                        texthighlight = false;

                        PrepareWordInput();
                        if(mnmn["m e s s a g e s"].treeElem !=null)
                        mnmn["m e s s a g e s"].treeElem.Expand();
                        if(mnmn.listCou <3)
                        mnmn[0].treeElem.Expand();
                        return;
                    }


                    opis mnmnm = ((opis)e.Node.Tag);

                    if (mnmnm.PartitionName == "_path_")
                    {
                        gotoPathToolStripMenuItem_Click(null, null);
                    }
                    else
                    {

                        HighlightedWord = mnmnm.PartitionName;
                        HighlightedOpis = mnmnm;
                        texthighlight = true;
                        colorInput();
                        texthighlight = false;

                        PrepareWordInput();
                        popupBanner("array items count  " + mnmnm.listCou);
                    }
                }
            }

        }


        void PrepareWordInput()
        {
            //opis ooo =mf.GetModel("ModelNotion");

            SaveTreeChangesAnywhere();
            if (HighlightedOpis == null)
            {
                return;
            }          

            if (HighlightedOpis.PartitionKind != ModelNotion.patritionKinda
                || treeView3.TopNode ==null 
                || HighlightedWordTreeEdited == HighlightedOpis.PartitionName
                || HighlightedOpis.treeElem==null)
            {
                HighlightedWordTreeEdited = HighlightedOpis.PartitionKind == ModelNotion.patritionKinda ?
                                            HighlightedOpis.PartitionName : HighlightedWordTreeEdited;
                treeView3.Nodes.Clear();
                treeView3.Nodes.Add(HighlightedOpis.GetDebugTree().FirstNode);
            }
            else
            {
                HighlightedWordTreeEdited = HighlightedOpis.PartitionName;
                treeView3.Nodes.Clear();
                treeView3.TopNode = null;
                
                try
                {
                    treeView3.Nodes.Add(HighlightedOpis.treeElem);
                }
                catch (ArgumentException e)
                {
                    treeView3.Nodes.Add(HighlightedOpis.GetDebugTree().FirstNode);
                }
            }

            treeView3.TopNode.Expand();
            richTextBox4.Text = "";

        }


        private void button7_Click(object sender, EventArgs e)
        {
            if (EditingOpis != null)
            {
                opis ne = new opis();
                ne.PartitionName = getNewPartitionIndexerName(EditingOpis);
                AddElemToEditingOpis(ne);

                SetStateEdited();
            }
            panel1.Visible = false;
            //PrepareWordInput();
        }

        string getNewPartitionIndexerName(opis container)
        {
            string rez = "";
            int i = 1;
            while (container.isHere(i.ToString()))
            {
                i++;
            }

            rez = i.ToString();

            return rez;
        }

        void AddElemToEditingOpis(opis ne)
        {
            EditingOpis.AddArr(ne);

            if (EditingOpis.treeElem != null)
            {
                EditingOpis.treeElem.Nodes.Add(ne.GetDebugTree().FirstNode);
            }
            SetStateEdited();
            panel1.Visible = false;
        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {
            if (EditingOpis != null && richTextBox4.Focused)
            {
                EditingOpis.body = richTextBox4.Text;
            }
        }

        private void richTextBox3_MouseHover(object sender, EventArgs e)
        {
            //textBox4.Text = richTextBox3.GetCharIndexFromPosition(Cursor.Position).ToString();


        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox3_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            if (Math.Abs(mouPos.X - e.Location.X) > 15 || Math.Abs(mouPos.Y - e.Location.Y) > 15)
            {
                mouPos = e.Location;
                return;
            }


            toolTip1.SetToolTip(this.richTextBox3, GetPointedWord(e.Location));

        }

        public string GetPointedWord(Point p)
        {
            int i = richTextBox3.GetCharIndexFromPosition(p);

            //textBox4.Text = richTextBox3.GetCharIndexFromPosition(e.Location).ToString();
            string w = GetWordAtIdx(i);

            PointedWord = w;
            if (w.Length > 0)
            {
                w = w.Trim(new char[] { '(', ')', '{', '}',
                        '.', ',', ':', '[', ']',
                        '!', '#' , '<', '>', '-', '+','=','?'}).Replace("\"", "");

                PointedWord = w;
                w = Parser.ContextGlobal["words"].getForm(w).info();
                //w=  Parser.ContextGlobal["words"][w].info();
            }



            return w;
        }

        public string GetWordAtIdx(int idx)
        {
            string rez = "";

            if (idx < currParseText.Length && currParseText[idx] != ' ')
            {
                int st = -1;
                int fin = -1;


                for (int i = 0; i < 20; i++)
                {
                    if (fin < 0 && idx + i < currParseText.Length && currParseText[idx + i] == ' ')
                    {
                        fin = idx + i;
                    }

                    if (st < 0 && idx - i >= 0 && currParseText[idx - i] == ' ')
                    {
                        st = idx - i;
                    }
                }

                if (st != -1 && fin != -1)
                {
                    if (fin - st < 4 || (idx + 1 != fin && idx - 1 != st))
                    {
                        rez = currParseText.Substring(st + 1, fin - st - 1);
                    }
                }

            }



            return rez;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //highlightUnspec = checkBox2.Checked;
            //colorInput();
        }

        private void richTextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            keyDown = true;
        }

        private void richTextBox3_MouseClick(object sender, MouseEventArgs e)
        {
            highlightDerivants = false;
            GetPointedWord(e.Location);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (HighlightedWord != PointedWord)
                {
                    treeView3.Focus();
                    HighlightedWord = PointedWord;
                    highlightWord();
                    richTextBox3.Focus();
                }
            }
        }

        private void treeView2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void treeView2_KeyDown(object sender, KeyEventArgs e)
        {
            //highlightDerivants = e.Alt;
            if (e.KeyValue == 46) //del
            {
                if (HighlightedOpis != null)
                {
                    Parser.ContextGlobal["words"].RemoveArrElem(HighlightedOpis);

                    deletedWord = HighlightedOpis.body.Length > 0 ? HighlightedOpis.body : HighlightedOpis.PartitionName;
                    treeView2.Nodes.Remove(HighlightedOpis.treeElem);
                }
            }

            if (e.KeyValue == 45) //ins
            {
                if (HighlightedOpis != null && HighlightedOpis.treeElem != null)
                {
                    HighlightedOpis["formz"][deletedWord].body = deletedWord;
                    TreeNode n = HighlightedOpis.GetDebugTree();
                    HighlightedOpis.treeElem.Nodes.Clear();
                    //TreeNode[] arr = new TreeNode[n.Nodes.Count];
                    // n.Nodes.CopyTo(arr, 0);
                    //HighlightedOpis.treeElem.Nodes.AddRange(arr);
                }
            }
        }

        private void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
            Font tooltipFont = new Font("Courier New", 11.0f);
            e.DrawBackground();
            e.DrawBorder();
            //temptooltiptext = e.ToolTipText;
            e.Graphics.DrawString(e.ToolTipText, tooltipFont, Brushes.Black, new PointF(4, 4));

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = TextRenderer.MeasureText(toolTip1.GetToolTip(e.AssociatedControl), new Font("Courier New", 12.0f));

        }

        private void button8_Click(object sender, EventArgs e)
        {
            copiedText = richTextBox4.Text;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox4.Text += copiedText;
        }

        private void richTextBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            highlightDerivants = true;

            HighlightedWord = PointedWord;
            treeView2.Focus();
            highlightWord();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //inputParser.DirectiveBlocks(Parser.ContextGlobal["words"], richTextBox3.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (TreeViewOpis != null && HighlightedOpis == null)
            {
                HighlightedOpis = TreeViewOpis;
            }

            if (HighlightedOpis != null && !listBox2.Visible)
            {
                TreeViewOpis = HighlightedOpis;
                OntologyTreeBuilder tpb = new OntologyTreeBuilder();
                tpb.context = Parser.ContextGlobal["words"];
                opis o = new opis("context");

                opis nnn = tpb.buildOntologyOnly(HighlightedOpis);

                //treeView2.TreeViewNodeSorter = null;
                treeView2.TreeViewNodeSorter = new NodeSorter();
                treeView2.Nodes.Clear();
                //treeView2.Nodes.Add(nnn.GetDebugTree());

                opis wrwr = new opis();
                wrwr.PartitionName = "notion_tree";
                wrwr.AddArr(nnn);

                notionTree = wrwr.GetDebugTree().FirstNode;
                treeView2.Nodes.Add(notionTree);
                treeView2.ExpandAll();
                //treeView2.TopNode.Expand();
            }
            else
            {
                treeView2.Nodes.Clear();
                if (notionTree != null)
                    treeView2.Nodes.Add(notionTree);

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string[] sentenceParts = null;
            if (richTextBox3.SelectedText != null && richTextBox3.SelectedText.Length > 0)
            {
                sentenceParts = richTextBox3.SelectedText.Split();
            }
            else
            {
                sentenceParts = new string[1];
                sentenceParts[0] = HighlightedOpis.PartitionName;
            }

            if (HighlightedOpis != null || sentenceParts != null)
            {
                TreeViewOpis = HighlightedOpis;
                OntologyTreeBuilder tpb = new OntologyTreeBuilder();
                tpb.context = Parser.ContextGlobal["words"];


                opis o = new opis("context");
                o.PartitionName = "context";
                o.Vset("level", "topBranch");
                o.Vset(context.Higher, "none");               
                o.Vset("тип", "глобальний"); 
                o.Vset(models.context.Organizer, "контекстречення");

                foreach (string s in sentenceParts)
                {
                    tpb.buildTree(tpb.context.Find(s), o);
                    // tpb.buildContext(tpb.context.Find(s), o);
                }

                treeView2.Nodes.Clear();
                treeView2.TreeViewNodeSorter = new NodeSorter();

                currContext = o;
                //treeView2.Nodes.Add(o.GetDebugTree());
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //splitContainer1.SplitterDistance = 666;
            //splitContainer2.SplitterDistance = 293;
            if (moneyThread != null)
                moneyThread.Abort();

            if (modelLocherThread == null)
            {
                modelLocherThread = new Thread(new ThreadStart(SysInstance.ModelStopLockThread));
                modelLocherThread.Start();
            }

            if (currContext !=null)
            {
                currContext["globalcomm"].UnlockThisForDuplication();
                currContext["sys"].UnlockThisForDuplication();
                currContext.ClearNodesRef();
            }

            currContext["globalcomm"] = new opis();
            OntologyTreeBuilder tpb = new OntologyTreeBuilder();
            tpb.context = Parser.ContextGlobal["words"];           

            tpb.initInstances(currContext);

            if (SysInstance.Log != null)
                SysInstance.Log.Clear();
            tpb.contextToIgnite = currContext;
          

            Thread thth = new Thread(new ThreadStart(tpb.igniteTree),200000);
            thth.Start();
            
            currContext["globalcomm"].lockThisForDuplication();
            treeView2.Nodes.Clear();
           
            Thread.Sleep(300);

            listBox2.Visible = true;
            listBox2.Items.Clear();

            MsgLogItem[] lll = new MsgLogItem[SysInstance.Log.Count];
            SysInstance.Log.CopyTo(lll);
            foreach (MsgLogItem m in lll)
            {
                listBox2.Items.Add(m);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            currContext["sys"].lockThisForDuplication();

            if (treeView2.Nodes.Count > 0 && treeView2.Nodes[0].Tag != null)
            {
                //((opis)treeView2.Nodes[0].Tag).CleanNodeRef();
            }
            treeView2.Nodes.Clear();
            treeView2.TreeViewNodeSorter = new NodeSorter();

            try
            {

                treeView2.Nodes.Add(((MsgLogItem)listBox2.SelectedItem).o.GetDebugTree(12).Nodes[0]);
                treeView2.TopNode.Expand();
                //treeView2.TopNode.FirstNode.Expand();

                foreach (TreeNode tn in treeView2.TopNode.Nodes)
                {
                    tn.Expand();
                }
            }
            catch
            {

            }
        }

        private void treeView2_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //{
            //    if (treeView2.TopNode.LastNode.IsExpanded)
            //    {
            //        treeView2.CollapseAll();
            //    }
            //    else
            //    {
            //        treeView2.ExpandAll();
            //    }            
            //}
        }

        private void button14_Click(object sender, EventArgs e)
        {
            listBox2.Visible = true;
        }

        public void SaveTreeChanges(string newVal, string bodyVal)
        {
            if (EditingOpis != null && EditingOpis.PartitionName != null && !string.IsNullOrEmpty(newVal))
            {
                string[] arr = newVal.Split('[', ']');
                if (arr.Length == 3)
                {
                    if (EditingOpis.PartitionName != arr[0].Trim()
                        || EditingOpis.PartitionKind != arr[1].Trim()
                        || EditingOpis.body != arr[2].Trim())
                        SetStateEdited();
                    EditingOpis.PartitionName = arr[0].Trim();

                    if (!string.IsNullOrEmpty(arr[1])) // && string.IsNullOrEmpty(EditingOpis.PartitionKind))
                        EditingOpis.PartitionKind = arr[1].Trim();

                    if (!string.IsNullOrWhiteSpace(arr[2]))
                        EditingOpis.body = arr[2].Trim();
                }
            }

            if (EditingOpis != null && !string.IsNullOrEmpty(bodyVal)
                && bodyVal.Trim() != "_")
            {
                if ( EditingOpis.body != bodyVal.Trim())
                    SetStateEdited();
                EditingOpis.body = bodyVal.Trim();
            }

            if (EditingOpis != null && EditingOpis.treeElem != null)
            {
                EditingOpis.treeElem.Text = EditingOpis.PartitionName + "[" + EditingOpis.PartitionKind + "]  " + EditingOpis.body;
            }
        }

        public void SaveTreeChangesAnywhere()
        {
           
            SaveTreeChanges(EditingOpisValue, richTextBox4.Text);

        }

        private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                SaveTreeChangesAnywhere();

                EditingOpis = ((opis)e.Node.Tag);
                EditingOpisValue = e.Node.Text;
                richTextBox4.Text = EditingOpis.body;

                TreeNode parentNode = null;
                string curr = "";
                string parent = "";
                if (e.Node.Parent != null && e.Node.Parent.Tag != null)
                {
                    //parent = ((opis)e.Node.Parent.Tag).PartitionKind;
                    curr = ((opis)e.Node.Parent.Tag).PartitionKind;
                    parentNode = e.Node.Parent;
                }

                if (parentNode != null && parentNode.Parent != null && parentNode.Parent.Tag != null)
                {
                    parent = ((opis)parentNode.Parent.Tag).PartitionKind;
                }

                //curr = !string.IsNullOrEmpty(EditingOpis.PartitionKind) ? EditingOpis.PartitionKind : "";
                //curr = !EditingOpis.isInitlze ? "creation" : curr;

                if (parent == "set") { parent = ""; }
                if (!string.IsNullOrEmpty(curr))
                { partInfo = mf.GetModelInfo(curr); }

                if (partInfo != null && partInfo[EditingOpis.PartitionName].isInitlze)
                {
                    textBox5.Text = partInfo[EditingOpis.PartitionName].body;
                    textBox5.Visible = true;
                }
                else
                {
                    textBox5.Visible = false;
                }

                BuildModelsList(curr, "" /*parent*/);
            }
        }

        private void treeView3_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            EditingOpisValue = e.Label;
            SaveTreeChangesAnywhere();
        }

        public void BuildModelsList(string local, string parent)
        {
            var modlist = mf.getAppliableModels(local, parent);
            toolStrip1.Items.Clear();
            foreach (var s in modlist)
            {
                var itm = toolStrip1.Items.Add(s);
                itm.Tag = mf.GetModel(s);

                itm.ToolTipText = mf.GetModelInfo(s).V(s);
                itm.AutoToolTip = false;
                itm.ForeColor = Color.AntiqueWhite;
                itm.MouseLeave += new System.EventHandler(this.UnHighlightModel);
                itm.MouseHover += new System.EventHandler(this.showInfoAboutModel);

            }

            if (EditingOpis != null && EditingOpis.body != null
                 && EditingOpis.body.Length < 17)
                toolStrip1.Visible = (modlist.Count > 0);
            else
                toolStrip1.Visible = false;
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            if (e.ClickedItem.Tag != null)
            {
                opis builtin = (opis)e.ClickedItem.Tag;

                EditingOpis.PartitionKind = builtin.PartitionKind;
                EditingOpis.CopyArr(builtin.Duplicate());

                TreeNode original = EditingOpis.treeElem;
                var trel = EditingOpis.GetDebugTree().FirstNode;

                TreeNode[] arr = new TreeNode[trel.Nodes.Count];
                trel.Nodes.CopyTo(arr, 0);
                original.Nodes.AddRange(arr);
                original.Text = trel.Text;
                EditingOpis.treeElem = original;

                EditingOpisValue = "";
            }
        }

        private void treeView3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46) //del
            {               
                if (EditingOpis != null && EditingOpis.treeElem != null)
                {
                    SetStateEdited();
                    if (EditingOpis.treeElem.Parent != null && EditingOpis.treeElem.Parent.Tag != null)
                    {
                        ((opis)EditingOpis.treeElem.Parent.Tag).RemoveArrElem(EditingOpis); ;
                    }

                    treeView3.Nodes.Remove(EditingOpis.treeElem);
                }
            }

            if (e.KeyCode == Keys.Space)
            {
                //partInfo = mf.GetModelInfo(EditingOpis.PartitionKind);

                //if (partInfo != null && partInfo[EditingOpis.PartitionKind].isInitlze)
                //{
                //    textBox5.Text = partInfo[EditingOpis.PartitionKind].body;
                //    textBox5.Visible = true;
                //}
            }
        }

        private void treeView3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //if (treeView3.SelectedNode.Tag != null)
                //{
                //    opis o = (opis)treeView3.SelectedNode.Tag;
                //    if (string.IsNullOrEmpty(o.body))
                //    {
                //        o.body = copiedText;
                //        SaveTreeChangesAnywhere();
                //    }
                //    else
                //        copiedText = o.body;
                //}
            }

            if (e.Button == MouseButtons.Middle)
            {
                if (treeView3.SelectedNode.Tag != null)
                {
                    //opis o = (opis)treeView3.SelectedNode.Tag;

                }
            }
        }

        private void textBox5_MouseMove(object sender, MouseEventArgs e)
        {
            textBox5.Visible = false;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //EditingOpis.PartitionName;
            if (HighlightedOpis != null
                && HighlightedOpis.PartitionKind == ModelNotion.patritionKinda
                && EditingOpis != null)
            {
                opis tmp = Parser.ContextGlobal["words"].getForm(HighlightedOpis.PartitionName);
                if (tmp[EditingOpis.PartitionName].isInitlze)
                    tmp[EditingOpis.PartitionName] = EditingOpis;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(mf.GetModel(EditingOpis.PartitionKind).PartitionKind))
            {
                string pk = EditingOpis.PartitionKind;
                opis wordz = Parser.ContextGlobal["words"];
                opis reflist = new opis();

                for (int i = 0; i < wordz.listCou; i++)
                {
                    opis reflisttmp = new opis();
                    wordz[i].FindTreePartitions(pk, wordz[i].PartitionName, reflisttmp);
                    if (reflisttmp.listCou > 0)
                    {
                        reflist.CopyParams(reflisttmp);
                    }
                }

                treeView2.Nodes.Clear();
                treeView2.Nodes.Add(reflist.GetDebugTree().FirstNode);
                treeView2.TopNode.Expand();

            }
        }

        private void copy_Click(object sender, EventArgs e)
        {
            copiedBranch = EditingOpis.Duplicate();
        }

        private void paste_Click(object sender, EventArgs e)
        {
            if (copiedBranch != null)
            {
                SetStateEdited();
                opis tt = copiedBranch.Duplicate();

                if (Control.ModifierKeys == Keys.Alt)
                {
                    EditingOpis.AddArrRange(tt);
                    tt.GetDebugTree();
                    TreeNode[] arr = new TreeNode[tt.listCou];

                    for (int i = 0; i < tt.listCou; i++)
                        arr[i] = tt[i].treeElem;

                   // tt.GetDebugTree().Nodes.CopyTo(arr, 0); some stupid shit                 
                    EditingOpis.treeElem.Nodes.AddRange(arr);
                }
                else
                {
                    EditingOpis.AddArr(tt);
                    EditingOpis.treeElem.Nodes.Add(tt.GetDebugTree().FirstNode);
                }
            }
        }

        private void button7_MouseHover(object sender, EventArgs e)
        {
            if (EditingOpis != null && !string.IsNullOrEmpty(EditingOpis.PartitionKind))
            {
                partInfo = mf.GetModelInfo(EditingOpis.PartitionKind);
                label2.Text = EditingOpis.PartitionKind + " (aval)";
                var modlist = mf.GetModel(EditingOpis.PartitionKind).Duplicate();
                toolStrip2.Items.Clear();
                for (int i = 0; i < modlist.listCou; i++)
                {
                    if (EditingOpis.getPartitionIdx(modlist[i].PartitionName) == -1)
                    {
                        var itm = toolStrip2.Items.Add(modlist[i].PartitionName);
                        itm.Tag = modlist[i];
                        itm.AutoToolTip = false;
                        itm.MouseHover += new System.EventHandler(this.toolStrip2_MouseHover);
                    }
                }

                panel1.Visible = toolStrip2.Items.Count > 0;

            }
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag != null)
            {
                opis builtin = (opis)e.ClickedItem.Tag;
                AddElemToEditingOpis(builtin);
            }
        }

        private void treeView3_Enter(object sender, EventArgs e)
        {
            panel1.Visible = false;

            if(doSplitterMove)
            splitContainer1.SplitterDistance = prevSplitPos;
        }

        private void treeView3_MouseEnter(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void toolStrip2_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void toolStrip2_MouseHover(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                textBox5.Text = partInfo[((ToolStripItem)sender).Text].body;
                textBox5.Visible = true;
            }
        }

        private void showInfoAboutModel(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                ((ToolStripItem)sender).ForeColor = Color.DarkRed;
                textBox5.Text = ((ToolStripItem)sender).ToolTipText;
                textBox5.Visible = textBox5.Text.Length > 0;
                //textBox5.Visible = true;              
            }
        }

        private void UnHighlightModel(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                ((ToolStripItem)sender).ForeColor = Color.AntiqueWhite;

            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            treeView2.Nodes.Clear();
            if (currContext.treeElem != null)
            {
                treeView2.Nodes.Add(currContext.treeElem);
            }
            else
            {
                treeView2.Nodes.Add(currContext.GetDebugTree());
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            listBox2.Visible = true;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            SysInstance.debugNext = true;
            listBox2.Visible = true;
            listBox2.Items.Clear();

            textBox5.Visible = false;
            textBox5.Text = "";

            MsgLogItem[] lll = new MsgLogItem[SysInstance.Log.Count];
            SysInstance.Log.CopyTo(lll);
            foreach (MsgLogItem m in lll)
            {
                listBox2.Items.Add(m);
            }

            treeView2.Nodes.Clear();
            treeView2.Nodes.Add(currContext["globalcomm"].GetDebugTree(5));

            //foreach (MsgLogItem m in SysInstance.Log)
            //{
            //    listBox2.Items.Add(m);
            //}
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            if (EditingOpis != null)
            {
                treeView2.Nodes.Clear();
                treeView2.TreeViewNodeSorter = new NodeSorter();
                treeView2.Nodes.Add(EditingOpis.GetDebugTree(40).FirstNode);
                treeView2.TopNode.Expand();
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            //string pk = "Pause_To_debug";
            //opis wordz = Parser.ContextGlobal["words"];
            //opis reflist = new opis();

            //for (int i = 0; i < wordz.listCou; i++)
            //{
            //    opis reflisttmp = new opis();
            //    wordz[i].FindTreePartitions(pk, wordz[i].PartitionName, reflisttmp);
            //    if (reflisttmp.listCou > 0)
            //    {
            //        reflist.CopyParams(reflisttmp);
            //    }
            //}

            //treeView2.Nodes.Clear();
            //treeView2.Nodes.Add(reflist.GetDebugTree().FirstNode);
        }

        private void treeView2_MouseEnter(object sender, EventArgs e)
        {
            if (!listBox2.Visible && splitContainer1.SplitterDistance > 530 && doSplitterMove)
                prevSplitPos = splitContainer1.SplitterDistance;

            if(doSplitterMove)
            splitContainer1.SplitterDistance = 530;
        }

        private void treeView2_MouseLeave(object sender, EventArgs e)
        {
            //if (prevSplitPos > 600)
            //    prevSplitPos = 528;

            if (!listBox2.Visible && !treeView2.Focused && doSplitterMove)
                splitContainer1.SplitterDistance = prevSplitPos;
        }

        private void treeView3_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            toolStrip1.Visible = false;
            if (EditingOpis != null)
                richTextBox4.Text = EditingOpis.body;
            else
                richTextBox4.Text = "";
        }

        private void button20_Click(object sender, EventArgs e)
        {
            opis templates = Parser.ContextGlobal["words"]["templates_of_models"];
            if (EditingOpis != null)
                templates.AddArr(EditingOpis.Duplicate());
            SetStateEdited();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            opis templates = Parser.ContextGlobal["words"]["templates_of_models"];

            treeView2.Nodes.Clear();
            treeView2.TreeViewNodeSorter = new NodeSorter();
            treeView2.Nodes.Add(templates.GetDebugTree().FirstNode);
            treeView2.TopNode.Expand();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (EditingOpis == null)
                return;

            opis reflist = new opis();

            string[] arr = new string[] { "EditingPart", "Action", "Pause_To_debug", "MsgTemplate", "exec",
                "initValues", "ConditionChecker", "SharedContextRoles","buildTreeVal_sdc_i","TreeDataExtractor", "global_log", "deleted","added", "modified", "moved"};

            if (sender is bool)
            {
                arr = new string[] { "deleted", "added", "modified", "moved" };
            }

            foreach (string s in arr)
            {
                opis reflisttmp = new opis();
                EditingOpis.FindTreePartitions(s, EditingOpis.PartitionName, reflisttmp);

                for (int i = 0; i < reflisttmp.listCou; i++)
                {
                    reflist[s].AddArr(reflisttmp[i][0]);
                }
            }


            reflist.PartitionName = "references_list";
            treeView2.Nodes.Clear();
            treeView2.TreeViewNodeSorter = new NodeSorter();
            treeView2.Nodes.Add(reflist.GetDebugTree().FirstNode);
            treeView2.TopNode.Expand();

            if (!(sender is bool))
            {
                PrepareWordInput();
            }
        

        }

        private void button23_Click(object sender, EventArgs e)
        {
            SavedStateLabel.Text = "";
            var bkgrsave = new Thread(new ThreadStart(Parser.SaveEnvironment));           
            bkgrsave.Start();
          
        }

        private void button24_Click(object sender, EventArgs e)
        {
            ModelNotion mn = new ModelNotion();

            //IActionProcessor processor = (IActionProcessor)mn;
            //mn.CpecifyActionModel(Parser.ContextGlobal["words"]);

            mn.CompileNotion(EditingOpis, Parser.ContextGlobal);

            if (EditingOpis.treeElem != null)
            {
                EditingOpis.treeElem.Nodes.Clear();
                for (int i = 0; i < EditingOpis.listCou; i++)

                    EditingOpis.treeElem.Nodes.Add(EditingOpis[i].GetDebugTree().FirstNode);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void button25_Click(object sender, EventArgs e)
        {
           
            TreeNode now_node = treeView2.SelectedNode == null ?  treeView2.Nodes[0] : treeView2.SelectedNode;

            if (now_node.Tag != null && EditingOpis != null)
            {
                opis now = ((opis)now_node.Tag).Duplicate();

                opis then = null;
                if (EditingOpis == now_node.Tag)
                {
                    then = Parser.ContextGlobal["words"]["version_control"][now.PartitionName].Duplicate();
                }
                else
                 then = EditingOpis.Duplicate();

                
                then.NormalizeNamesForComparison(now);
                then.NormalizeNamesForComparison(then);

                then.CheckForVersionControl(then, now, "added");
                then.CheckForVersionControl(now, then, "deleted");
              
                then.PartitionKind = "previous";
                now.PartitionKind = "current";
                opis comp = new opis("comparison result");
                comp.AddArr(now);
                comp.AddArr(then);

                HighlightedOpis = comp;              
          
                EditingOpis = comp;
                button22_Click(true, null);
                treeView3.Nodes.Clear();
                treeView3.Nodes.Add(comp.GetDebugTree().FirstNode);

            }

        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void kkk(opis o, bool alltypes)
        {
            if (EditingOpis == null)
                return;

            opis reflist = new opis();

            if (alltypes)
            {
                opis so = new opis();
                opis reflisttmp = new opis();
                string typeOfSearch = "name";

                if (!string.IsNullOrEmpty(o.PartitionName))
                {
                    so.PartitionName = o.PartitionName;

                    EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);
                   

                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                    }


                    so = new opis();
                    reflisttmp = new opis();
                    typeOfSearch = "name as model";

                    so.PartitionKind = o.PartitionName;

                    EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                    }


                    so = new opis();
                    reflisttmp = new opis();
                    typeOfSearch = "name as body";

                    so.body = o.PartitionName;

                    EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                    }

                }


                if (!string.IsNullOrEmpty(o.PartitionKind))
                {
                    so = new opis();
                    reflisttmp = new opis();
                    typeOfSearch = "model";

                    so.PartitionKind = o.PartitionKind;

                    EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                    }

                    so = new opis();
                    reflisttmp = new opis();
                    typeOfSearch = "model as name";

                    so.PartitionName = o.PartitionKind;

                    EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                    }

                }

                if (!string.IsNullOrEmpty(o.body))
                {
                    so = new opis();
                    reflisttmp = new opis();
                    typeOfSearch = "body";

                    so.body = o.body;

                    EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                    }

                    so = new opis();
                    reflisttmp = new opis();
                    typeOfSearch = "body as name";

                    so.PartitionName = o.body;

                    EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                    }


                    so = new opis();
                    reflisttmp = new opis();
                    typeOfSearch = "body as model";

                    so.PartitionKind = o.body;

                    EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                    }

                }

                so = new opis();
                reflisttmp = new opis();
                typeOfSearch = "same set";

                so = o;

                EditingOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                for (int i = 0; i < reflisttmp.listCou; i++)
                {
                    reflist[typeOfSearch].AddArr(reflisttmp[i][0]);
                }

            }
            else
            {

                opis reflisttmp = new opis();
                EditingOpis.FindTreePartitions(o, EditingOpis.PartitionName, reflisttmp);

                //HighlightedOpis.FindTreePartitions(o, HighlightedOpis.PartitionName, reflisttmp);

                for (int i = 0; i < reflisttmp.listCou; i++)
                {
                    reflist.AddArr(reflisttmp[i][0]);
                }
            }

            reflist.PartitionName = "references_list";

            treeView2.Nodes.Clear();
            treeView2.TreeViewNodeSorter = new NodeSorter();
            treeView2.Nodes.Add(reflist.GetDebugTree().FirstNode);
            treeView2.TopNode.Expand();

            foreach (TreeNode node in treeView2.TopNode.Nodes)
            {
                if(node.Nodes.Count <12)
                node.Expand();
            }

            PrepareWordInput();
        }



        private void toolStripSearchModel_Click(object sender, EventArgs e)
        {

        }

        private void searchExactlySameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            opis now = null;
            if (treeView2.Focused)
                now = ((opis)treeView2.SelectedNode.Tag);
            if (treeView3.Focused)
                now = ((opis)treeView3.SelectedNode.Tag);

            EditingOpisValue = "";
            richTextBox4.Text = "";

            EditingOpis = HighlightedOpis;
            kkk(now, true);
        }

        private void searchNameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            opis now = null;
            if (treeView2.Focused)
                now = ((opis)treeView2.SelectedNode.Tag);
            if (treeView3.Focused)
                now = ((opis)treeView3.SelectedNode.Tag);

            EditingOpis = Parser.ContextGlobal["words"];

            HighlightedWord = "";
            HighlightedOpis = EditingOpis;

            //PrepareWordInput();
            EditingOpisValue = "";
            richTextBox4.Text = "";

            kkk(now, true);
        }

        private void hideMsgLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox2.Visible = false;
        }

        private void button26_Click(object sender, EventArgs e)
        {
            //BrowserWindow.form.d
            CreateBrowserW();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opis now = new opis();
            if (treeView2.Focused && treeView2.SelectedNode != null)
                now = ((opis)treeView2.SelectedNode.Tag);
            if (treeView3.Focused && treeView3.SelectedNode != null)
                now = ((opis)treeView3.SelectedNode.Tag);

            importexportCtrl.Text = now.serialize();

           // returnHtmlText = Clipboard.GetText(TextDataFormat.Html);
            Clipboard.SetText(importexportCtrl.Text, TextDataFormat.UnicodeText);           
        }

        private void button27_Click(object sender, EventArgs e)
        {
            opis import = new opis();
            import.load(importexportCtrl.Text);
            copiedBranch = import;
            // returnHtmlText = Clipboard.GetText(TextDataFormat.Html);
        }

        private void gotoPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opis now = null;
            if (treeView2.Focused && treeView2.SelectedNode != null)
                now = ((opis)treeView2.SelectedNode.Tag);
            if (treeView3.Focused && treeView3.SelectedNode != null)
                now = ((opis)treeView3.SelectedNode.Tag);

            EditingOpisValue = "";
            richTextBox4.Text = "";
            

            if (now != null && now.PartitionName != null
                && now.PartitionName == "_path_")
            {
                string[] arr = now.body.Trim('-','>').Split('>');

                opis rood = Parser.ContextGlobal["words"].Find(arr[0].Replace("-", ""));
                if (rood.isInitlze)
                {
                    opis tmp = rood;
                    for (int i = 1; i < arr.Length; i++)
                    {
                        string currpart = arr[i].Replace("-", "");
                        if (tmp.isHere(currpart))
                            tmp = tmp[currpart];
                        else break;
                    }

                    EditingOpis = rood;
                    HighlightedWord = "";
                    HighlightedOpis = EditingOpis;

                    listBox2.Visible = false;
                    PrepareWordInput();

                    treeView3.TopNode = tmp.treeElem;
                    treeView3.SelectedNode = tmp.treeElem;
                }
            }

        }

        private void showModelToolStripMenuItem_Click(object sender, EventArgs e)
        {

            opis now = null;
            if (treeView2.Focused && treeView2.SelectedNode != null)
                now = ((opis)treeView2.SelectedNode.Tag);
            if (treeView3.Focused && treeView3.SelectedNode != null)
                now = ((opis)treeView3.SelectedNode.Tag);

            EditingOpisValue = "";
            richTextBox4.Text = "";

            if (now != null && now.PartitionKind != null)
            {
                opis so = new opis();
                so.PartitionName = now.PartitionKind;
                so.PartitionKind = "func";

                opis reflisttmp = new opis();
                HighlightedOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);

                if (reflisttmp.listCou == 0)
                {
                    so.PartitionKind = "Action"; 
                     reflisttmp = new opis();
                    HighlightedOpis.FindTreePartitions(so, EditingOpis.PartitionName, reflisttmp);
                }

                if (reflisttmp.listCou > 0)
                {
                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        now = reflisttmp[i][0];
                    }

                    treeView3.TopNode = now.treeElem;
                    treeView3.SelectedNode = now.treeElem;
                }else
                {
                    string path = "";
                    reflisttmp = new opis();
                    Parser.ContextGlobal["words"].FindTreePartitions(so, "", reflisttmp);
                    for (int i = 0; i < reflisttmp.listCou; i++)
                    {
                        path = reflisttmp[i].PartitionName;
                        now = reflisttmp[i][0].Duplicate();
                        now["_path_"].body = path;
                        break;
                    }

                    treeView2.TreeViewNodeSorter = new NodeSorter();
                    treeView2.Nodes.Clear();
                    TreeNode nnn = now.GetDebugTree();
                    //nnn.Text = reflisttmp.PartitionName + reflisttmp.body;
                    treeView2.Nodes.Add(nnn);
                    treeView2.TopNode.Expand();
                    treeView2.TopNode.FirstNode.Expand();
                    //treeView2.ExpandAll();
                }
            }
        
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var txt = Clipboard.GetText(TextDataFormat.UnicodeText);

            Data_In_Buffer.rawData = txt;

            opis import = new opis();
          
            if (txt.Trim().StartsWith("{") || txt.Trim().StartsWith("["))
                import.load(txt);
            else
            {
                import.load(Compress.DeComprez(txt));
            }

            copiedBranch = import;

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            opis templates = Parser.ContextGlobal["words"]["version_control"];
            if (EditingOpis != null)
            {
                templates[EditingOpis.PartitionName] = EditingOpis.Duplicate();
                SetStateEdited();
            }
        }


        #region node drag n drop sorting          
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.  
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

            // Copy the dragged node when the right mouse button is used.  
            else if (e.Button == MouseButtons.Right)
            {
                DoDragDrop(e.Item, DragDropEffects.Copy);
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.  
            Point targetPoint = treeView3.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.  
            treeView3.SelectedNode = treeView3.GetNodeAt(targetPoint);
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.  
            Point targetPoint = treeView3.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.  
            TreeNode targetNode = treeView3.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.  
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));


            if (draggedNode.Parent == targetNode.Parent)
            {
                opis commonParent = ((opis)draggedNode.Parent.Tag);
                opis dragged = ((opis)draggedNode.Tag);
                opis target = ((opis)targetNode.Tag);

                if (target == null || dragged == null || target == dragged)
                    return;

                int insertPos = -1;

                if (Control.ModifierKeys == Keys.Alt)// one parent resorting -- add as subnode
                {

                    // do insert on same list item
                }
                else
                {
                    SetStateEdited();
                    commonParent.RemoveArrElem(dragged);
                    insertPos = commonParent.FindArrIdx(target);
                    commonParent.InsertArrElem(dragged, insertPos );
                  
                    // do repositioning inside common parent list
                }
                           
              
                draggedNode.Remove();
                commonParent.treeElem.Nodes.Insert(insertPos , draggedNode);
               

            }

           

        
            //if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            //{
            //    // If it is a move operation, remove the node from its current   
            //    // location and add it to the node at the drop location.  
            //    if (e.Effect == DragDropEffects.Move)
            //    {
            //        draggedNode.Remove();
            //        targetNode.Nodes.Add(draggedNode);
            //    }

            //    // If it is a copy operation, clone the dragged node   
            //    // and add it to the node at the drop location.  
            //    else if (e.Effect == DragDropEffects.Copy)
            //    {
            //        targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
            //    }

            //    // Expand the node at the location   
            //    // to show the dropped node.  
            //    targetNode.Expand();
            //}

        }



        #endregion

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button23_Click(null, null);
        }

        private void SetStateEdited()
        {
            SavedStateLabel.Text = "#";
        }
    }

    public class NodeSorter : IComparer
    {       
        public int Compare(object x, object y)
        {
            return 1;
        }
    }
}

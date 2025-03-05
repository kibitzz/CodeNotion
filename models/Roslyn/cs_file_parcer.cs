using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Roslyn
{
    [appliable("file_formats")]
    [info(@"to find text point to edit (using syntax tree names to find in source_file); 
            to use to regenerate same document with modifications;
         ")]
    public class cs_file_parcer : ModelBase
    {      
        public override void Process(opis message)
        {
            if (message.PartitionKind != "stringArray")
                logopis["err:"].body = "message is not a stringArray ";

            opis rez = new opis();
            string[] proc = (string[])message.bodyObject;

            Stack<opis> blocks = new Stack<opis>();
            Stack<opis> chains = new Stack<opis>();           

            char[] separators = new char[] { ' ', '.', '=', '(' , ')' , '{' , '}', '[', ']', ',', ';' };

            char[] blockOpener = new char[] { '(', '{', '[' };
            char[] blockEncloser = new char[] { ')',  '}', ']' };

            char[] chainOpener = new char[] { ' ', '.', '=', '<', '>' };
            char[] chainEncloser = new char[] { ',', ';' };

            opis currChain = null;
            opis currNode = null;
            opis nodeAlreadyChained = null;
            opis currBlock = rez;
            blocks.Push(currBlock);

            string textBlock = "";
            var lineNumber = "0";

            opis AddNewToCurrNode(string itm, string type = "")
            {
                opis d = new opis() { PartitionName = itm, PartitionKind = type };
                d.Vset("line", lineNumber);

                currNode.AddArr(d);

                return d;
            }

            opis AddNewToCurrBlock(string itm, string type = "")
            {
                opis d = new opis() { PartitionName = itm, PartitionKind = type };

                currBlock.AddArr(d);

                return d;
            }

            


            Dictionary<char, string> separatorSubName = new Dictionary<char, string>() {
                 {' ', "_"},
                 {'.', "."},
                 {',', ","},
                 {'(', "("},
                 {')', ")"},
                 {'{', "{"},
                 {'}', "}"}

            };
            
            for (int i = 0; i < proc.Length; i++)
            {
                lineNumber = (i+1).ToString();
                var line = proc[i].TrimStart();

                if (line.StartsWith("//") || line.StartsWith("#"))
                {
                    opis d = new opis() { PartitionName = line };
                    d.Vset("line", lineNumber);

                    currBlock.AddArr(d);

                    continue;
                }

                if(i == 32)
                {
                    i = 32;
                }

                if (!string.IsNullOrWhiteSpace(line))
                {                   
                    foreach (char c in line)
                    {
                        if (!separators.Contains(c))
                        {
                            textBlock += c;
                        }
                        else
                        {
                            opis d = string.IsNullOrWhiteSpace(textBlock) ? null :
                                new opis() { PartitionName = textBlock };

                            d?.Vset("line", lineNumber);
                            currNode = d == null ? currNode : d;

                            textBlock = "";

                            // few separators sequentially " = "
                            if (chainOpener.Contains(c) && currChain != null && nodeAlreadyChained == currNode)
                            {
                                currNode = new opis() { PartitionName = c.ToString() };
                                currNode.Vset("line", lineNumber);

                                currChain.AddArr(currNode);

                                currChain = currNode;
                                nodeAlreadyChained = currNode;
                            }

                            if (chainOpener.Contains(c) && currNode != null && nodeAlreadyChained != currNode)
                            {
                                if(currChain == null)
                                {                                   
                                    currBlock.AddArr(currNode);                                   
                                }
                                else
                                {
                                    currChain.AddArr(currNode);                                    
                                }

                               // currChain = currNode[c.ToString()];
                                currChain = AddNewToCurrNode(c.ToString(), "~");

                                nodeAlreadyChained = currNode;
                            }
                           

                            if (chainEncloser.Contains(c))
                            {
                                if (currNode != null)
                                    currChain?.AddArr(currNode);
                              
                                AddNewToCurrNode(c.ToString(), "");
                               
                                currChain = null;
                                currNode = null;
                            }

                            if (blockOpener.Contains(c))
                            {
                                if (currNode != null)
                                {
                                    if (nodeAlreadyChained != currNode)
                                    {
                                        currChain?.AddArr(currNode);

                                        if (currChain == null)
                                        {                                           
                                            currBlock.AddArr(currNode);
                                            nodeAlreadyChained = currNode;
                                        }
                                    }

                                    currChain = null;

                                    currBlock = AddNewToCurrNode(c.ToString(), "+");
                                    //   currBlock = currNode[c.ToString()];
                                    blocks.Push(currBlock);
                                }
                                else
                                {
                                    // currBlock = currBlock[c.ToString()];
                                    currBlock = AddNewToCurrBlock(c.ToString(), "");
                                    blocks.Push(currBlock);
                                }

                              //  currNode = null;
                            }                           

                            if (blockEncloser.Contains(c) && blocks.Count > 0)
                            {
                                currBlock = blocks.Pop();
                                currBlock = blocks.Peek();
                                currChain = null;
                                //currNode = null;

                                if(currBlock.FindArr(currNode))
                                {
                                    currNode = null;
                                }
                            }
                                                     
                        }

                    }

                }
                else
                {
                    opis d = new opis() { PartitionName = "", PartitionKind = "_" };
                    d.Vset("line", lineNumber);

                    currBlock.AddArr(d);
                }
            }

            message["data"] = rez;

        }

    }
}
